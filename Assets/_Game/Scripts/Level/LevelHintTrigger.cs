using System;
using UnityEngine;

public class LevelHintTrigger : MonoBehaviour
{
    [SerializeField] private int hintId = 0;

    public event Action<int> OnHintTriggerEntered;

    private bool pendingHint;

    private void Reset()
    {
        Collider c = GetComponent<Collider>();
        if (c != null) c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Keep the hint while the additive UI scene is loading.
        pendingHint = true;
    }
    private void LateUpdate()
    {
        Action<int> listeners = OnHintTriggerEntered;
        if (!pendingHint || listeners == null)
            return;

        // UI Start methods must finish before the popup receives the hint.
        pendingHint = false;
        listeners.Invoke(hintId);
    }
}
