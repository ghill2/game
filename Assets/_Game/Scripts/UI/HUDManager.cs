using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    private PlayerHealth playerHealth;

    private PlayerCollectibles playerCollectibles;

    private HUDBarController healthBarController;

    private HUDCollectiblesController hudCollectiblesController;

    private SpellPanelController spellPanelController;

    [SerializeField]
    private PopupController popupController;

    [SerializeField]
    private bool showPopupOnStart = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        // Get PlayerHealth component from the player
        playerHealth = player.GetComponent<PlayerHealth>();
        playerCollectibles = player.GetComponent<PlayerCollectibles>();

        // Event Listener
        playerHealth.OnHealthChanged += UpdateHealthUI;
        playerCollectibles.OnEggCollected += UpdateEggsCount;
        playerCollectibles.OnEggCollected += UpdatePopup;

        // END

        var hpBar = GameObject.Find("HP Bar");
        healthBarController = hpBar.GetComponentInChildren<HUDBarController>();

        hudCollectiblesController = GetComponentInChildren<HUDCollectiblesController>();

        popupController = GetComponentInChildren<PopupController>();

        spellPanelController = GetComponentInChildren<SpellPanelController>();

        // Test Area

        if (showPopupOnStart)
        {
            popupController.ShowPopup(3.0f);
        }

        UpdateSpellPanel(3);
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        Debug.Log($"Updating health UI: Current Health = {currentHealth}, Max Health = {maxHealth}");
        healthBarController.UpdateBar((float)currentHealth / maxHealth);
    }

    private void UpdateEggsCount(object _, CollectiblesEventArgs e)
    {
        Debug.Log($"Updating eggs count UI: Eggs Collected = {e.Eggs}");
        hudCollectiblesController.SetEggsCount(e.Eggs);
    }

    private void UpdatePopup(Texture icon, string text)
    {
        Debug.Log($"Updating Popup Window: Icon = {icon}, text = {text}");
        popupController.UpdateWindow(icon, text);
        if (!popupController.showPopup) popupController.ShowPopup(3.0f);
    }

    private void UpdatePopup(object _, CollectiblesEventArgs e)
    {
        Debug.Log($"Unpacking EventArgs.");
        UpdatePopup(e.Icon, e.Text);
    }

    private void UpdateSpellPanel(int SpellNo)
    {
        Debug.Log($"Updating Spell Panel: Last Spell Casted = {SpellNo}");
        spellPanelController.UpdatePanel(SpellNo);
    }

}
