using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private Transform levelButtonParent;
    [SerializeField] private Button levelButtonPrefab;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button continueButton;

    private void Start()
    {
        CreateLevelButtons();

        if (menuButton != null)
            menuButton.onClick.AddListener(ShowMenu);

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(HideMenu);
            // Only show continue if there's a save
            continueButton.gameObject.SetActive(
                SaveManager.Instance != null && SaveManager.Instance.HasSave()
            );
        }

        // Start with menu hidden if there's a save (game auto-loads)
        if (SaveManager.Instance != null && SaveManager.Instance.HasSave())
        {
            HideMenu();
        }
        else
        {
            ShowMenu();
        }
    }

    private void CreateLevelButtons()
    {
        if (levelManager == null || levelButtonPrefab == null) return;

        LevelData[] levels = levelManager.Levels;

        for (int i = 0; i < levels.Length; i++)
        {
            int index = i; // Capture for closure
            LevelData level = levels[i];

            Button button = Instantiate(levelButtonPrefab, levelButtonParent);

            // Set button text
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = $"{level.levelName}\n{level.rows}x{level.columns}";
            }

            button.onClick.AddListener(() =>
            {
                levelManager.SelectLevel(index);
                HideMenu();
            });
        }
    }

    public void ShowMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);

        // Update continue button
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(
                SaveManager.Instance != null && SaveManager.Instance.HasSave()
            );
        }
    }

    public void HideMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
    }
}