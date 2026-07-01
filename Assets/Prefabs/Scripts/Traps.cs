using UnityEngine;

public class Spikes : MonoBehaviour
{
    [Header("Damage")]
    public float damage = 20f;
    public float damageCooldown = 0.5f; // урон каждые 0.5 секунды

    private float damageTimer = 0f;

    void Update()
    {
        damageTimer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HeroKnight hero = other.GetComponent<HeroKnight>();
            if (hero != null)
            {
                hero.TakeDamage(damage);
                damageTimer = 0f;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && damageTimer >= damageCooldown)
        {
            HeroKnight hero = other.GetComponent<HeroKnight>();
            if (hero != null)
            {
                hero.TakeDamage(damage);
                damageTimer = 0f;
            }
        }
    }
}