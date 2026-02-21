using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Available Levels")]
    [SerializeField] private LevelData[] levels;

    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameManager gameManager;

    private int currentLevelIndex = 0;

    public LevelData[] Levels => levels;
    public int CurrentLevelIndex => currentLevelIndex;
    public LevelData CurrentLevel => levels[currentLevelIndex];

    public static event System.Action<LevelData> OnLevelSelected;

    public void SelectLevel(int index)
    {
        if (index < 0 || index >= levels.Length)
        {
            Debug.LogError($"Invalid level index: {index}");
            return;
        }

        currentLevelIndex = index;
        LevelData level = levels[index];

        gridManager.SetGridSize(level.rows, level.columns);
        gameManager.StartNewGame();

        OnLevelSelected?.Invoke(level);

        Debug.Log($"Level selected: {level.levelName} ({level.rows}x{level.columns})");
    }
}