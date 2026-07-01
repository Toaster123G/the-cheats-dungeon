using UnityEngine;

public class GoalPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<HeroKnight>(out _))
        {
            if (GameManager.instance != null)
                GameManager.instance.LevelCleared();

            Destroy(gameObject);
        }
    }
}