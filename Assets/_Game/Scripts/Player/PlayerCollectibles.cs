using UnityEngine;
using System;
using UnityEngine.UI;

public class CollectiblesEventArgs : EventArgs
{
    // Arguments
    public int Eggs;
    public Texture Icon;
    public string Text;

    // Constructor
    public CollectiblesEventArgs(int eggs, Texture icon, string text)
    {
        Eggs = eggs;
        Icon = icon;
        Text = text;
    }
}

public class PlayerCollectibles : MonoBehaviour
{
    [SerializeField]
    private int eggsCollected = 0;
    [SerializeField]
    private Texture icon;

    //public event Action<int> OnEggCollected;
    public event EventHandler<CollectiblesEventArgs> OnEggCollected;
    
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
        CollectiblesEventArgs args = new(eggsCollected, icon, "Collected!");
        OnEggCollected?.Invoke(this, args);
        Debug.Log($"Egg collected! Total eggs: {eggsCollected}");
    }
}
