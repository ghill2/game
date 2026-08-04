using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed = 15f;
    
    [SerializeField]
    private float lifetime = 5f;

    [SerializeField]
    private int damage = 30;

    private Rigidbody body;

    public Vector3 Position => body.position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody>();

        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        Vector3 movement =
            transform.forward * speed * Time.fixedDeltaTime;

        body.MovePosition(body.position + movement);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Get IDamageable component from the other object
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        
        Destroy(gameObject);
    }
}
