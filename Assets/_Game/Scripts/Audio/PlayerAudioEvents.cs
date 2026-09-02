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
        playerCollectibles.OnBootsCollected += HandleBootCollected;
        playerCollectibles.OnPotionCollected += HandlePotionCollected;
    }

    private void OnDisable()
    {
        playerHealth.Damaged -= HandleDamaged;
        playerCollectibles.OnEggCollected -= HandleEggCollected;
        playerCollectibles.OnBootsCollected -= HandleBootCollected;
        playerCollectibles.OnPotionCollected -= HandlePotionCollected;
    }

    public void PlayFireballCast()
    {
        //AudioManager.Instance?.PlayFireballCast();
        //AudioManager.Instance?.PlayerActionResolver(CharAction.FireBall, audioSource);
    }

    public void Step1()
    {
        //AudioManager.Instance?.PlayStep1();
        AudioManager.Instance?.PlayerActionResolver(CharAction.Step);
    }

    public void Step2()
    {
        AudioManager.Instance?.PlayerActionResolver(CharAction.Step);
    }

    private void HandleDamaged(int amount)
    {
        //AudioManager.Instance?.PlayRandomOuch();
        AudioManager.Instance?.PlayerActionResolver(CharAction.Hurt);
    }

    private void HandleEggCollected(object sender, EggCollectedEventArgs eventArgs)
    {
        //AudioManager.Instance?.PlayCollected();
        AudioManager.Instance?.PlayerActionResolver(CharAction.CollectEgg);
    }

    private void HandleBootCollected(object sender, BootsCollectedEventArgs eventArgs)
    {
        //AudioManager.Instance?.PlayCollected();
        AudioManager.Instance?.PlayerActionResolver(CharAction.CollectBoot);
    }

    private void HandlePotionCollected(object sender, PotionCollectedEventArgs eventArgs)
    {
        //AudioManager.Instance?.PlayCollected();
        AudioManager.Instance?.PlayerActionResolver(CharAction.CollectPotion);
    }
}


