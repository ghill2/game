using UnityEngine;

public class CollectibleController : MonoBehaviour
{
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
            player.CollectEgg();
            Debug.Log("Player collected an egg!");
            Destroy(gameObject);
        }
    }
}
