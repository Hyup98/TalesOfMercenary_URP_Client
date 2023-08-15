using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Scriptable;

//todo

//

public class PickManager : MonoBehaviour
{
    [SerializeField] private RectTransform mMyPickingBuilding;
    [SerializeField] private RectTransform mEnemyPickingBuilding;
    [SerializeField] private RectTransform mAllBuildingCard;
    [SerializeField] private BuildingCardManager mBuildingCardManager; //ī�� ���� �켱 4�� �ٲ����

    private PhotonView mPhotonView;
    private BuildingCard [] mBuildingCards;

    private int[] mMySelectingCardIndex;
    private int[] mEnemySelectingCardIndex;

    private int mEnemySelectingCount;
    private int mMySelectingCount;

    private bool mMyPickingComp = false;
    private bool mEnemyPickingComp = false;


    private void Awake()
    {
        mPhotonView = GetComponent<PhotonView>();
        mBuildingCards = new BuildingCard[mAllBuildingCard.childCount];
        mMySelectingCardIndex = new int[mBuildingCardManager.MaxBuildingCardNum];
        mEnemySelectingCardIndex = new int[mBuildingCardManager.MaxBuildingCardNum];
        int i = 0;
        foreach (Transform child in mAllBuildingCard)
        {
            mBuildingCards[i] = child.GetComponent<BuildingCard>();
            mBuildingCards[i].OnPointerDownAction += PickingBuildingCard;
            i++;
        }
    }

    /// <summary>
    /// �ǹ� ī�� UI Ŭ�� �� �ߵ�
    /// </summary>
    /// <param name="cardUniqueNumber">� �ǹ� ī������ �����ϴ� �ε���</param>
    private void PickingBuildingCard(int cardUniqueNumber)
    {
        if (mMyPickingComp) return;

        mMySelectingCardIndex[mMySelectingCount++] = cardUniqueNumber;

        BuildingCard buildingCard = Instantiate(mBuildingCards[cardUniqueNumber]);
        buildingCard.transform.SetParent(mMyPickingBuilding);
        mPhotonView.RPC(nameof(PickingEnemyBuildingCard), RpcTarget.OthersBuffered, cardUniqueNumber);

        if (mMySelectingCount == mBuildingCardManager.MaxBuildingCardNum) mMyPickingComp = true;
        if ((mMyPickingComp && PhotonNetwork.PlayerList.Length == 1) || (mMyPickingComp && mEnemyPickingComp)) PickingComplete();
    }

    [PunRPC]
    private void PickingEnemyBuildingCard(int cardUniqueNumber)
    {
        mEnemySelectingCardIndex[mEnemySelectingCount++] = cardUniqueNumber;
        BuildingCard buildingCard = Instantiate(mBuildingCards[cardUniqueNumber]);
        buildingCard.transform.SetParent(mEnemyPickingBuilding);

        if (mEnemySelectingCount == mBuildingCardManager.MaxBuildingCardNum) mEnemyPickingComp = true;
        if (mMyPickingComp && mEnemyPickingComp) PickingComplete();
    }

    /// <summary>
    /// ��� ī�� ���� �Ϸ�� �ߵ�
    /// </summary>
    private void PickingComplete()
    {
        mBuildingCardManager.RegisterSelectingCard(mMySelectingCardIndex);
        mBuildingCardManager.RegisterEnemySelectingCard(mEnemySelectingCardIndex);

        for (int i = 0; i < mBuildingCards.Length; i++) mBuildingCards[i].OnPointerDownAction -= PickingBuildingCard;
        
        gameObject.SetActive(false);
    }
       
}
