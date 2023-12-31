using Photon.Pun;
using Scriptable;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NetworkUnitManager : MonoBehaviour
{
    /// <summary>
    /// 1: ��
    /// 0: ��
    /// </summary>
    private GameManager mGameManger;

    public static Damageable[] usingUnit = new Damageable[4];
    public static Dictionary<string, Damageable> enemyUnitList { get; } = new();
    public static Dictionary<string, Damageable> myUnitList { get; } = new();
    public static List<BuildingCard> mybuildingList { get; } = new();
    public static List<BuildingCard> enemyBuildingList { get; } = new();



    private void Init()
    {
        usingUnit = new Damageable[4];
        enemyUnitList.Clear();
        myUnitList.Clear();
        mybuildingList.Clear();
        enemyBuildingList.Clear();
    }

    private void Awake()
    {
        Init();

        mGameManger = GetComponent<GameManager>();
        usingUnit = FindObjectOfType<TempUnitData>().GetUnitData();

        if (PhotonNetwork.IsMasterClient)
        {
            enemyUnitList.Add("1", mGameManger.GetNexus(1));
            myUnitList.Add("1", mGameManger.GetNexus(0));
        }
        else
        {
            enemyUnitList.Add("1", mGameManger.GetNexus(0));
            myUnitList.Add("1", mGameManger.GetNexus(1));
        }
    }
}