using System.Collections.Generic;
using UnityEngine;

public class CardPool : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform poolParent;
    [SerializeField] private int initialPoolSize = 30;

    private Queue<Card> pool = new Queue<Card>();

    private void Awake()
    {
        PreWarm();
    }

    private void PreWarm()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            Card card = CreateNewCard();
            card.gameObject.SetActive(false);
            pool.Enqueue(card);
        }
    }

    public Card GetCard()
    {
        Card card;

        if (pool.Count > 0)
        {
            card = pool.Dequeue();
            card.gameObject.SetActive(true);
        }
        else
        {
            card = CreateNewCard();
        }

        return card;
    }

    public void ReturnCard(Card card)
    {
        card.gameObject.SetActive(false);
        card.transform.SetParent(poolParent);
        pool.Enqueue(card);
    }

    public void ReturnAll(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            if (card != null)
                ReturnCard(card);
        }
        cards.Clear();
    }

    private Card CreateNewCard()
    {
        Card card = Instantiate(cardPrefab, poolParent);
        return card;
    }
}