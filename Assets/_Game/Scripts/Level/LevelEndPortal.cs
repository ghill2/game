using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndPortal : MonoBehaviour
{
    [SerializeField]
    private string targetSceneName = "SCN_BETA_Level_1";


    private BoxCollider portalCollider;
    private bool transitionStarted;


    private void Start()
    {
        portalCollider = GetComponent<BoxCollider>();

        if (portalCollider == null)
        {
            Debug.LogError("No BoxCollider found on the portal object.", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (transitionStarted || !other.CompareTag("Player"))
        {
            return;
        }

        transitionStarted = true;

        if (portalCollider != null)
        {
            portalCollider.enabled = false;
        }

        Debug.Log("Player entered the portal. Loading scene: " + targetSceneName);

        if (AudioManager.Instance == null ||
            !AudioManager.Instance.PlayPortalTransition(targetSceneName))
        {
            
            Debug.LogWarning(
                "No available AudioManager was found.",
                this);
            
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
