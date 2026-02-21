using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private CardPool cardPool;

    [Header("Grid Settings")]
    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 4;

    [Header("Display Area")]
    [SerializeField] private float targetWidth = 8f;
    [SerializeField] private float targetHeight = 6f;
    [SerializeField] private float cardPadding = 0.1f;

    [Header("Card Data")]
    [SerializeField] private CardData[] cardDataArray;

    private List<Card> activeCards = new List<Card>();

    public List<Card> ActiveCards => activeCards;
    public int Rows => rows;
    public int Columns => columns;

    public void SetGridSize(int newRows, int newColumns)
    {
        rows = newRows;
        columns = newColumns;
    }


    public void GenerateGridFromSave(GameSaveData saveData)
    {
        ClearGrid();

        rows = saveData.rows;
        columns = saveData.columns;

        float cardWidth = (targetWidth - (columns + 1) * cardPadding) / columns;
        float cardHeight = (targetHeight - (rows + 1) * cardPadding) / rows;
        float cardSize = Mathf.Min(cardWidth, cardHeight);

        float gridWidth = columns * cardSize + (columns + 1) * cardPadding;
        float gridHeight = rows * cardSize + (rows + 1) * cardPadding;
        float startX = -gridWidth / 2f + cardPadding + cardSize / 2f;
        float startY = gridHeight / 2f - cardPadding - cardSize / 2f;

        for (int i = 0; i < saveData.cards.Count; i++)
        {
            CardSaveData cardSave = saveData.cards[i];

            int row = i / columns;
            int col = i % columns;

            float xPos = startX + col * (cardSize + cardPadding);
            float yPos = startY - row * (cardSize + cardPadding);

            // Use the saved index to get the EXACT CardData
            int dataIndex = cardSave.cardDataIndex;
            if (dataIndex < 0 || dataIndex >= cardDataArray.Length)
            {
                Debug.LogError($"Invalid cardDataIndex: {dataIndex}");
                continue;
            }

            CardData data = cardDataArray[dataIndex];

            Card card = (cardPool != null) ? cardPool.GetCard() : Instantiate(cardPrefab, gridParent);
            card.transform.SetParent(gridParent);
            card.transform.localPosition = new Vector3(xPos, yPos, 0f);
            card.transform.localScale = Vector3.one * cardSize;
            card.Initialize(data, dataIndex);

            // Restore matched state
            if (cardSave.isMatched)
            {
                card.SetMatched();
                card.SetFlippedInstant();
               // card.FlipUp();
            }

            activeCards.Add(card);
        }
    }

    public void GenerateGrid()
    {
        ClearGrid();

        int totalCards = rows * columns;

        if (totalCards % 2 != 0)
        {
            Debug.LogError("Total cards must be even! Adjust rows/columns.");
            return;
        }

        int pairsNeeded = totalCards / 2;

        // Build pair list with index tracking
        List<int> cardDataIndices = new List<int>();
        for (int i = 0; i < pairsNeeded; i++)
        {
            int dataIndex = i % cardDataArray.Length;
            cardDataIndices.Add(dataIndex);
            cardDataIndices.Add(dataIndex);
        }

        // Shuffle
        ShuffleList(cardDataIndices);

        // Calculate card size to fit display area
        float cardWidth = (targetWidth - (columns + 1) * cardPadding) / columns;
        float cardHeight = (targetHeight - (rows + 1) * cardPadding) / rows;
        float cardSize = Mathf.Min(cardWidth, cardHeight);

        float gridWidth = columns * cardSize + (columns + 1) * cardPadding;
        float gridHeight = rows * cardSize + (rows + 1) * cardPadding;
        float startX = -gridWidth / 2f + cardPadding + cardSize / 2f;
        float startY = gridHeight / 2f - cardPadding - cardSize / 2f;

        int index = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float xPos = startX + col * (cardSize + cardPadding);
                float yPos = startY - row * (cardSize + cardPadding);

                int dataIndex = cardDataIndices[index];
                CardData data = cardDataArray[dataIndex];

                Card card = (cardPool != null) ? cardPool.GetCard() : Instantiate(cardPrefab, gridParent);
                card.transform.SetParent(gridParent);
                card.transform.localPosition = new Vector3(xPos, yPos, 0f);
                card.transform.localScale = Vector3.one * cardSize;
                card.Initialize(data, dataIndex);
                activeCards.Add(card);

                index++;
            }
        }
    }

    public void ClearGrid()
    {
        if (cardPool != null)
        {
            cardPool.ReturnAll(activeCards);
        }
        else
        {
            foreach (Card card in activeCards)
            {
                if (card != null)
                    Destroy(card.gameObject);
            }
            activeCards.Clear();
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}