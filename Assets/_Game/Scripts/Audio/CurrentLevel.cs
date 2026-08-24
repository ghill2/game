using UnityEngine;

public class CurrentLevel : MonoBehaviour
{
    public int currentLevel = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setCurrentLevel();
    }

    public void setCurrentLevel()    
    {

        AudioManager.Instance.PlayEnvironmentStageSwitch(currentLevel);
    }
}
