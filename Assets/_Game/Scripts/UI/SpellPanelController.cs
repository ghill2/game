using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellPanelController : MonoBehaviour
{
    private GameObject Selected_arrows;
    private List<Image> cooldownOverlays;

    private List<bool> cooldownFlags;

    private void Start()
    {
        Selected_arrows = GameObject.Find("Selected-Arrows");

        cooldownOverlays = new List<Image>();
        cooldownFlags = new List<bool>();

        for (int i = 0; i < this.transform.childCount - 1; i++) //-1 for Selected-Arrows object
        {
            Transform spellTransform = this.transform.GetChild(i);
            Transform overlayTransform = spellTransform.Find("Cooldown Overlay");

            Image overlayImage = overlayTransform.GetComponent<Image>();
            if (overlayTransform != null)
            {
                cooldownOverlays.Add(overlayImage);
                cooldownFlags.Add(false);
            }
        }

        // Initialize overlays' values
        cooldownOverlays.ForEach(overlay => overlay.fillAmount = 0f);

        // Test Purpose
        // UpdateSelectedArrows(3);
    }

    public void UpdatePanel(int SpellNo)
    {
        UpdateSelectedArrows(SpellNo);
        UpdateCooldownFill(SpellNo);
    }

    private void UpdateSelectedArrows(int SpellNo)
    {
        // Failure states
        if (Selected_arrows == null)
        {
            Debug.Log("Error: Selected Arrows RawImage object is Null");
            return;
        }

        if (!(SpellNo >= 0 && SpellNo <= 3))
        {
            Debug.Log("Error: SpellNo not in range");
            return;
        }

        Transform currentPos = Selected_arrows.transform;
        Debug.Log(-444 + (296 * SpellNo));
        Selected_arrows.transform.localPosition = new Vector2(-444 + (296 * SpellNo), currentPos.localPosition.y);
    }

    
    private void UpdateCooldownFill(int SpellNo)
    {
        if (cooldownFlags[SpellNo])
        {
            Debug.Log($"CooldownFlag {SpellNo} is {cooldownFlags[SpellNo]}");
            return;
        }

        StartCoroutine(CooldownFillAnimation(SpellNo, 5f));
    }

    private IEnumerator CooldownFillAnimation(int SpellNo, float duration = 1f)
    {
        Debug.Log($"Cooldown Fill Animation Starts on Spell {SpellNo}");

        cooldownFlags[SpellNo] = true;

        Image overlay = cooldownOverlays[SpellNo];
        float timer = duration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            overlay.fillAmount = Mathf.Clamp01(timer / duration);
            yield return null;
        }

        overlay.fillAmount = 0f;
        cooldownFlags[SpellNo] = false;

        Debug.Log($"Cooldown Fill Animation Ended on Spell {SpellNo}");
    }
}
