using UnityEngine;
using UnityEngine.UI;

public class HP_Fill : MonoBehaviour
{
    [SerializeField, Range(0,1)] float HP_percentage = 0.6f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.fillAmount = HP_percentage;
            //image.fillAmount = currentHP / MaxHP; // Logic wise.
        }
    }
}
