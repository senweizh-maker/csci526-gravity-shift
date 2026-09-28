using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("UI")]
    public GameObject startPanel;
    public GameObject gameHUD;
    public GameObject winPanel;
    public GameObject nextLevelButton;

    [Header("Player")]
    public PlayerGravityController playerController;
    public Rigidbody2D playerRigidbody;

    private bool gameStarted = false;
    private bool levelCompleted = false;
    private bool restarting = false;

    // 静态变量：Scene重新加载以后仍然保留
    // 用于判断这次加载是不是Restart导致的
    private static bool restartIntoGame = false;

    private void Awake()
    {
        Instance = this;

        // ==========================
        // 情况1：R / 死亡 / Play Again
        // ==========================
        if (restartIntoGame)
        {
            restartIntoGame = false;

            gameStarted = true;
            levelCompleted = false;
            restarting = false;

            Time.timeScale = 1f;

            if (startPanel != null)
                startPanel.SetActive(false);

            if (gameHUD != null)
                gameHUD.SetActive(true);

            if (winPanel != null)
                winPanel.SetActive(false);

            return;
        }

        // ==========================
        // 情况2：第一次打开游戏
        // ==========================

        gameStarted = false;
        levelCompleted = false;
        restarting = false;

        Time.timeScale = 0f;

        if (startPanel != null)
            startPanel.SetActive(true);

        if (gameHUD != null)
            gameHUD.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            // 开始界面按R不做任何事情
            // 游戏中或者Win界面才允许Restart
            if (gameStarted || levelCompleted)
            {
                RestartLevel();
            }
        }
    }

    public void StartGame()
    {
        gameStarted = true;

        if (startPanel != null)
            startPanel.SetActive(false);

        if (gameHUD != null)
            gameHUD.SetActive(true);

        if (winPanel != null)
            winPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void CompleteLevel()
    {
        if (!gameStarted ||
            levelCompleted ||
            restarting)
        {
            return;
        }

        levelCompleted = true;

        Time.timeScale = 0f;

        if (nextLevelButton != null)
        {
            bool hasNext =
                SceneManager.GetActiveScene().buildIndex + 1
                < SceneManager.sceneCountInBuildSettings;

            nextLevelButton.SetActive(hasNext);
        }

        if (gameHUD != null)
            gameHUD.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(true);
    }

    public void PlayerDied()
    {
        if (restarting || levelCompleted)
            return;

        restarting = true;

        if (playerController != null)
            playerController.enabled = false;

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.simulated = false;
        }

        if (playerController != null)
            playerController.gameObject.SetActive(false);

        StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0.4f);

        RestartLevel();
    }


    public void LoadNextLevel()
    {
        // Show the start screen on the new level instead of skipping it
        restartIntoGame = false;
        Time.timeScale = 1f;

        int next = SceneManager.GetActiveScene().buildIndex + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene(0);
    }

    public void RestartLevel()
    {
        // 告诉下一次 Awake：
        // 不显示 Start Screen，直接开始游戏
        restartIntoGame = true;

        Time.timeScale = 1f;

        Scene currentScene =
            SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.name);
    }
}