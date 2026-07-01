using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxLayer : MonoBehaviour
{
    [Header("ѕараллакс")]
    [Range(0f, 1f)] public float parallaxFactorX = 0.5f;   // горизонтальный
    [Range(0f, 1f)] public float parallaxFactorY = 0f;     // вертикальный (обычно 0)

    [Header("Ѕесконечный повтор")]
    public bool infiniteScrolling = true;

    private Vector3 startPosition;
    private float spriteWidth;
    private Camera mainCam;
    private Transform camTransform;

    void Start()
    {
        mainCam = Camera.main;
        camTransform = mainCam.transform;

        startPosition = transform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;
    }

    void LateUpdate()  // LateUpdate Ч важно, чтобы после движени€ камеры
    {
        Vector3 camPos = camTransform.position;

        // ѕараллакс движение
        float distanceX = camPos.x * parallaxFactorX;
        float distanceY = camPos.y * parallaxFactorY;

        transform.position = new Vector3(
            startPosition.x + distanceX,
            startPosition.y + distanceY,
            transform.position.z
        );

        // Ѕесконечный скроллинг
        if (infiniteScrolling)
        {
            float temp = (camPos.x * (1 - parallaxFactorX));
            if (temp > startPosition.x + spriteWidth)
            {
                startPosition.x += spriteWidth;
            }
            else if (temp < startPosition.x - spriteWidth)
            {
                startPosition.x -= spriteWidth;
            }
        }
    }
}