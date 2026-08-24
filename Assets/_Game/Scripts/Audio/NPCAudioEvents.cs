using UnityEngine;

public enum NpcType
{ 
    None = 0,
    Skeleton = 1,
    Wolf = 2
}

public enum CharAction
{ 
    Step = 0,
    Attack = 1,
    Hurt = 2,
    Death = 3,
    FireBall = 4,
    Ice = 5,
    Lightning = 6,
    CollectEgg =7,
    CollectGeneric=8

}


//[DisallowMultipleComponent]
//public class NPCAudioEvents : MonoBehaviour
//{
        

//    [SerializeField] public NpcType npcType = 0;

//    private AudioSource audioSource;
//    private void Awake()
//    {
//        audioSource = GetComponent<AudioSource>();
//        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
//    }
//    public void PlayAttackSFX()
//    {
//        AudioManager.Instance?.NPCActionResolver(npcType, CharAction.Attack, audioSource);
//    }

//    public void PlayHurtSFX()
//    {
//        AudioManager.Instance?.NPCActionResolver(npcType, CharAction.Hurt, audioSource);
//    }

//    public void PlayDeathSFX()
//    {
//        AudioManager.Instance?.NPCActionResolver(npcType, CharAction.Death, audioSource);
//    }

//}

