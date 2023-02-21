using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private DataSender dataSender;
    [SerializeField] private RectTransform allCard;

    private BanPickManager banPickManager;
    public DeckCard[] deckCards { get; private set; } = new DeckCard[4];
    private UnitJsonData[] allUnitData;
    private UnitJsonData[] usingUnitData;
    private AuthenticationManager _authenticationManager;

    //�κ�� ���ƿö����� ȣ���
    //-> static�̳� bool������ ������ �ε� 1���� �ǵ��� ���� �ʿ�
    private void Awake()
    {
        banPickManager = GetComponent<BanPickManager>();
        //deckCards = allCard.GetComponentsInChildren<DeckCard>();
        allUnitData = new UnitJsonData[deckCards.Length];
        _authenticationManager = FindObjectOfType<AuthenticationManager>();
        InitDataSetting();
    }

    /// <summary>
    /// ��� ī�� ������ �ε�
    /// </summary>
    private void InitDataSetting()
    {
        allUnitData[0] = JsonManager.LoadJsonFile<UnitJsonData>("0_Warrior_Data");
        allUnitData[1] = JsonManager.LoadJsonFile<UnitJsonData>("1_Wizard_Data");
        allUnitData[2] = JsonManager.LoadJsonFile<UnitJsonData>("2_ShieldMan_Data");
        allUnitData[3] = JsonManager.LoadJsonFile<UnitJsonData>("3_Archer_Data");
        //allUnitData[0].Print();
    }

    /// <summary>
    /// ���� ���� �� ���� ����
    /// </summary>
    public void GameStart()
    {
        int[] dataNum = banPickManager.GetUsingCardNum();
        usingUnitData = new UnitJsonData[dataNum.Length];
        for (int i = 0; i < dataNum.Length; i++) usingUnitData[i] = allUnitData[dataNum[i]];
        
        dataSender.SetUsingCardData(usingUnitData);
        SceneManager.LoadScene(1);
    }

    public void OnClick()
    {
        Debug.Log("onLoginClicked ");
        string loginUrl = _authenticationManager.GetLoginUrl();
        Application.OpenURL(loginUrl);
    }
}
