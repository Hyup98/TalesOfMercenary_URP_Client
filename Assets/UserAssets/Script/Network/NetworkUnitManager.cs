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
    public static Dictionary<string, Damageable> enemyUnitList { get; } = new();
    public static Dictionary<string, Damageable> myUnitList { get; } = new();
    public static List<BuildingCard> enemyBuildingList { get; } = new();



    private void Init()
    {
        enemyUnitList.Clear();
        myUnitList.Clear();
        enemyBuildingList.Clear();
    }

    private void Awake()
    {
        Init();

        mGameManger = GetComponent<GameManager>();

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

    public static void AddmyUnit(string uuid, Damageable unit)
    {
        myUnitList.Add(uuid, unit);
        Debug.Log(uuid + " : �Ʊ� ���� �߰�");
    }

    public static void RemoveMyUnit(string uuid)
    {
        bool success;
        success = myUnitList.Remove(uuid);
        Debug.Log(uuid + " : �Ʊ� ���� ���� ->" + success + "���� ���� ����" + myUnitList.Count);
        foreach (KeyValuePair<string, Damageable> kv in myUnitList)
        {
            Debug.Log(kv.Key + "���� �Ʊ�");
        }
    }

    public static void AddEnemyUnit(string uuid, Damageable unit)
    {
        enemyUnitList.Add(uuid, unit);
        Debug.Log(uuid + " : �� ���� �߰�");
    }

    public static void RemoveEnemyUnit(string uuid)
    {
        bool success;
        success = enemyUnitList.Remove(uuid);
        Debug.Log(uuid + " : �� ���� ���� -> " + success + "���� ���� ����" + enemyUnitList.Count);
        foreach (KeyValuePair<string, Damageable> kv in enemyUnitList)
        {
            Debug.Log(kv.Key + "���� ����");
        }
    }
}