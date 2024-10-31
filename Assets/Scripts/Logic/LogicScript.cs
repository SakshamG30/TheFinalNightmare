using UnityEngine.SceneManagement;
using UnityEngine;

public class LogicScript : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject pauseScreen;
    private bool isPaused = false;
    private bool isGameOver = false; // New flag for game over state
    private GameObject player;
    private float timer = 0f;
    AstroScript astro;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        astro = player.GetComponent<AstroScript>();
        //pauseScreen = GameObject.Find("PauseScreen");
        //gameOverScreen = GameObject.Find("GameOver");
    }

    void Update()
    {
        if (!isPaused && !isGameOver) // Timer only runs if not paused or game over
        {
            timer += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver) // Can only pause if not game over
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeScreen();
        }
        else
        {
            PauseScreen();
        }
    }

    public void RestartGame()
    {
        float cumulativeTime = PlayerPrefs.GetFloat("TotalGameTime", 0f);

        // Add the current level's timer to the cumulative total
        cumulativeTime += timer;

        // Store the updated cumulative time back in PlayerPrefs
        PlayerPrefs.SetFloat("TotalGameTime", cumulativeTime);
        PlayerPrefs.Save();
        isGameOver = false;
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        if (player != null)
        {
            player.SetActive(false);
        }

        isGameOver = true;
        PauseGame(); // Ensures time is stopped and disables player interactions
        gameOverScreen.SetActive(true);
        ShowCanvasGroup(gameOverScreen);
    }

    public void PauseScreen()
    {
        astro.cutSceneEnabled = true;
        isPaused = true;
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        ShowCanvasGroup(pauseScreen);
    }

    public void ResumeScreen()
    {
        astro.cutSceneEnabled = false;
        isPaused = false;
        Time.timeScale = 1.0f;
        pauseScreen.SetActive(false);
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    private void ShowCanvasGroup(GameObject screen)
    {
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
    public void LoadExit()
    {
        SceneManager.LoadScene("EndScene");
    }

    public void LoadNextLevel(string sceneName)
    {
        // Retrieve cumulative game time from PlayerPrefs
        float cumulativeTime = PlayerPrefs.GetFloat("TotalGameTime", 0f);

        // Add the current level's timer to the cumulative total
        cumulativeTime += timer;

        // Store the updated cumulative time back in PlayerPrefs
        PlayerPrefs.SetFloat("TotalGameTime", cumulativeTime);
        PlayerPrefs.Save();

        // Reset the timer for the next level
        timer = 0f;

        // Load the specified next scene
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
