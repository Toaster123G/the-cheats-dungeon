using UnityEngine;

public class FatalErrorBlock : MonoBehaviour
{
    public float damage = 99999f;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<HeroKnight>(out var hero))
            hero.TakeDamage(damage);
    }
}