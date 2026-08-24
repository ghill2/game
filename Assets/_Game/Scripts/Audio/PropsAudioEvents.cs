using UnityEngine;


public enum PropsType
{
    None = 0,
    orb = 1,
    windmill = 2,
    smoke = 3,
    portal  = 4
}

[DisallowMultipleComponent]
public class PropsAudioEvents : MonoBehaviour
{

    [SerializeField] PropsType propsType = 0;

    private AudioSource audioSource;
    

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null ) audioSource = gameObject.AddComponent<AudioSource>();
        
    }

    private void Start()
    {
        AudioManager.Instance?.PropsSFXResolver(propsType, audioSource);
    }

    private void OnEnable()
    {
        AudioManager.Instance?.PropsSFXResolver(propsType, audioSource);
    }
}

