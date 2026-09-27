using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour
{
    [Header("=== LOSE / GAME OVER SCREEN BUTTONS ===")]
    public Button loseRestartButton;
    public Button loseMainMenuButton;

    [Header("=== WIN SCREEN BUTTONS ===")]
    public Button winRestartButton;
    public Button winMainMenuButton;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();

        // Hook up Lose Screen buttons
        if (loseRestartButton != null)
            loseRestartButton.onClick.AddListener(RestartGame);
        
        if (loseMainMenuButton != null)
            loseMainMenuButton.onClick.AddListener(GoToMainMenu);

        // Hook up Win Screen buttons
        if (winRestartButton != null)
            winRestartButton.onClick.AddListener(RestartGame);
        
        if (winMainMenuButton != null)
            winMainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        if (playerHealth != null)
            playerHealth.RestartGame();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }
}