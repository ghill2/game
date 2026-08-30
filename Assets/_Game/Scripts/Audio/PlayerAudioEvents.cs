using UnityEngine;


[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerCollectibles))]
public sealed class PlayerAudioEvents : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerCollectibles playerCollectibles;

    private AudioSource audioSource;


    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerCollectibles = GetComponent<PlayerCollectibles>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        AudioManager.Instance.playerSource = audioSource;
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
        //AudioManager.Instance?.PlayerActionResolver(CharAction.FireBall, audioSource);
    }

    public void Step1()
    {
        //AudioManager.Instance?.PlayStep1();
        AudioManager.Instance?.PlayerActionResolver(CharAction.Step, audioSource);
    }

    public void Step2()
    {
        AudioManager.Instance?.PlayerActionResolver(CharAction.Step, audioSource);
    }

    private void HandleDamaged(int amount)
    {
        //AudioManager.Instance?.PlayRandomOuch();
        AudioManager.Instance?.PlayerActionResolver(CharAction.Hurt, audioSource);
    }

    private void HandleEggCollected(object sender, CollectiblesEventArgs eventArgs)
    {
        //AudioManager.Instance?.PlayCollected();
        AudioManager.Instance?.PlayerActionResolver(CharAction.CollectEgg, audioSource);
    }
}


