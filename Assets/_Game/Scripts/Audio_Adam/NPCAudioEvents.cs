using UnityEngine;

public enum npcType
{ 
    None = 0,
    Skeleton = 1,
    Wolf = 2
}

public enum charAction
{ 
    Step = 0,
    Attack = 1,
    Hurt = 2,
    Death = 3,
    FireBall = 4,
    Ice = 5,
    Lightning = 6
}


[DisallowMultipleComponent]
public class NPCAudioEvents : MonoBehaviour
{
        

    [SerializeField] public npcType npcType;

    private void Awake()
    {

    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    public void PlayAttackSFX()
    {
        AudioManager.Instance?.NPCActionResolver(npcType, charAction.Attack, transform.position);
    }
    public void PlayDeathSFX()
    {
        AudioManager.Instance?.NPCActionResolver(npcType, charAction.Death, transform.position);
    }

}

