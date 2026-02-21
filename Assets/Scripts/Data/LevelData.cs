using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Card Match/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public int rows;
    public int columns;
    public Sprite levelIcon;
}