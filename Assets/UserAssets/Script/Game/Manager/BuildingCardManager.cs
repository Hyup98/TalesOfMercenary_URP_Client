using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("CardProbability")]
    [SerializeField] private CardProbability[] mCardProbability;
    #endregion

    private const int maxDeckCardNum = 2;
    private const float mTotal = 100;

    private GameManager gameManager;
    private BuildingCard nexusCard;
    private BuildingCard[] deckCards;
    //deckCards 0�� index = NexusCard


}
