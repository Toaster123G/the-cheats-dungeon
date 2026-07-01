using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Звуки Игрока")]
    public AudioClip playerJump;
    public AudioClip playerAttack;
    public AudioClip playerHurt;
    public AudioClip playerDeath;
    public AudioClip playerRoll;

    [Header("Звуки Bringer of Death")]
    public AudioClip bringerAttack;
    public AudioClip bringerCast;
    public AudioClip bringerHurt;
    public AudioClip bringerDeath;
    public AudioClip bringerTeleport;

    [Header("Звуки Wizard")]
    public AudioClip wizardAttack;
    public AudioClip wizardHurt;
    public AudioClip wizardDeath;

    private AudioSource source;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        source = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
            source.PlayOneShot(clip, volume);
    }
}