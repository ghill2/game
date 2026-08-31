using UnityEngine;
using System;
using UnityEngine.UI;

// types of collectibles the player can pick up; drives dispatch in CollectibleController
public enum Collectible
{
    DragonEgg,
    SpeedBoots,
    HealthPotion
}

// raised when an egg is picked up
public class EggCollectedEventArgs : EventArgs
{
    public int Eggs;      // running total of eggs collected
    public Texture Icon;  // popup icon
    public string Text;   // popup text

    public EggCollectedEventArgs(int eggs, Texture icon, string text)
    {
        Eggs = eggs;
        Icon = icon;
        Text = text;
    }
}

// raised when the speed boots are picked up
public class BootsCollectedEventArgs : EventArgs
{
    public float Multiplier;  // speed multiplier applied by the boost
    public float Duration;    // boost duration in seconds

    public BootsCollectedEventArgs(float multiplier, float duration)
    {
        Multiplier = multiplier;
        Duration = duration;
    }
}

// raised when a health potion is picked up
public class PotionCollectedEventArgs : EventArgs
{
    public int HealAmount;  // how much health was restored

    public PotionCollectedEventArgs(int healAmount)
    {
        HealAmount = healAmount;
    }
}

public class PlayerCollectibles : MonoBehaviour
{
    [SerializeField]
    private int eggsCollected = 0;
    [SerializeField]
    private Texture icon;

    public event EventHandler<EggCollectedEventArgs> OnEggCollected;
    public event EventHandler<BootsCollectedEventArgs> OnBootsCollected;
    public event EventHandler<PotionCollectedEventArgs> OnPotionCollected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EggCollect()
    {
        eggsCollected++;
        EggCollectedEventArgs args = new(eggsCollected, icon, "Collected");
        OnEggCollected?.Invoke(this, args);
        Debug.Log($"Egg collected! Total eggs: {eggsCollected}");
    }

    public void BootsCollect(float multiplier, float duration)
    {
        BootsCollectedEventArgs args = new(multiplier, duration);
        OnBootsCollected?.Invoke(this, args);
        Debug.Log($"Speed boots collected! x{multiplier} speed for {duration} seconds");
    }

    public void PotionCollect(int healAmount)
    {
        PotionCollectedEventArgs args = new(healAmount);
        OnPotionCollected?.Invoke(this, args);
        Debug.Log($"Health potion collected! Restored {healAmount} health");
    }
}
