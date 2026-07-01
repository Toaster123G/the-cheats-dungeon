using UnityEngine;
using System.Collections;

public class CorruptionEffects : MonoBehaviour
{
    public static CorruptionEffects Instance;

    [Header("Fatal Error Block (физический)")]
    public GameObject fatalErrorBlockPrefab;

    [Header("Enemy Spawning (50%)")]
    public GameObject[] enemyPrefabs;
    public Transform player;

    [Header("Screen Flash")]
    public UnityEngine.UI.Image flashOverlay;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 25% — мигание экрана
    public void TriggerWarning()
    {
        StartCoroutine(ScreenFlash(Color.red, 3));
    }

    // 50% — враги падают сверху
    public void TriggerEnemyDrop()
    {
        StartCoroutine(ScreenFlash(Color.red, 2));

        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPos = player.position + new Vector3(Random.Range(-3f, 3f), 8f, 0f);
            GameObject enemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(enemy, spawnPos, Quaternion.identity);
        }
    }

    // 75% — баф врагов
    public void TriggerEnemyBuff()
    {
        StartCoroutine(ScreenFlash(new Color(0.5f, 0f, 0.5f), 5));

        BringerOfDeath[] enemies = FindObjectsByType<BringerOfDeath>(FindObjectsSortMode.None);
        foreach (var e in enemies)
        {
            e.maxHealth *= 2f;
            e.currentHealth = Mathf.Min(e.currentHealth * 2f, e.maxHealth);
            e.chaseSpeed *= 1.5f;
            e.attackDamage *= 1.5f;
        }
    }

    // 100% — блок падает каждые 3 секунды
    public void TriggerFatalError()
    {
        StartCoroutine(FatalErrorSequence());
    }

    IEnumerator FatalErrorSequence()
    {
        yield return new WaitForSeconds(0.1f);

        if (fatalErrorBlockPrefab == null || player == null) yield break;

        // Закрываем чит меню
        CheatMenu cheatMenu = FindFirstObjectByType<CheatMenu>();
        if (cheatMenu != null) cheatMenu.ForceClose();

        while (true)
        {
            HeroKnight hero = FindFirstObjectByType<HeroKnight>();
            if (hero == null) yield break;

            Vector3 spawnPos = hero.transform.position + new Vector3(0f, 2f, 0f);
            Instantiate(fatalErrorBlockPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator ScreenFlash(Color color, int times)
    {
        if (flashOverlay == null) yield break;

        for (int i = 0; i < times; i++)
        {
            flashOverlay.color = new Color(color.r, color.g, color.b, 0.4f);
            yield return new WaitForSeconds(0.1f);
            flashOverlay.color = Color.clear;
            yield return new WaitForSeconds(0.1f);
        }
    }
}