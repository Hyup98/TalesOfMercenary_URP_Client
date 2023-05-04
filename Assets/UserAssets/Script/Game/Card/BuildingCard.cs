using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class BuildingCard : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Card[] cards;
    [SerializeField] private int cardUpgradCost;
    [SerializeField] private int cardUniqueNumber;
    [SerializeField] private int cardMaxLevel;
    private string mID { get; }
    private int cardCurrentLevel;

    public UnityAction<int> OnPointerDownAction { get; set; }

    public int CardUpgradeCost
    {
        get => cardUpgradCost;
        private set => cardUpgradCost = value;
    }

    public int CardId { get; set; }
    public int CardCurrentLevel { get => cardCurrentLevel; set => cardCurrentLevel = value; }
    public int CardMaxLevel { get => cardMaxLevel; }
    public int CardUniqueNumber { get => cardUniqueNumber; }
    public string CardName { get; set; }
   
    public void BuildingUpgarde()
    {
        cardCurrentLevel++;
    }

    private void Awake()
    {
        CardName = gameObject.name;
    }

    /// <summary>
    /// ������ �ִ� ī�带 ������ ���� Ȯ���� ��ȯ��
    /// </summary>
    /// <returns>Card</returns>
    public Card GetRandomCard()
    {
        int rand = Random.Range(0, cards.Length);
        return cards[rand];
    }

    public Card GetCard(int index) => cards[index];

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) =>
        OnPointerDownAction?.Invoke(CardId);
}
