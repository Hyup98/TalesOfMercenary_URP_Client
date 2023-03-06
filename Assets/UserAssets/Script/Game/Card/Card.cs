using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class Card : MonoBehaviour
{
    protected CanvasGroup canvasGroup;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int cardCost;
    [SerializeField] public CardType cardType;
    public UnityAction<int, Vector2, CardType> OnDragAction { get; set; }
    public UnityAction<int, CardType> OnPointerDownAction { get; set; }
    public UnityAction<int, CardType> OnPointerUpAction { get; set; }

    protected void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public int CardCost
    {
        get => cardCost;
        private set => cardCost = value;
    }
    public GameObject CardPrefab
    {
        get => cardPrefab;
        private set => cardPrefab = value;
    }

    public CardType CardType
    {
        get => cardType;
        set => cardType = value;
    }

    public int CardId { get; set; }

    public virtual void InitializeData()
    {
        //������
    }

    /// <summary>
    /// �𵨶Ǵ� UI�� ����
    /// </summary>
    /// <param name="isActive">true : UI����, false : �ش� �� ����</param>
    public void ChangeActiveState(bool isActive) =>
        canvasGroup.alpha = (isActive) ? 0 : 1;
}
