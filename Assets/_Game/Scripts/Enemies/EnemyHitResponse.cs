using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public sealed class EnemyHitResponse : MonoBehaviour
{
    private static readonly int BaseColorProperty =
        Shader.PropertyToID("_BaseColor");
    private static readonly int GltfBaseColorProperty =
        Shader.PropertyToID("baseColorFactor");
    private static readonly int GltfEmissionProperty =
        Shader.PropertyToID("emissiveFactor");

    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color hitColor = Color.white;
    [SerializeField] private Color defeatedColor = Color.gray;
    [SerializeField, Min(0.01f)] private float flashDuration = 0.12f;

    private EnemyHealth health;
    private MaterialPropertyBlock propertyBlock;
    private int colorProperty = -1;
    private bool hasGltfEmission;
    private Color normalColor = Color.white;
    private Color normalEmissionColor = Color.black;
    private Coroutine flashRoutine;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        propertyBlock = new MaterialPropertyBlock();

        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }

        if (targetRenderer == null)
        {
            return;
        }

        Material material = targetRenderer.sharedMaterial;

        if (material == null)
        {
            return;
        }

        // Determine which color property to use based on the shader.
        if (material.HasProperty(BaseColorProperty))
        {
            colorProperty = BaseColorProperty;
        }
        else if (material.HasProperty(GltfBaseColorProperty))
        {
            colorProperty = GltfBaseColorProperty;
        }
        else
        {
            return;
        }

        normalColor = material.GetColor(colorProperty);
        hasGltfEmission = material.HasProperty(GltfEmissionProperty);

        if (hasGltfEmission)
        {
            normalEmissionColor = material.GetColor(GltfEmissionProperty);
        }
    }

    private void OnEnable()
    {
        health.Damaged += HandleDamaged;
        health.Defeated += HandleDefeated;
    }

    private void OnDisable()
    {
        health.Damaged -= HandleDamaged;
        health.Defeated -= HandleDefeated;

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
            SetColors(normalColor, normalEmissionColor);
        }
    }

    private void HandleDamaged(
        int currentHealth,
        int maximumHealth)
    {
        if (targetRenderer == null || colorProperty == -1)
        {
            return;
        }

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(Flash());
    }

    private void HandleDefeated()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        SetColors(defeatedColor, normalEmissionColor * defeatedColor);
    }

    private IEnumerator Flash()
    {
        // Tint emission, so it does not hide the hit colour.
        SetColors(hitColor, normalEmissionColor * hitColor);

        yield return new WaitForSeconds(flashDuration);

        SetColors(normalColor, normalEmissionColor);
        flashRoutine = null;
    }

    private void SetColors(Color color, Color emissionColor)
    {
        if (targetRenderer == null || colorProperty == -1)
        {
            return;
        }

        // A script reload in the Editor can clear this block.
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(colorProperty, color);

        if (hasGltfEmission)
        {
            propertyBlock.SetColor(GltfEmissionProperty, emissionColor);
        }

        targetRenderer.SetPropertyBlock(propertyBlock);
    }
}