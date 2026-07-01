using UnityEngine;
using TMPro;

public class UIStats : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI corruptionText;

    private HeroKnight player;

    void Start()
    {
        player = FindFirstObjectByType<HeroKnight>();
    }

    void Update()
    {
        // Переищем если потерялся
        if (player == null)
        {
            player = FindFirstObjectByType<HeroKnight>();
            return;
        }

        if (hpText != null)
            hpText.text = "Heal: " + Mathf.RoundToInt(player.GetHP());

        if (corruptionText != null && CorruptionManager.Instance != null)
            corruptionText.text = "Corruption: " + Mathf.RoundToInt(CorruptionManager.Instance.corruptionLevel) + "%";
    }
}