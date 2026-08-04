using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public sealed class EnemyHitResponse : MonoBehaviour
{
    private static readonly int BaseColorProperty =
        Shader.PropertyToID("_BaseColor");

    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color hitColor = Color.white;
    [SerializeField] private Color defeatedColor = Color.gray;
    [SerializeField, Min(0.01f)] private float flashDuration = 0.12f;

    private EnemyHealth health;
    private MaterialPropertyBlock propertyBlock;
    private Color normalColor = Color.white;
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

        if (material != null &&
            material.HasProperty(BaseColorProperty))
        {
            normalColor =
                material.GetColor(BaseColorProperty);
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
    }

    private void HandleDamaged(
        int currentHealth,
        int maximumHealth)
    {
        if (targetRenderer == null)
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

        SetColor(defeatedColor);
    }

    private IEnumerator Flash()
    {
        SetColor(hitColor);

        yield return new WaitForSeconds(flashDuration);

        SetColor(normalColor);
        flashRoutine = null;
    }

    private void SetColor(Color color)
    {
        if (targetRenderer == null)
        {
            return;
        }

        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(BaseColorProperty, color);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }
}