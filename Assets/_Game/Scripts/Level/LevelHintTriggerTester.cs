using UnityEngine;

public class LevelHintTriggerTester : MonoBehaviour
{
    [SerializeField] private LevelHintTrigger trigger;

    private void Awake()
    {
        trigger.OnHintTriggerEntered += HandleHint;
    }

    private void HandleHint(int id)
    {
        Debug.Log("Hint Trigger Entered! ID = " + id);
    }
}
