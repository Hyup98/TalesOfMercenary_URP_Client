using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicEventDisplayer : MonoBehaviour
{
    [SerializeField] private DragableCardManager mDragableCardManager;
    [SerializeField] private MagicCard[] mMagicCards;
    [SerializeField] private RectTransform mUIPos;
    [SerializeField] private int mBatchNum;

    public void Init()
    {

    }

    [ContextMenu("DisplayMagicEvent")]
    public void DisplayMagicEvent()
    {
        gameObject.SetActive(true);
        RectTransform rectTransform;
        MagicCard magicCard;
        for (int i = 0; i < mBatchNum; i++)
        {
            magicCard = Instantiate(mMagicCards[i]);
            magicCard.IsSelectingMode = true;

            rectTransform = magicCard.GetComponent<RectTransform>();
            rectTransform.SetParent(mUIPos);
            rectTransform.SetAsLastSibling();

            magicCard.OnPointerDownSelectAction += ProcessSelect;
        }
    }

    private void ProcessSelect(int cardUniqueID)
    {
        mDragableCardManager.LoadMagicCard(mMagicCards[cardUniqueID]);
        EndMagicEvent();
    }

    private void EndMagicEvent()
    {
        //�̹���, �ؽ�Ʈ�� �ٲ�� ����
        //��ȭ�� ũ�� ���� ��� �ߺ� �̸� ���� �� ���� ����
        //�� �ܴ� ���� + ����
        gameObject.SetActive(false);
    }
}
