using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Photon.Pun;
using Scriptable;

public class BuildingCard : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Card[] cards;
    [SerializeField] private int cardUpgradCost;
    [SerializeField] private int cardUniqueNumber;
    [SerializeField] private int cardMaxLevel;
    private PhotonView mPhotonView;


    private string mID { get; }
    private int cardCurrentLevel;

    public UnityAction<int> OnPointerDownAction { get; set; }

    public int CardUpgradeCost
    {
        get => cardUpgradCost;
        private set => cardUpgradCost = value;
    }

    public int CardId { get; set; } // �ε��� ��ȣ(�迭)
    public int CardCurrentLevel { get => cardCurrentLevel; set => cardCurrentLevel = value; }
    public int CardMaxLevel { get => cardMaxLevel; }
    public int CardUniqueNumber { get => cardUniqueNumber; } // ������ȣ
    public string CardName { get; set; }
   
    public void BuildingUpgarde()
    {
        cardCurrentLevel++;
    }

    private void Awake()
    {
        CardName = gameObject.name;
        mPhotonView = GetComponent<PhotonView>();
    }

    public void Init()
    {
        NetworkUnitManager.mybuildingList.Add(this);
        mPhotonView.RPC(nameof(InitRPC), RpcTarget.Others);
    }

    [PunRPC]
    public void InitRPC()
    {
        NetworkUnitManager.enemyBuildingList.Add(this);
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
