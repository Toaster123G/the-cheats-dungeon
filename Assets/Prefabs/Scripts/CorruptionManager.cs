using UnityEngine;
using System.Collections;

public class CorruptionManager : MonoBehaviour
{
    public static CorruptionManager Instance;

    [Range(0, 100)]
    public float corruptionLevel = 0f;
    private float corruptionStep = 25f;

    public delegate void CorruptionChanged(float level);
    public static event CorruptionChanged OnCorruptionChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddCorruption()
    {
        corruptionLevel = Mathf.Min(corruptionLevel + corruptionStep, 100f);
        OnCorruptionChanged?.Invoke(corruptionLevel);
        HandleCorruptionLevel();
    }

    void HandleCorruptionLevel()
    {
        if (corruptionLevel >= 100f)
        {
            CorruptionEffects.Instance.TriggerFatalError();
        }
        else if (corruptionLevel >= 75f)
        {
            CorruptionEffects.Instance.TriggerEnemyBuff();
        }
        else if (corruptionLevel >= 50f)
        {
            CorruptionEffects.Instance.TriggerEnemyDrop();
        }
        else if (corruptionLevel >= 25f)
        {
            CorruptionEffects.Instance.TriggerWarning();
        }
    }
}