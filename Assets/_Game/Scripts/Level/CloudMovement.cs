using System.IO;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    [Header("Cloud Movement Setting")]
    public float speedX = 0.002f;
    public float speedY = 0.002f;

    private Renderer rend;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        rend.material.mainTextureOffset += new Vector2(speedX * Time.deltaTime, speedY * Time.deltaTime);
    }
}
