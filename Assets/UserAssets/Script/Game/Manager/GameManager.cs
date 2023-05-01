using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool managerMode; //�׽�Ʈ�� ��� (�� ���� ���...)
    [SerializeField] private Nexus[] damageable;
    [SerializeField] private GameObject[] mCamera;
    [SerializeField] private LayerMask mHostLayer;
    [SerializeField] private LayerMask mClientLayer;

    public static int MyUnitLayer;
    public static int EnemyUnitLayer;
    public static readonly int HOST_NUMBER = 0;
    public static readonly int CLIENT_NUMBER = 1;

    public Nexus GetNexus(int i) => damageable[i];

    #region �ð�
    //float ���� �ð�
    public float CurrentTime { get; private set; }
    //int ���� �ð�
    public int CurrentTimeSecond { get; private set; }
    //�� �� �� ���� ���ʹ�
    #endregion

    #region ������
    //���� ��ġ �̻���

    //���� �ִ� ���
    public int MaxGold { get; set; }

    //���� ���
    public int CurrentGold { get; set; }

    //���� ��� ���� �ð�
    public float IncreseGoldTime { get; set; } // IncreseGoldTime�ʴ� 1�� ����

    //��� ���� �ð� ����
    private float CoolTime { get; set; }
    #endregion

    /// <summary>
    /// CurrentGold�� ������ ���ų� Ŭ ��� CurrentGold���� ���� ���� true, �ƴҰ�� false
    /// </summary>
    /// <param name="gold">�Ҹ��� ���</param>
    /// <returns>return CurrentGold >= gold</returns>
    public bool DoValidGold(int gold)
    {
        if (CurrentGold >= gold)
        {
            CurrentGold -= gold;
            return true;
        }
        return false;
    }

    public void UpgradeNexus(float decreseTime = 0.05f)
    {
        IncreseGoldTime -= decreseTime;
    }

    private void Awake()
    {
        InitCamera();
        InitLayer();

        MaxGold = 100;
        IncreseGoldTime = 0.25f;
    }

    private void InitCamera()
    {
        mCamera[0].SetActive(PhotonNetwork.IsMasterClient);
        mCamera[1].SetActive(!PhotonNetwork.IsMasterClient);
    }

    private void InitLayer()
    {
        //MyUnitLayer = PhotonNetwork.IsMasterClient ? mHostLayer : mClientLayer;
        //EnemyUnitLayer = PhotonNetwork.IsMasterClient ? mClientLayer : mHostLayer;

        MyUnitLayer = 16;
        EnemyUnitLayer = 17;

        Debug.Log(MyUnitLayer);
        Debug.Log(EnemyUnitLayer);
    }

    private void FixedUpdate()
    {
        CoolTime += Time.deltaTime;
        CurrentTime += Time.deltaTime;
        CurrentTimeSecond = (int)CurrentTime;

        #region Gold
        if (managerMode) CurrentGold = MaxGold;
        if (CurrentGold < MaxGold)
        {
            if (CoolTime > IncreseGoldTime)
            {
                CurrentGold += 1;
                CoolTime = 0;
            }
        }
        #endregion
        #region Event
        //if(CurrentTime == DragonEventTime)
        //{
        //    if(PhotonNetwork.IsMasterClient)
        //    {
        //        DragonSpawnEvent();
        //    }
        //}
        #endregion
    }

    private void DragonSpawnEvent()
    {
        GameObject obj = null;
        Vector3 spwanPostion = new Vector3(0f, 0f, 0f);
        obj = PhotonNetwork.Instantiate("OfficialUnit/NeutralUnit/" + "RedDragon", spwanPostion, Quaternion.identity);
        //obj.GetComponent<NeutralUnit>().Init();
    }

}
