using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PickManager : MonoBehaviour
{
    [SerializeField] private RectTransform mMyPickingBuilding;
    [SerializeField] private RectTransform mEnemyPickingBuilding;
    [SerializeField] private RectTransform mAllBuildingCard;
    [SerializeField] private BuildingCardManager mBuildingCardManager;

    private BuildingCard [] mBuildingCards;
    private int[] mSelectingCardIndex;
    private int mSelectingCount;
    private bool mPickingComp;
    private void Awake()
    {
        mBuildingCards = new BuildingCard[mAllBuildingCard.childCount];
        mSelectingCardIndex = new int[mBuildingCardManager.MaxBuildingCardNum];

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

        if (mSelectingCount == mBuildingCardManager.MaxBuildingCardNum) PickingComplete();
    }

    /// <summary>
    /// ��� ī�� ���� �Ϸ�� �ߵ�
    /// </summary>
    private void PickingComplete()
    {
        mPickingComp = true;
        mBuildingCardManager.RegisterSelectingCard(mSelectingCardIndex);
        EndPickEvent();
    }
        

    public void EndPickEvent()
    {
        for(int i = 0; i < mBuildingCards.Length; i++)
            mBuildingCards[i].OnPointerDownAction -= PickingBuildingCard;

        gameObject.SetActive(false);
    }
       
}
