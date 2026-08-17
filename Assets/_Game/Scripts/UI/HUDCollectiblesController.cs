using UnityEngine;
using TMPro;

public class HUDCollectiblesController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI eggsCountText;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEggsCount(int count)
    {
        eggsCountText.text = $"{count}";
    }
}
