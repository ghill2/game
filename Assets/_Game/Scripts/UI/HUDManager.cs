using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines.ExtrusionShapes;
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

    private SpellCaster spellCaster;

    [SerializeField]
    private PopupController popupController;

    private List<LevelHintTrigger> levelHintTriggers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        // Get components from the player
        playerHealth = player.GetComponent<PlayerHealth>();
        playerCollectibles = player.GetComponent<PlayerCollectibles>();
        spellCaster = player.GetComponent<SpellCaster>();

        // Get Triggers from level
        levelHintTriggers = new List<LevelHintTrigger>();
        foreach (var trigger in FindObjectsByType<LevelHintTrigger>(FindObjectsSortMode.None))
        {
            levelHintTriggers.Add(trigger);
        }


        // Event Listener
        playerHealth.OnHealthChanged += UpdateHealthUI;
        playerCollectibles.OnEggCollected += UpdateEggsCount;
        playerCollectibles.OnEggCollected += UpdatePopup;
        spellCaster.OnSpellCast += UpdateSpellPanel;
        playerCollectibles.OnBootsCollected += UpdateBootTimer;

        levelHintTriggers.ForEach(trigger => trigger.OnHintTriggerEntered += OnTriggerEntered);

        // END

        var hpBar = GameObject.Find("HP Bar");
        healthBarController = hpBar.GetComponentInChildren<HUDBarController>();

        hudCollectiblesController = GetComponentInChildren<HUDCollectiblesController>();

        popupController = GetComponentInChildren<PopupController>();

        spellPanelController = GetComponentInChildren<SpellPanelController>();

        // Test Area

    }

    private void OnDestroy()
    {
        playerHealth.OnHealthChanged -= UpdateHealthUI;
        playerCollectibles.OnEggCollected -= UpdateEggsCount;
        playerCollectibles.OnEggCollected -= UpdatePopup;
        spellCaster.OnSpellCast -= UpdateSpellPanel;
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        // Debug.Log($"Updating health UI: Current Health = {currentHealth}, Max Health = {maxHealth}");
        healthBarController.UpdateBar((float)currentHealth / maxHealth);
    }

    private void UpdateEggsCount(object _, EggCollectedEventArgs e)
    {
        // Debug.Log($"Updating eggs count UI: Eggs Collected = {e.Eggs}");
        hudCollectiblesController.SetEggsCount(e.Eggs);
    }

    private void UpdatePopup(Texture icon, string text, float duration = 3f)
    {
        // Debug.Log($"Updating Popup Window: Icon = {icon}, text = {text}");
        popupController.UpdateWindow(icon, text, duration);
    }

    private void UpdatePopup(object _, EggCollectedEventArgs e)
    {
        // Debug.Log($"Unpacking EventArgs.");
        UpdatePopup(e.Icon, e.Text);
    }

    private void UpdateBootTimer(object _, BootsCollectedEventArgs e)
    {
        hudCollectiblesController.UpdateBootsTimer(e.Duration);
    }

    private void UpdateSpellPanel(SpellId SpellNo, float recastDelay)
    {
        // Debug.Log($"Updating Spell Panel: Last Spell Casted = {SpellNo}");
        spellPanelController.UpdatePanel(SpellNo, recastDelay);
    }

    private void OnTriggerEntered(int hintId)
    {
        Texture icon;
        string text;
        switch (hintId)
        {
            case 0:
                // -- Movement Tutorial --
                // Chained = true
                // Popup 1
                // Debug.Log($"Level Trigger invoked, hintId = {hintId}");
                icon = Resources.Load<Texture2D>("Popup/wasd-square");
                text = "Move";

                UpdatePopup(icon, text, 2.5f);

                // Popup 2
                icon = Resources.Load<Texture2D>("Popup/spacebar-key");
                text = "Jump";

                UpdatePopup(icon, text, 2.5f);

                // Popup 3
                icon = Resources.Load<Texture2D>("Popup/mouse-click-right-sexybody");
                text = "Look Around";

                UpdatePopup(icon, text, 2.5f);
                break;

            case 1:
                // -- Spells Tutorial --
                // Chained = true
                // Popup 1
                // Debug.Log($"Level Trigger invoked, hintId = {hintId}");
                icon = Resources.Load<Texture2D>("Popup/f-key-tight");
                text = "Fireball";
                
                UpdatePopup(icon, text, 2f);

                // Popup 2
                icon = Resources.Load<Texture2D>("Popup/r-key-tight");
                text = "Frost Blast";

                UpdatePopup(icon, text, 2f);

                // Popup 3
                icon = Resources.Load<Texture2D>("Popup/e-key-tight");
                text = "Electric Storm";

                UpdatePopup(icon, text, 2f);

                // Popup 4
                icon = Resources.Load<Texture2D>("Popup/mouse-click-left-sexybody");
                text = "Last Spell";

                UpdatePopup(icon, text, 2f);
                break;

            case 2:
                // -- Egg Open Portal Tutorial --
                // Chained = true
                // Popup 1
                // Debug.Log($"Level Trigger invoked, hintId = {hintId}");
                icon = Resources.Load<Texture>("Popup/dragon-egg-render");
                text = "Dragon Egg Open Portal";

                UpdatePopup(icon, text, 3f);
                break;

            default:
                // Debug.Log($"Level Trigger invoked, hintId = {hintId}");
                // Debug.Log($"No Icon/Text provided for Trigger hintId = {hintId}");
                break;
        }   
    }
}
