using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    public int rows;
    public int columns;
    public List<CardSaveData> cards;
    public ScoreSaveData scoreData;
    public bool isGameActive;
}

[System.Serializable]
public class CardSaveData
{
    public int cardId;
    public int cardDataIndex; 
    public bool isMatched;
    public bool isFlipped;
    public int gridIndex;
}