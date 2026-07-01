using UnityEngine;
using System.Collections;

public class HelpPlatformSpawner : MonoBehaviour
{
    public static HelpPlatformSpawner Instance;

    [Header("Платформы (добавляй сколько хочешь)")]
    public GameObject[] platformPrefabs;

    [Header("References")]
    public Transform player;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnPlatform()
    {
        if (platformPrefabs.Length == 0) return;

        // Случайная платформа из массива
        GameObject prefab = platformPrefabs[Random.Range(0, platformPrefabs.Length)];
        Vector3 pos = player.position + new Vector3(2f, -1f, 0f);
        GameObject p = Instantiate(prefab, pos, Quaternion.identity);
        Destroy(p, 20f);
    }
}