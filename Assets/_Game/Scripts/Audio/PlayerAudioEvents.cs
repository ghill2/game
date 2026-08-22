using UnityEngine;


[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerCollectibles))]
public sealed class PlayerAudioEvents : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerCollectibles playerCollectibles;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerCollectibles = GetComponent<PlayerCollectibles>();
    }

    private void OnEnable()
    {
        playerHealth.Damaged += HandleDamaged;
        playerCollectibles.OnEggCollected += HandleEggCollected;
    }

    private void OnDisable()
    {
        playerHealth.Damaged -= HandleDamaged;
        playerCollectibles.OnEggCollected -= HandleEggCollected;
    }

    public void PlayFireballCast()
    {
        //AudioManager.Instance?.PlayFireballCast();
        AudioManager.Instance?.PlayerActionResolver(charAction.FireBall, transform.position);
    }

    public void Step1()
    {
        AudioManager.Instance?.PlayStep1();
    }

    public void Step2()
    {
        AudioManager.Instance?.PlayStep2();
    }

    private void HandleDamaged(int amount)
    {
        Debug.Log($"Playing ouch sound.");
        AudioManager.Instance?.PlayRandomOuch();
    }

    private void HandleEggCollected(object sender, CollectiblesEventArgs eventArgs)
    {
        AudioManager.Instance?.PlayCollected();
    }
}
