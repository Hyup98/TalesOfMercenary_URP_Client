using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���
//���� ����
public class GameManager : MonoBehaviour
{
    private const int maxHandSize = 10;
    private const int maxDeckSize = 5;
    private const int initUnitListSize = 15;
    private const int initEventUnitSize = 2;

    [SerializeField] private bool managerMode; //�׽�Ʈ�� ��� (�� ���� ���...)

    public static Vector3 player1Nexus;
    public static Vector3 player2Nexus;

    private DataSender dataSender;
    private UnitJsonData[] unitData;// = new UnitJsonData[6];

    //0: �� 1: ���


    #region ����
    private List<Unit> myHand_Unit;
    private List<Magic> myHand_Magic;
    //������ �ڵ�� 2���� ���� ����Ǿ�� �Ѵ�.
    #endregion

    #region ��

    #endregion

    #region �ʵ� �� ����
    private List<Unit> spawndedMyUnit;
    private List<Unit> spawndedEnemyUnit;
    private List<Unit> spawndedEventUnit;
    #endregion

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
        InitData();

        player1Nexus = GameObject.Find("MyNexus").transform.position;
        player2Nexus = GameObject.Find("EnemyNexus").transform.position;

        MaxGold = 100;
        IncreseGoldTime = 0.25f;
    }

    private void InitData()
    {
        dataSender = FindObjectOfType<DataSender>();
        if (dataSender == null) return;

        unitData = dataSender.GetUnitData();

        Destroy(dataSender.gameObject);
    }

    public UnitJsonData[] GetDeckInfo() => unitData;

    private void FixedUpdate()
    {
        CoolTime += Time.deltaTime;
        CurrentTime += Time.deltaTime;
        CurrentTimeSecond = (int)CurrentTime;

        if (managerMode) CurrentGold = MaxGold;
        if (CurrentGold < MaxGold)
        {
            if (CoolTime > IncreseGoldTime)
            {
                CurrentGold += 1;
                CoolTime = 0;
            }
        }
    }
}
