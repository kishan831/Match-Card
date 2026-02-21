using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalMovesText;
    [SerializeField] private TextMeshProUGUI bestComboText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Manager References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private MenuUI menuUI;

    private void OnEnable()
    {
        GameManager.OnGameOver += ShowGameOver;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= ShowGameOver;
    }

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClicked);
    }

    private void ShowGameOver()
    {
        if (gameOverPanel == null) return;

        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {scoreManager.CurrentScore}";
            if (finalMovesText != null)
                finalMovesText.text = $"Total Moves: {scoreManager.TotalMoves}";
            if (bestComboText != null)
                bestComboText.text = $"Best Combo: {scoreManager.BestCombo}x";
        }

        gameOverPanel.SetActive(true);
    }

    private void OnRestartClicked()
    {
        gameOverPanel.SetActive(false);
        if (gameManager != null)
            gameManager.StartNewGame();
    }

    private void OnMenuClicked()
    {
        gameOverPanel.SetActive(false);
        if (menuUI != null)
            menuUI.ShowMenu();
    }
}