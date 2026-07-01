using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Header("References:")]
    public GameObject player = null;

    [Header("Scores")]
    [SerializeField] private int gameManagerScore = 0;

    public static int score
    {
        get { return instance.gameManagerScore; }
        set { instance.gameManagerScore = value; }
    }

    public int highScore = 0;

    [Header("Game Progress / Victory Settings")]
    public bool gameIsWinnable = true;
    public int gameVictoryPageIndex = 0;
    public GameObject victoryEffect;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        // Ищем HeroKnight вместо PlayerController
        if (player == null)
        {
            HeroKnight hero = FindFirstObjectByType<HeroKnight>();
            if (hero != null)
                player = hero.gameObject;
            else if (SceneManager.GetActiveScene().name != "MainMenu")
                Debug.Log("HeroKnight не найден в сцене.");
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("highscore"))
            highScore = PlayerPrefs.GetInt("highscore");

        if (PlayerPrefs.HasKey("score"))
            score = PlayerPrefs.GetInt("score");
    }

    private void OnApplicationQuit()
    {
        SaveHighScore();
        ResetScore();
    }

    public static void UpdateUIElements()
    {
        if (UIManager.instance != null)
            UIManager.instance.UpdateUI();
    }

    public void LevelCleared()
    {
        PlayerPrefs.SetInt("score", score);

        if (UIManager.instance != null)
        {
            if (player != null) player.SetActive(false);
            UIManager.instance.allowPause = false;
            UIManager.instance.GoToPage(gameVictoryPageIndex);

            if (victoryEffect != null)
                Instantiate(victoryEffect, transform.position, transform.rotation, null);
        }
    }

    [Header("Game Over Settings:")]
    public int gameOverPageIndex = 0;
    public GameObject gameOverEffect;

    [HideInInspector]
    public bool gameIsOver = false;

    public void GameOver()
    {
        gameIsOver = true;

        if (gameOverEffect != null)
            Instantiate(gameOverEffect, transform.position, transform.rotation, null);

        if (UIManager.instance != null)
        {
            UIManager.instance.allowPause = false;
            UIManager.instance.GoToPage(gameOverPageIndex);
        }
    }

    public static void AddScore(int scoreAmount)
    {
        score += scoreAmount;
        if (score > instance.highScore)
            SaveHighScore();
        UpdateUIElements();
    }

    public static void ResetScore()
    {
        PlayerPrefs.SetInt("score", 0);
        score = 0;
    }

    public static void ResetGamePlayerPrefs()
    {
        PlayerPrefs.SetInt("score", 0);
        score = 0;
    }

    public static void SaveHighScore()
    {
        if (score > instance.highScore)
        {
            PlayerPrefs.SetInt("highscore", score);
            instance.highScore = score;
        }
        UpdateUIElements();
    }

    public static void ResetHighScore()
    {
        PlayerPrefs.SetInt("highscore", 0);
        if (instance != null)
            instance.highScore = 0;
        UpdateUIElements();
    }
}