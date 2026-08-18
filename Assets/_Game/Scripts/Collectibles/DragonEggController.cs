using UnityEngine;

public class DragonEggController : MonoBehaviour
{
    [SerializeField]
    private GameObject portal;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Character>();
        
        if (player != null)
        {
            player.CollectEgg();
            Debug.Log("Player collected the Dragon Egg!");

            if (portal != null && !portal.activeSelf)
            {
                portal.SetActive(true);
                Debug.Log("Portal activated!");
            }
            
            Destroy(gameObject);
        }
    }
}
