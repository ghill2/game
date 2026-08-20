using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject bossPortal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (bossPortal == null)
        {
            Debug.LogError("Boss portal is not set.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has entered the trigger
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the boss trigger. Activating boss portal.");
            if (bossPortal != null && !bossPortal.activeSelf)
            {
                bossPortal.SetActive(true);
            }
        }
    }
}
