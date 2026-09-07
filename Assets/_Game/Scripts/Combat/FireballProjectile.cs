using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed = 15f;
    
    [SerializeField]
    private float lifetime = 5f;

    [SerializeField]
    private int damage = 30;

    [SerializeField]
    private ParticleSystem impactEffectPrefab;

    private Rigidbody body;
    private bool didHit;

    public Vector3 Position => body.position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody>();

        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (didHit)
        {
            return;
        }

        Vector3 movement =
            transform.forward * speed * Time.fixedDeltaTime;

        body.MovePosition(body.position + movement);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (didHit)
        {
            return;
        }

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null && damageable.IsAlive)
        {
            damageable.TakeDamage(damage);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
        {
            return;
        }

        didHit = true;

        if (impactEffectPrefab != null)
        {
            Vector3 impactPosition = body != null ? body.position : transform.position;
            Instantiate(impactEffectPrefab, impactPosition, Quaternion.identity).Play();
        }

        Vector3 hitPosition = other.ClosestPoint(transform.position);
        AudioManager.Instance?.PlayFireballHit(hitPosition);
        Destroy(gameObject);
    }
}
