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

    public int CardId { get; set; } // 인덱스 번호(배열)
    public int CardCurrentLevel { get => cardCurrentLevel; set => cardCurrentLevel = value; }
    public int CardMaxLevel { get => cardMaxLevel; }
    public int CardUniqueNumber { get => cardUniqueNumber; } // 고유번호
    public string CardName { get; set; }
   
    private void Awake()
    {
        CardName = gameObject.name;
        mPhotonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// 업그레이드시 미리 골드 가격을 확인하고 사용
    /// </summary>
    public void BuildingUpgarde()
    {
        cardCurrentLevel++;
        mPhotonView.RPC(nameof(BuildingUpgardeRPC), RpcTarget.Others);
    }

    [PunRPC]
    public void BuildingUpgardeRPC()
    {
        cardCurrentLevel++;
    }

    //적절한 위치에 함수 호춢 추가요함
    public void Init()
    {
        mPhotonView.RPC(nameof(InitRPC), RpcTarget.Others);
    }

    [PunRPC]
    public void InitRPC()
    {
        NetworkUnitManager.enemyBuildingList.Add(this);
    }


    /// <summary>
    /// 가지고 있는 카드를 레벨별 일정 확률로 반환함
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
