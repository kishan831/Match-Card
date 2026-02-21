using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int baseMatchScore = 100;
    [SerializeField] private int mismatchPenalty = 10;
    [SerializeField] private float comboMultiplierStep = 0.5f;
    [SerializeField] private int maxCombo = 10;

    private int currentScore = 0;
    private int currentCombo = 0;
    private int totalMoves = 0;
    private int bestCombo = 0;

    // Properties
    public int CurrentScore => currentScore;
    public int CurrentCombo => currentCombo;
    public int TotalMoves => totalMoves;
    public int BestCombo => bestCombo;

    // Events
    public static event System.Action<int, int> OnScoreChanged;    
    public static event System.Action<int> OnComboChanged;         
    public static event System.Action<int> OnMoveMade;              

    private void OnEnable()
    {
        GameManager.OnCardsMatched += HandleMatch;
        GameManager.OnCardsMismatched += HandleMismatch;
    }

    private void OnDisable()
    {
        GameManager.OnCardsMatched -= HandleMatch;
        GameManager.OnCardsMismatched -= HandleMismatch;
    }

    private void HandleMatch(Card cardA, Card cardB)
    {
        totalMoves++;
        currentCombo++;

        if (currentCombo > bestCombo)
            bestCombo = currentCombo;

        float multiplier = 1f + (Mathf.Min(currentCombo, maxCombo) - 1) * comboMultiplierStep;
        int scoreGain = Mathf.RoundToInt(baseMatchScore * multiplier);
        currentScore += scoreGain;

        Debug.Log($"Match! +{scoreGain} pts (Combo x{currentCombo}, Multiplier: {multiplier:F1}) | Total: {currentScore}");

        OnScoreChanged?.Invoke(currentScore, currentCombo);
        OnComboChanged?.Invoke(currentCombo);
        OnMoveMade?.Invoke(totalMoves);
    }

    private void HandleMismatch(Card cardA, Card cardB)
    {
        totalMoves++;

        if (currentCombo > 0)
        {
            Debug.Log($"Combo broken! Was {currentCombo}x");
        }
        currentCombo = 0;

        currentScore = Mathf.Max(0, currentScore - mismatchPenalty);

        Debug.Log($"Mismatch! -{mismatchPenalty} pts | Total: {currentScore}");

        OnScoreChanged?.Invoke(currentScore, currentCombo);
        OnComboChanged?.Invoke(currentCombo);
        OnMoveMade?.Invoke(totalMoves);
    }

    public void ResetScore()
    {
        currentScore = 0;
        currentCombo = 0;
        totalMoves = 0;
        bestCombo = 0;

        OnScoreChanged?.Invoke(currentScore, currentCombo);
        OnComboChanged?.Invoke(currentCombo);
        OnMoveMade?.Invoke(totalMoves);
    }

    public ScoreSaveData GetSaveData()
    {
        return new ScoreSaveData
        {
            score = currentScore,
            combo = currentCombo,
            moves = totalMoves,
            bestCombo = bestCombo
        };
    }

    public void LoadSaveData(ScoreSaveData data)
    {
        currentScore = data.score;
        currentCombo = data.combo;
        totalMoves = data.moves;
        bestCombo = data.bestCombo;

        OnScoreChanged?.Invoke(currentScore, currentCombo);
        OnComboChanged?.Invoke(currentCombo);
        OnMoveMade?.Invoke(totalMoves);
    }
}

[System.Serializable]
public class ScoreSaveData
{
    public int score;
    public int combo;
    public int moves;
    public int bestCombo;
}