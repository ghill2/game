using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class AdditiveSceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName = "SCN_UI";

    private void Start()
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return;

        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (scene.IsValid() && scene.isLoaded)
            return;

        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }
}
