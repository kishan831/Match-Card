using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private SaveManager saveManager;

    [Header("Settings")]
    [SerializeField] private float mismatchShowTime = 0.8f;
    [SerializeField] private float autoSaveInterval = 30f;

    // Card tracking
    private List<Card> flippedCards = new List<Card>();
    private int matchedPairs = 0;
    private int totalPairs = 0;
    private bool isGameActive = false;

    // Events
    public static event System.Action<Card, Card> OnCardsMatched;
    public static event System.Action<Card, Card> OnCardsMismatched;
    public static event System.Action OnGameOver;
    public static event System.Action<Card> OnCardFlipped;

    private void OnEnable()
    {
        Card.OnCardClicked += HandleCardClicked;
    }

    private void OnDisable()
    {
        Card.OnCardClicked -= HandleCardClicked;
    }

    private void Start()
    {
        // Try to load existing save, otherwise start new game
        if (saveManager != null && saveManager.HasSave())
        {
            LoadGame();
        }
        else
        {
            StartNewGame();
        }

        // Start auto-save
        StartCoroutine(AutoSaveRoutine());
    }

    public void StartNewGame()
    {
        matchedPairs = 0;
        flippedCards.Clear();

        if (scoreManager != null)
            scoreManager.ResetScore();

        gridManager.GenerateGrid();
        totalPairs = (gridManager.Rows * gridManager.Columns) / 2;
        isGameActive = true;

        // Delete old save
        if (saveManager != null)
            saveManager.DeleteSave();

        Debug.Log($"New game started! {totalPairs} pairs to match.");
    }

    private void LoadGame()
    {
        GameSaveData saveData = saveManager.LoadGame();
        if (saveData == null)
        {
            StartNewGame();
            return;
        }

        flippedCards.Clear();

        // Restore grid
        gridManager.GenerateGridFromSave(saveData);

        // Restore score
        if (scoreManager != null && saveData.scoreData != null)
            scoreManager.LoadSaveData(saveData.scoreData);

        // Count already matched pairs
        totalPairs = (saveData.rows * saveData.columns) / 2;
        matchedPairs = 0;
        foreach (CardSaveData card in saveData.cards)
        {
            if (card.isMatched) matchedPairs++;
        }
        matchedPairs /= 2; // Each pair has 2 cards

        isGameActive = saveData.isGameActive;

        Debug.Log($"Game loaded! {matchedPairs}/{totalPairs} pairs matched.");
    }

    public void SaveGame()
    {
        if (saveManager == null) return;
        saveManager.SaveGame(gridManager, scoreManager, isGameActive);
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            if (isGameActive)
            {
                SaveGame();
            }
        }
    }

    private void HandleCardClicked(Card card)
    {
        if (!isGameActive) return;
        if (card.IsMatched || card.IsFlipped || card.IsFlipping) return;

        if (flippedCards.Count >= 2)
        {
            FlipDownUnmatched();
        }

        card.FlipUp();
        flippedCards.Add(card);
        OnCardFlipped?.Invoke(card);

        if (flippedCards.Count == 2)
        {
            CheckForMatch();
        }
    }

    private void CheckForMatch()
    {
        Card cardA = flippedCards[0];
        Card cardB = flippedCards[1];

        if (cardA.CardId == cardB.CardId)
        {
            cardA.SetMatched();
            cardB.SetMatched();

            // Play match effects
            cardA.PlayMatchEffect();
            cardB.PlayMatchEffect();

            flippedCards.Clear();
            matchedPairs++;

            OnCardsMatched?.Invoke(cardA, cardB);

            SaveGame();

            if (matchedPairs >= totalPairs)
            {
                isGameActive = false;
                OnGameOver?.Invoke();

                if (saveManager != null)
                    saveManager.DeleteSave();

                Debug.Log("Game Over! All pairs matched!");
            }
        }
        else
        {
            // Play mismatch effects
            cardA.PlayMismatchEffect();
            cardB.PlayMismatchEffect();

            StartCoroutine(MismatchTimeout());
            OnCardsMismatched?.Invoke(cardA, cardB);
        }
    }

    private IEnumerator MismatchTimeout()
    {
        yield return new WaitForSeconds(mismatchShowTime);

        if (flippedCards.Count >= 2)
        {
            FlipDownUnmatched();
        }
    }

    private void FlipDownUnmatched()
    {
        for (int i = flippedCards.Count - 1; i >= 0; i--)
        {
            if (!flippedCards[i].IsMatched)
            {
                flippedCards[i].FlipDown();
            }
        }
        flippedCards.Clear();
    }

    // Save when app is paused or quit
    private void OnApplicationPause(bool pause)
    {
        if (pause && isGameActive)
            SaveGame();
    }

    private void OnApplicationQuit()
    {
        if (isGameActive)
            SaveGame();
    }
}