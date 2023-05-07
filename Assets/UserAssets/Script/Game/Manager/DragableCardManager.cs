using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{
    Unit = 0,
    Magic,
    Building
}
public class DragableCardManager : MonoBehaviour
{
    private const int maxUnitCardNum = 4; //���� ������ 4�� ����
    private const int maxMagicCardNum = 2;

    #region Serialize Member
    [Header("Mouse Layer")]
    [Tooltip("���콺�� ������ ���̾�")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Pool")]
    [Tooltip("�п� ��ġ�� ī���� RectTransform")]
    [SerializeField] private RectTransform activeCardPool;

    [Tooltip("�п� �ö󰡱����� ��ġ�� ī���� RectTransform")]
    [SerializeField] private RectTransform readyCardPool;

    [Tooltip("����ī���� RectTransform")]
    [SerializeField] private RectTransform magicCardPool;

    [Tooltip("���콺�� ������ ���� �� ���¿� ��ġ�� Transform")]
    [SerializeField] private Transform previewHolder;

    [Tooltip("�ʵ忡 �ö� ������ ��ġ�� Transform")]
    [SerializeField] private Transform unitPool;

    [Tooltip("����ī�尡 ��ȯ���� �� ��ġ�� Transform")]
    [SerializeField] private Transform magicPool;

    [Tooltip("����ī�尡 ��ȯ���� �� ��ġ�� Transform -> Position")]
    [SerializeField] private Transform magicStartPos;
    #endregion

    private GameManager gameManager;
    private BuildingCardManager deckManager;
    private RectTransform backupCardTransform;
    private Card[] unitCards;
    private Card[] magicCards;

    private Vector2 startPos;

    private bool isActiveCard;
    private bool mIsClickedCard;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        deckManager = GetComponent<BuildingCardManager>();
        unitCards = new Card[maxUnitCardNum];
        magicCards = new Card[maxMagicCardNum];
    }


    private void Start()
    {
        LoadHandCard();
    }

    private void LoadHandCard()
    {
        StartCoroutine(AddUnitCard());
        for (int i = 0; i < maxUnitCardNum; i++)
        {
            StartCoroutine(ObserveCard(i));
            StartCoroutine(AddUnitCard());
        }
    }

    public void LoadMagicCard(MagicCard magicCard)
    {
        StartCoroutine(AddMagicCards(0, magicCard));
    }

    #region �� ����
    /*
    * 
    * %%%%%%%%%%%%%%%%%%%%%%%%% AddCard, CardObserve %%%%%%%%%%%%%%%%%%%%%%%%%%
    * %%%%%%%%%%%%%%%%%%%%%%%%%  delay�߸� �ǵ帮��  %%%%%%%%%%%%%%%%%%%%%%%%%%
    * %%%%%%%%%%%%%%%%%%%%%%%%%       ���� ����      %%%%%%%%%%%%%%%%%%%%%%%%%%
    * 
    * 
    * 
    * 
    *  ������ �ð� 0���� �Ҳ��� AddCard, CardObserve �� �Լ� �ڷ�ƾ ���� ����
    */

    /// <summary>
    /// �з� ������ ī�带 �غ��Ŵ
    /// </summary>
    /// <param name="delay">ī�� ��ο� �ð�</param>
    /// <returns></returns>
    // private IEnumerator AddCard(GameObject obj, float delay = 0f)
    private IEnumerator AddUnitCard(float delay = 0f)
    {
        //Debug.Log("AddCard");

        yield return new WaitForSeconds(delay);

        backupCardTransform = Instantiate(deckManager.GetRandomUnitCard()).GetComponent<RectTransform>();
        backupCardTransform.SetParent(readyCardPool, true);
        backupCardTransform.localScale = Vector3.one * 0.7f;
        backupCardTransform.anchoredPosition = new Vector2(0, 0);

        Card cardScript = backupCardTransform.GetComponent<Card>();
        cardScript.CardType = CardType.Unit;
        cardScript.InitializeData();
    }
    private IEnumerator AddMagicCards(int cardId, MagicCard magicCard, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        RectTransform backupCardTransform = Instantiate(magicCard).GetComponent<RectTransform>();
        backupCardTransform.SetParent(magicCardPool, true);
        backupCardTransform.anchoredPosition = new Vector2(-40 + cardId * 80, 0);

        Card cardScript = backupCardTransform.GetComponent<Card>();
        cardScript.InitializeData();

        cardScript.CardId = cardId;
        magicCards[cardId] = cardScript;
        cardScript.CardType = CardType.Magic;

        cardScript.OnPointerDownBatchAction += CardTapped;
        cardScript.OnDragAction += CardDragged;
        cardScript.OnPointerUpAction += CardReleased;
    }

    /// <summary>
    /// AddCard���� �غ�� ī�带 �з� ������
    /// </summary>
    /// <param name="cardId">0 ~ maxCardNum - 1�� ������ �ĺ���ȣ</param>
    /// <param name="delay">ī�� ��ο� �ð�</param>
    /// <returns></returns>
    private IEnumerator ObserveCard(int cardId, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        Card cardScript = backupCardTransform.GetComponent<Card>();

        backupCardTransform.SetParent(activeCardPool, true);
        backupCardTransform.localScale = Vector3.one;
        backupCardTransform.anchoredPosition = new Vector2(-150 + cardId * 100, 0);
        backupCardTransform.GetComponentInChildren<Text>().text = backupCardTransform.GetComponentInChildren<Text>().text + cardScript.CurrentCardLevel;
        //�ӽ�

        cardScript.CardId = cardId;
        unitCards[cardId] = cardScript;

        cardScript.OnPointerDownBatchAction += CardTapped;
        cardScript.OnDragAction += CardDragged;
        cardScript.OnPointerUpAction += CardReleased;
    }
    #endregion

    #region ���콺 ó�� ���

    private void DefineCardType(int cardId, CardType cardType, out Card usingCard)
    {
        if (cardType == CardType.Unit) usingCard = unitCards[cardId];
        else usingCard = magicCards[cardId];
    }

    /// <summary>
    /// ī�带 ���콺�� Ŭ�� �� �ߵ���
    /// </summary>
    /// <param name="cardId">0 ~ maxCardNum - 1�� ������ �ĺ���ȣ</param>
    private void CardTapped(int cardId, CardType cardType)
    {
        RectTransform card = null;
        if (cardType == CardType.Unit)
        {
            card = unitCards[cardId].GetComponent<RectTransform>();
            GameManager.mMyNexus.DisplaySpawnAbleArea(true);
        }
        else if (cardType == CardType.Magic) card = magicCards[cardId].GetComponent<RectTransform>();

        card.SetAsLastSibling(); //UI�������� �� ���� ���� �ϴ°̴ϴ�
        startPos = card.anchoredPosition;
    }

    /// <summary>
    /// ī�� Ŭ�� �� �巡�� �� �ߵ���
    /// </summary>
    /// <param name="cardId">0 ~ maxCardNum - 1�� ������ �ĺ���ȣ</param>
    /// <param name="dragAmount">eventData.delta��</param>
    private void CardDragged(int cardId, Vector2 dragAmount, CardType cardType)
    {
        DefineCardType(cardId, cardType, out Card usingCard);

        usingCard.transform.Translate(dragAmount);

        if (IsHitToGround(out RaycastHit hit))
        {
            if (!isActiveCard)
            {
                isActiveCard = true;
                previewHolder.position = hit.point;
                usingCard.ChangeActiveState(true);

                Instantiate(usingCard.CardPrefab, hit.point, Quaternion.identity, previewHolder);
            }
            else
            {
                Vector3 cardPos = hit.point;
                if (cardType == CardType.Unit) cardPos = hit.collider.transform.position + Vector3.up * 0.8f;

                previewHolder.position = cardPos;
            }
        }
        else if (isActiveCard)
        {
            isActiveCard = false;
            usingCard.ChangeActiveState(false);

            ClearPreviewObject();
        }

    }

    /// <summary>
    /// ī�带 Ŭ���ϰ� ���� ��� �ߵ���
    /// </summary>
    /// <param name="cardId">0 ~ maxCardNum - 1�� ������ �ĺ���ȣ</param>
    private void CardReleased(int cardId, CardType cardType)
    {
        GameManager.mMyNexus.DisplaySpawnAbleArea(false);
        DefineCardType(cardId, cardType, out Card usingCard);

        if (IsHitToGround(out RaycastHit hit))
        {
            ClearPreviewObject();

            if (!gameManager.DoValidGold(usingCard.CardCost) ||
                (cardType == CardType.Unit && !GameManager.mMyNexus.IsInArea(hit.collider.transform.position)))
            {
                usingCard.GetComponent<RectTransform>().anchoredPosition = startPos;
                usingCard.ChangeActiveState(false);
                return;
            }

            Destroy(usingCard.gameObject);
            SpawnUnit(cardId, cardType, hit, usingCard);
        }
        else usingCard.GetComponent<RectTransform>().anchoredPosition = startPos;
    }

    /// <summary>
    /// ī��κ��� ���� ��ȯ��
    /// </summary>
    /// <param name="cardId">ī���ȣ(RectTransform��ȣ)</param>
    /// <param name="cardType">ī�� ����</param>
    /// <param name="hit">���� Ray����</param>
    /// <param name="usingCard">���� ��ȯ�ϰ��� �ϴ� ī���� Card��ü</param>
    /// <param name="isPlayer">��Ʈ��ũ�� ������ ī������</param>
    public void SpawnUnit(int cardId, CardType cardType, RaycastHit hit, Card usingCard)
    {
        GameObject obj = null;
        if (cardType == CardType.Unit)
        {
            Vector3 cardPos = hit.collider.transform.position + Vector3.up * 0.2f;
            obj = PhotonNetwork.Instantiate("OfficialUnit/" + usingCard.CardPrefab.name, cardPos, Quaternion.identity);

            obj.GetComponent<Unit>().InitBatch();
            obj.transform.SetParent(unitPool);

            StartCoroutine(ObserveCard(cardId));
            StartCoroutine(AddUnitCard());
        }
        else if (cardType == CardType.Magic)
        {
            obj = Instantiate(usingCard.CardPrefab, magicStartPos.position, Quaternion.identity);
            obj.GetComponent<Magic>().Init(hit.point);
            obj.transform.SetParent(magicPool);
        }
    }

    /// <summary>
    /// ���� ��ũ�� ��ǥ���� ���콺 ��ġ���� ���� ��ǥ����� ���̿��� GroundLayer�� �����Ǵ��� �˾Ƴ�
    /// </summary>
    /// <param name="hit">out RaycastHit hit</param>
    /// <returns>Ground ���̾�� �浹 o : true, �浹 x : false</returns>
    private bool IsHitToGround(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer);
    }

    /// <summary>
    /// previewHolder�� �ڽ� ������Ʈ ��� ����
    /// </summary>
    private void ClearPreviewObject()
    {
        for (int i = 0; i < previewHolder.childCount; i++)
            Destroy(previewHolder.GetChild(i).gameObject);
    }
    #endregion    
}
