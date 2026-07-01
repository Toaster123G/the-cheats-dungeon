using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Image hpImage;
    public HeroKnight player;

    [Header("5 спрайтов по убыванию HP")]
    public Sprite hp100; // полный
    public Sprite hp75;  // 75%
    public Sprite hp50;  // 50%
    public Sprite hp25;  // 25%
    public Sprite hp0;   // мёртвый

    void Update()
    {
        if (player == null || hpImage == null) return;

        float ratio = player.GetHP() / player.GetMaxHP();

        if (ratio > 0.75f)
            hpImage.sprite = hp100;
        else if (ratio > 0.50f)
            hpImage.sprite = hp75;
        else if (ratio > 0.25f)
            hpImage.sprite = hp50;
        else if (ratio > 0f)
            hpImage.sprite = hp25;
        else
            hpImage.sprite = hp0;
    }
}