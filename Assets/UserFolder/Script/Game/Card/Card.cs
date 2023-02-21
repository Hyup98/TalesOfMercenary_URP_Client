using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum CardType
{
    Unit = 0,
    Magic = 1
}
public class Card : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private CanvasGroup canvasGroup;

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int cardCost;
    [SerializeField] private CardType cardType;
    public UnityAction<int, Vector2, CardType> OnDragAction { get; set; }
    public UnityAction<int, CardType> OnPointerDownAction { get; set; }
    public UnityAction<int, CardType> OnPointerUpAction { get; set; }

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



    private void Awake() => canvasGroup = GetComponent<CanvasGroup>();

    public void InitializeData()
    {
        //������
    }

    void IDragHandler.OnDrag(PointerEventData eventData) =>
        OnDragAction?.Invoke(CardId, eventData.delta, cardType);

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData) =>
        OnPointerUpAction?.Invoke(CardId, cardType);

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) =>
        OnPointerDownAction?.Invoke(CardId, cardType);

    /// <summary>
    /// �𵨶Ǵ� UI�� ����
    /// </summary>
    /// <param name="isActive">true : UI����, false : �ش� �� ����</param>
    public void ChangeActiveState(bool isActive) =>
        canvasGroup.alpha = (isActive) ? 0 : 1;

}
