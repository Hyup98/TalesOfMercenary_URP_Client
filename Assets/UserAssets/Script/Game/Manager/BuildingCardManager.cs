using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CardProbability
{
    [SerializeField] private float[] probability;

    public float[] ProbabilityArray { get => probability; }
    public float this[int index] { get => probability[index]; }
}

public class BuildingCardManager : MonoBehaviour
{
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

    [Header("ī�� Ȯ��")]
    [SerializeField] private CardProbability[] mCardProbability;
    #endregion

    private const int maxDeckCardNum = 2;
    private const float mTotal = 100;

    private GameManager gameManager;
    private BuildingCard nexusCard;
    private BuildingCard[] deckCards;
    //deckCards 0�� index = NexusCard

    private int GetCardProbability(int rand, int level)
    {
        float randomPoint = Random.value * mTotal;
        int length = mCardProbability[level].ProbabilityArray.Length;
        for (int i = 0; i < length; i++)
        {
            if (randomPoint < mCardProbability[level][i]) return i;
            else randomPoint -= mCardProbability[level][i];
        }
        return length - 1;
    }

    /// <summary>
    /// ������ �ִ� ������ ���� Ȯ���� ���� ī���� ���� ī�带 ��ȯ��
    /// </summary>
    /// <returns>Card</returns>
    public Card GetRandomUnitCard()
    {
        int rand = Random.Range(1, deckCards.Length);
        int level = deckCards[rand].CardCurrentLevel - 1;
        Debug.Log(deckCards[rand].GetCard(GetCardProbability(rand, level)));
        return deckCards[rand].GetCard(GetCardProbability(rand, level));


        //for (int i = 0; i < mCardProbability.Length; i++)
        //{
        //    for(int j = 0; j < mCardProbability[i].ProbabilityArray.Length; j++)
        //    {
        //        Debug.Log(mCardProbability[i][j]);
        //    }
        //    Debug.Log("-----------------------------------");
        //}

        //return deckCards[rand].GetRandomCard();
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
        for (int i = 0; i < maxDeckCardNum + 1; i++) LoadDeck(i);

        nexusCard = deckCards[0];
    }
    #region �� ����

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

        deckCardTransform.TryGetComponent(out BuildingCard buildingCard);

        buildingCard.CardId = cardId;
        buildingCard.CardCurrentLevel = 1;
        buildingCard.OnPointerDownAction += PromoteDeckCard;
        deckCards[cardId] = buildingCard;
    }

    /// <summary>
    /// �� ī�带 ���׷��̵� ��
    /// </summary>
    /// <param name="cardId">0 ~ maxDeckCardNum - 1�� ������ �ĺ���ȣ</param>
    private void PromoteDeckCard(int cardId)
    {
        //Debug.Log("PromoteDeckCard" + cardId);

        if (deckCards[cardId].CardCurrentLevel >= deckCards[cardId].CardMaxLevel) return;
        if (!gameManager.DoValidGold(deckCards[cardId].CardUpgradeCost)) return;
        //if (cardId == 0)
        //{
        //    gameManager.UpgradeNexus(0.05f);
        //    NetworkUnitManager.SendNeuxsUpgrade();
        //}
        //else NetworkUnitManager.SendBuildingUpgrade(deckCards[cardId].CardUniqueNumber);
        deckCards[cardId].CardCurrentLevel += 1;
        Text cardText = deckCards[cardId].GetComponentInChildren<Text>();
        cardText.text = deckCards[cardId].CardName + "\n" + deckCards[cardId].CardCurrentLevel.ToString();
    }
    #endregion
}
