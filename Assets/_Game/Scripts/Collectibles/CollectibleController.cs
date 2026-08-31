using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    [SerializeField]
    private Collectible type = Collectible.DragonEgg; // which collectible this is; drives what happens on pickup

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Character>();

        if (player != null)
        {
            if (type == Collectible.SpeedBoots) {
                player.CollectBoots();
            } else if (type == Collectible.HealthPotion) {
                player.CollectHealthPotion();
            } else {
                player.CollectEgg();
            }
            Debug.Log($"Player collected a collectible with type: {type}");
            Destroy(gameObject);
        }
    }
}
