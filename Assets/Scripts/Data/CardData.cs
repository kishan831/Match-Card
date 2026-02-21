using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card Match/Card Data")]
public class CardData : ScriptableObject
{
    public int cardId;
    public Sprite cardFace;
}