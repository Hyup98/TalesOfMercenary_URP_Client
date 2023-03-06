using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingCardManager : MonoBehaviour
{
    private const int maxDeckCardNum = 2;

    #region Serialize Member
    [Header("Prefab or CS")]

    [Tooltip("���ӿ� ������ ���� �ش��ϴ� ī��")] // �ϴ� �� �־�~
    [SerializeField] private BuildingCard[] deckCardPrefab;

    [Tooltip("���ӿ� ������ ���� ī��")]
    [SerializeField] private Card[] magicCard;


    [Header("Pool")]

    [Tooltip("���� ��ġ�� ī���� RectTransform")]
    [SerializeField] private RectTransform activeDeckPool;

    [Tooltip("�ؼ��� ī�尡 ��ġ�� RectTransform")]
    [SerializeField] private RectTransform nexusCardPool;

    #endregion
    private UnitJsonData[] unitJsonDatas;
    private GameManager gameManager;
    private BuildingCard nexusCard;
    private BuildingCard[] deckCards;

    /// <summary>
    /// ������ �ִ� ������ ���� Ȯ���� ���� ī���� ���� ī�带 ��ȯ��
    /// </summary>
    /// <returns>Card</returns>
    public Card GetRandomUnitCard()
    {
        int rand = Random.Range(1, deckCards.Length);
        return deckCards[rand].GetRandomCard();
    }

    /// <summary>
    /// ������ �ִ� ����ī�忡�� ���� Ȯ���� ����ī�带 ��ȯ��
    /// </summary>
    /// <returns></returns>
    public Card GetRandromMagicCard()
    {
        int rand = Random.Range(0, magicCard.Length);
        return magicCard[rand];
    }

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        deckCards = new BuildingCard[maxDeckCardNum + 1];
        //���� ���� ī�� + �ؼ��� ī��
    }

    private void Start()
    {
        LoadDeckInfo();
        for (int i = 0; i < maxDeckCardNum + 1; i++) LoadDeck(i);

        nexusCard = deckCards[0];
    }

    #region �� ����
    private void LoadDeckInfo()
    {
        unitJsonDatas = gameManager.GetDeckInfo();
    }

    /// <summary>
    /// ���� �ѹ� ���� �ε�
    /// </summary>
    /// <param name="cardId">0 ~ maxDeckCardNum - 1�� ������ �ĺ���ȣ</param>
    private void LoadDeck(int cardId)
    {
        //Debug.Log("LoadDeck");
        //DeckCard usingDeckCard = cardId == 0 ? deckCardPrefab[0] : deckCardPrefab[unitJsonDatas[cardId - 1].unitID + 1];
        BuildingCard usingDeckCard = cardId == 0 ? deckCardPrefab[0] : deckCardPrefab[cardId % 2 + 1];

        RectTransform deckCardTransform = Instantiate(usingDeckCard).GetComponent<RectTransform>();

        RectTransform parentTransform = cardId == 0 ? nexusCardPool : activeDeckPool;
        deckCardTransform.SetParent(parentTransform, true);
        deckCardTransform.anchoredPosition = new Vector2(cardId == 0 ? 0 : (-50 + (cardId - 1) * 100), 0);

        deckCardTransform.TryGetComponent(out BuildingCard deckCard);

        deckCard.CardId = cardId;
        deckCard.CardLevel = 1;
        deckCard.CardMaxLevel = 3;
        deckCard.OnPointerDownAction += PromoteDeckCard;
        deckCards[cardId] = deckCard;
    }

    /// <summary>
    /// �� ī�带 ���׷��̵� ��
    /// </summary>
    /// <param name="cardId">0 ~ maxDeckCardNum - 1�� ������ �ĺ���ȣ</param>
    private void PromoteDeckCard(int cardId)
    {
        //Debug.Log("PromoteDeckCard" + cardId);

        if (deckCards[cardId].CardLevel >= deckCards[cardId].CardMaxLevel) return;
        if (!gameManager.DoValidGold(deckCards[cardId].CardUpgradeCost)) return;
        if (cardId == 0)
        {
            gameManager.UpgradeNexus(0.05f);
            NetworkUnitManager.SendNeuxsUpgrade();
        }
        else NetworkUnitManager.SendBuildingUpgrade(deckCards[cardId].CardUniqueNumber);
        deckCards[cardId].CardLevel += 1;
        Text cardText = deckCards[cardId].GetComponentInChildren<Text>();
        cardText.text = deckCards[cardId].CardName + "\n" + deckCards[cardId].CardLevel.ToString();
    }
    #endregion
}
