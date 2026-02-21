using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI movesText;

    [Header("Combo Display")]
    [SerializeField] private float comboDisplayTime = 1.5f;
    [SerializeField] private Color comboColor = Color.yellow;

    private float comboTimer = 0f;

    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += UpdateScore;
        ScoreManager.OnComboChanged += UpdateCombo;
        ScoreManager.OnMoveMade += UpdateMoves;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= UpdateScore;
        ScoreManager.OnComboChanged -= UpdateCombo;
        ScoreManager.OnMoveMade -= UpdateMoves;
    }

    private void Start()
    {
        if (scoreText != null) scoreText.text = "Score: 0";
        if (movesText != null) movesText.text = "Moves: 0";
        if (comboText != null) comboText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;

            if (comboText != null)
            {
                comboText.transform.localScale = Vector3.Lerp(
                    comboText.transform.localScale,
                    Vector3.one,
                    Time.deltaTime * 5f
                );
            }

            if (comboTimer <= 0f && comboText != null)
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateScore(int score, int combo)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    private void UpdateCombo(int combo)
    {
        if (comboText == null) return;

        if (combo >= 2)
        {
            comboText.gameObject.SetActive(true);
            comboText.text = $"Combo x{combo}!";
            comboText.color = comboColor;
            comboTimer = comboDisplayTime;

          
            comboText.transform.localScale = Vector3.one * 1.3f;
        }
        else
        {
            comboText.gameObject.SetActive(false);
        }
    }

    private void UpdateMoves(int moves)
    {
        if (movesText != null)
            movesText.text = $"Moves: {moves}";
    }
}