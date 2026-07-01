using UnityEngine;

public class FallingBanner3x3 : MonoBehaviour
{
    [Header("Падение 3x3")]
    public float fallSpeed = 2.5f;        // скорость падения (меньше = медленнее)
    public float startHeight = 15f;       // высота спавна
    public float destroyHeight = -10f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Поднимаем баннер наверх
        transform.position = new Vector3(transform.position.x, transform.position.y + startHeight, 0);

        rb.linearVelocity = new Vector2(0, -fallSpeed);
    }

    void Update()
    {
        if (transform.position.y < destroyHeight)
            Destroy(gameObject);
    }

    // Игрок может запрыгнуть сверху
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Игрок запрыгнул на баннер 3x3!");
        }
    }
}