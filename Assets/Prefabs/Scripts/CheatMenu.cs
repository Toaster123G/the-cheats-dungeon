using UnityEngine;

public class CheatMenu : MonoBehaviour
{
    public GameObject cheatPanel;
    private HeroKnight player;

    void Start()
    {
        player = FindFirstObjectByType<HeroKnight>();

        if (cheatPanel == null)
            Debug.LogError("CheatMenu: cheatPanel не подключён в Inspector!");

        if (player == null)
            Debug.LogError("CheatMenu: HeroKnight не найден!");

        cheatPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (CorruptionManager.Instance == null)
            {
                Debug.LogError("CorruptionManager не найден!");
                return;
            }

            if (CorruptionManager.Instance.corruptionLevel >= 100f) return;

            cheatPanel.SetActive(!cheatPanel.activeSelf);
            Debug.Log("CheatMenu: " + (cheatPanel.activeSelf ? "открыт" : "закрыт"));
        }
    }

    public void OnMorePower()
    {
        if (player == null) return;
        player.AddDamage(5);
        CorruptionManager.Instance.AddCorruption();
        Debug.Log("[CHEAT] More Power");
    }

    public void OnMoreSpeed()
    {
        if (player == null) return;
        player.AddSpeed(1f);
        CorruptionManager.Instance.AddCorruption();
        Debug.Log("[CHEAT] More Speed");
    }

    public void OnPowerJump()
    {
        if (player == null) return;
        player.AddJump(2f);
        CorruptionManager.Instance.AddCorruption();
        Debug.Log("[CHEAT] Power Jump");
    }

    public void OnNeedHelp()
    {
        if (HelpPlatformSpawner.Instance == null)
        {
            Debug.LogError("HelpPlatformSpawner не найден!");
            return;
        }
        HelpPlatformSpawner.Instance.SpawnPlatform();
        Debug.Log("[CHEAT] Need Help");
    }

    public void ForceClose()
    {
        if (cheatPanel != null)
            cheatPanel.SetActive(false);
    }

    public void OnClose()
    {
        if (cheatPanel != null)
            cheatPanel.SetActive(false);
    }
}