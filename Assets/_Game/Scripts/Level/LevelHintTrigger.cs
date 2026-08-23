using System;
using UnityEngine;

public class LevelHintTrigger : MonoBehaviour
{
    [SerializeField] private int hintId = 0;

    public event Action<int> OnHintTriggerEntered;

    private void Reset()
    {
        Collider c = GetComponent<Collider>();
        if (c != null) c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        OnHintTriggerEntered?.Invoke(hintId);
    }
}
