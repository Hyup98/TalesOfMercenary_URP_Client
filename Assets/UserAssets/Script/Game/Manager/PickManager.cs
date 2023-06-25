using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Scriptable;
using System.ComponentModel;

//todo

//

public class PickManager : MonoBehaviour
{
    [SerializeField] private RectTransform mMyPickingBuilding;
    [SerializeField] private RectTransform mEnemyPickingBuilding;
    [SerializeField] private RectTransform mAllBuildingCard;
    [SerializeField] private BuildingCardManager mBuildingCardManager; //ī�� ���� �켱 4�� �ٲ����

    private BuildingCard [] mBuildingCards;
    private BuildingCard[] mEnemyBuildingCards; 
    private int[] mSelectingCardIndex;
    private int[] mSelectingEnemyCardIndex;
    private int mEnemySelectingCount;
    private int mSelectingCount;
    private bool mPickingComp = false;
    private bool mEnemyPickingComp = false;
    private PhotonView mPhotonView;


    private void Awake()
    {
        mPhotonView = GetComponent<PhotonView>();
        mBuildingCards = new BuildingCard[mAllBuildingCard.childCount];
        mSelectingCardIndex = new int[mBuildingCardManager.MaxBuildingCardNum / 2];
        mSelectingEnemyCardIndex = new int[mBuildingCardManager.MaxBuildingCardNum / 2];
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
        if (mPickingComp) return;

        mSelectingCardIndex[mSelectingCount++] = cardUniqueNumber;

        BuildingCard buildingCard = Instantiate(mBuildingCards[cardUniqueNumber]);
        buildingCard.transform.SetParent(mMyPickingBuilding);
        Debug.Log("�Ʊ� ī�� �̱�" + cardUniqueNumber + "    :    " + mSelectingCount);
        mPhotonView.RPC(nameof(PickingBuildingCardRPC), RpcTarget.Others, cardUniqueNumber);

        if (mSelectingCount == mBuildingCardManager.MaxBuildingCardNum) PickingComplete();
    }

    
    [PunRPC]
    public void PickingBuildingCardRPC(int cardUniqueNumber) //���� �� �� ī�� ����Ʈ �ޱ�
    {
        mSelectingEnemyCardIndex[mEnemySelectingCount++] = cardUniqueNumber;

        //���� ����ī�� �ν��Ͻÿ���Ʈ ���ּ���//
        BuildingCard buildingCard = Instantiate(mBuildingCards[cardUniqueNumber]);
        buildingCard.transform.SetParent(mMyPickingBuilding);
        Debug.Log("�� ī�� �̱�" + cardUniqueNumber + "    :    " + mSelectingCount);
        if (mEnemySelectingCount == (mBuildingCardManager.MaxBuildingCardNum / 2)) EnemyPickingComplete();
    }
   
    /// <summary>
    /// ��� ī�� ���� �Ϸ�� �ߵ�
    /// </summary>
    private void PickingComplete()
    {
        mPickingComp = true;
        mBuildingCardManager.RegisterSelectingCard(mSelectingCardIndex);
        if(mPickingComp == true && mEnemyPickingComp == true)
        {
            EndPickEvent();
        }
    }

    private void EnemyPickingComplete()
    {
        mEnemyPickingComp = true;
        mBuildingCardManager.RegisterSelectingCard(mSelectingCardIndex);
        if (mPickingComp == true && mEnemyPickingComp == true)
        {
            EndPickEvent();
        }
    }


    //���� �ʿ�
    public void EndPickEvent()
    {
        for(int i = 0; i < mBuildingCards.Length; i++)
        {
            mBuildingCards[i].OnPointerDownAction -= PickingBuildingCard;
            mEnemyBuildingCards[i].OnPointerDownAction -= PickingBuildingCard;
        }


        gameObject.SetActive(false);
    }
       
}
