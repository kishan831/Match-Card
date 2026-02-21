using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string SAVE_KEY = "CardMatchSave";

    public static SaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SaveGame(GridManager gridManager, ScoreManager scoreManager, bool isGameActive)
    {
        GameSaveData saveData = new GameSaveData();

        saveData.rows = gridManager.Rows;
        saveData.columns = gridManager.Columns;
        saveData.isGameActive = isGameActive;

        saveData.cards = new List<CardSaveData>();
        List<Card> cards = gridManager.ActiveCards;
        for (int i = 0; i < cards.Count; i++)
        {
            CardSaveData cardSave = new CardSaveData
            {
                cardId = cards[i].CardId,
                cardDataIndex = cards[i].CardDataIndex,
                isMatched = cards[i].IsMatched,
                isFlipped = cards[i].IsMatched,
                gridIndex = i
            };
            saveData.cards.Add(cardSave);
        }

        saveData.scoreData = scoreManager.GetSaveData();

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("Game Saved!");
    }

    public GameSaveData LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("No save data found.");
            return null;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        Debug.Log("Game Loaded!");
        return saveData;
    }

    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        Debug.Log("Save deleted!");
    }
}