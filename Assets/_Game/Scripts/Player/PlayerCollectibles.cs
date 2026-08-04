using UnityEngine;
using System;

public class PlayerCollectibles : MonoBehaviour
{
    [SerializeField]
    private int eggsCollected = 0;

    public event Action<int> OnEggCollected;

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
        OnEggCollected?.Invoke(eggsCollected);
        Debug.Log($"Egg collected! Total eggs: {eggsCollected}");
    }
}
