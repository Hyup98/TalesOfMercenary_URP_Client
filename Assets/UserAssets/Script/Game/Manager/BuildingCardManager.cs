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
    [Tooltip("�� �ǹ�ī�尡 ��ġ�� RectTransform")]
    [SerializeField] private RectTransform mMyBuildingCardPool;

    [Tooltip("�� �ǹ�ī�尡 ��ġ�� RectTransform")]
    [SerializeField] private RectTransform mEnemyBuildingCardPool;

    [Tooltip("�ؼ��� ī�尡 ��ġ�� RectTransform")]
    [SerializeField] private RectTransform nexusCardPool;

    [Header("CardProbability")]
    [SerializeField] private CardProbability[] mCardProbability;
    #endregion

    private const int mMaxBuildingCardNum = 2;
    private const float mTotal = 100;

    private PhotonView mPhotonView;
    private GameManager gameManager;
    private BuildingCard nexusCard;
    private BuildingCard[] mBuildingCards;
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
        int rand = Random.Range(1, mBuildingCards.Length);
        int level = mBuildingCards[rand].CardCurrentLevel - 1;
        return mBuildingCards[rand].GetCard(GetCardProbability(rand, level));
    }

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        mPhotonView = GetComponent<PhotonView>();
        mBuildingCards = new BuildingCard[mMaxBuildingCardNum + 1];
        //���� ���� ī�� + �ؼ��� ī��
    }

    private void Start()
    {
        LoadNexusCard();
        for (int i = 1; i < mMaxBuildingCardNum + 1; i++) LoadBuildingCard(i);//mPhotonView.RPC(nameof(LoadBuildingCard),RpcTarget.All, i);

        nexusCard = mBuildingCards[0];
    }
    #region �� ����

    private void LoadNexusCard()
    {
        BuildingCard usingBuildingCard = deckCardPrefab[0];

        RectTransform deckCardTransform = Instantiate(usingBuildingCard).GetComponent<RectTransform>();

        deckCardTransform.SetParent(nexusCardPool, true);
        deckCardTransform.TryGetComponent(out BuildingCard buildingCard);

        buildingCard.CardId = 0;
        buildingCard.CardCurrentLevel = 1;
        buildingCard.OnPointerDownAction += PromoteBuildingCard;
        mBuildingCards[0] = buildingCard;
    }

    /// <summary>
    /// ���� �ѹ� �ǹ��� �ε�
    /// </summary>
    /// <param name="cardId">0 ~ maxDeckCardNum - 1�� ������ �ĺ���ȣ</param>
    private void LoadBuildingCard(int cardId)
    {
        //Debug.Log("LoadDeck");
        //DeckCard usingDeckCard = cardId == 0 ? deckCardPrefab[0] : deckCardPrefab[unitJsonDatas[cardId - 1].unitID + 1];

        BuildingCard usingBuildingCard = deckCardPrefab[cardId];

        RectTransform deckCardTransform = Instantiate(usingBuildingCard).GetComponent<RectTransform>();
        mPhotonView.RPC(nameof(LoadEnemyBuildingCard), RpcTarget.Others, cardId);

        deckCardTransform.SetParent(mMyBuildingCardPool, true);
        deckCardTransform.TryGetComponent(out BuildingCard buildingCard);

        buildingCard.CardId = cardId;
        buildingCard.CardCurrentLevel = 1;
        buildingCard.OnPointerDownAction += PromoteBuildingCard;
        mBuildingCards[cardId] = buildingCard;
    }

    [PunRPC]
    private void LoadEnemyBuildingCard(int cardId)
    {
        BuildingCard usingBuildingCard = deckCardPrefab[cardId];
        RectTransform deckCardTransform = Instantiate(usingBuildingCard).GetComponent<RectTransform>();

        deckCardTransform.SetParent(mEnemyBuildingCardPool, true);
    }

    /// <summary>
    /// �ǹ��� ���׷��̵� ��
    /// </summary>
    /// <param name="cardId">0 ~ maxDeckCardNum - 1�� ������ �ĺ���ȣ</param>
    private void PromoteBuildingCard(int cardId)
    {
        //Debug.Log("PromoteDeckCard" + cardId);
        if (mBuildingCards[cardId].CardCurrentLevel >= mBuildingCards[cardId].CardMaxLevel) return;
        if (!gameManager.DoValidGold(mBuildingCards[cardId].CardUpgradeCost)) return;
        //if (cardId == 0)
        //{
        //    gameManager.UpgradeNexus(0.05f);
        //    NetworkUnitManager.SendNeuxsUpgrade();
        //}
        //else NetworkUnitManager.SendBuildingUpgrade(deckCards[cardId].CardUniqueNumber);
        mBuildingCards[cardId].CardCurrentLevel += 1;
        Text cardText = mBuildingCards[cardId].GetComponentInChildren<Text>();
        cardText.text = mBuildingCards[cardId].CardName + "\n" + mBuildingCards[cardId].CardCurrentLevel.ToString();
    }
    #endregion
}