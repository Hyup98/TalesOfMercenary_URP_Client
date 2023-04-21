
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Host = 0
/// Client = 1
/// </summary>
public class NetworkUnitManager : MonoBehaviour
{
    public static readonly int Host = 0;
    public static readonly int Client = 1;

    [SerializeField] private Nexus[] damageable;

    public static Damageable[] usingUnit = new Damageable[4];
    /// <summary>
    /// instanceID�� ���� �ʵ忡�ִ� ���� ������ ��� ������ ��ȣ�� �����ϰ� �ʹ�.
    /// �ؼ����� 0,1�� �������� �Ҵ��ϰ� ���� 
    /// HOST �ؼ��� = 0
    /// CLIENT �ؼ��� = 1
    /// </summary>
    public static int instanceID = 2;
    //{ "���� �ν��Ͻ� ���̵�": "����" }
    public static Dictionary<int, Damageable> unitList = new();
    public static Dictionary<int, BuildingCard> buildingList = new();



    void Awake()
    {
        usingUnit = FindObjectOfType<TempUnitData>().GetUnitData();
        for(int i = 0; i < damageable.Length; i++) unitList.Add(i, damageable[i]);
    }

    #region Ŭ���̾�Ʈ
    /// <summary>
    /// Ŭ���̾�Ʈ�� �Լ�
    /// Ÿ�� �ν��Ͻ��� �ѵ��� �����ϸ� �ȴ�. -> ���� ��Ÿ��� �Ǹ� �ű⼭ �����ϰ� ���� 
    /// ���� Ÿ���� �ٽ� ���� �����Ÿ� ������ ������ �ٽ� ���� �����Ÿ����� �̵� ->�Ѿư��� �����ϴ� ���� �ʿ�
    /// �� ������ �������� ���� ��Ŷ�� �� ������
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="unitID"></param>
    /// <param name="unitInstanceID"></param>
    /// <param name="position"></param>
    /// <param name="targetInstaceID"></param>
    public static void SpawnUnit(int userID, int unitID, int unitInstanceID, Vector3 position, int targetInstaceID)
    {
        /*todo
         * 1. ������ �迭�� �߰�
         */

        Debug.Log("SpawnUnit Host���� ���Ź���");
        //Debug.LogFormat("UnitSpwan : , {0} , {1}, {2}, {4}, {5}", userID, unitID, unitInstanceID, position, targetInstaceID);
        unitList.Add( unitInstanceID, usingUnit[unitID]);
        GameObject obj = Instantiate(usingUnit[unitID], position, Quaternion.identity).gameObject;

        unitList.TryGetValue(targetInstaceID, out Damageable damageable);
        obj.GetComponent<Unit>().InitBatch(userID, unitInstanceID,damageable.transform.position);
    }
    
    public static void UnitMove_Vector(int unitInstanceID, Vector3 position)
    { 
        /*todo
         * ���� ���� �迭�� ������ ���� �������� �������ش�.
         */
    }

    public static void UnitAttack(int attackUnitInstanceID, int attackedUnitInstanceID, int damage)
    {
        
    }

    public static void UnitMove_target(int unitInstanceID, int targetID)
    {

    }

    public static void UnitDied(int unitInstanceID)
    {

    }

    public static void PlayerNexusUpgraded()
    {
        //�迭�� �ؼ��� ������ �ø���
    }
    #endregion

    #region ȣ��Ʈ
    /// <summary>
    /// �ش� ��ġ���� ���� ����� �� Ž�� �� �ش� ��ġ�� Ÿ�� ���� -> �迭�� ��ȸ�ϸ� ����� ������ Ÿ�� ����
    /// ���� �� �޽��� ���� �׸��� �ش� ���� ���� �� �迭�� �߰�
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="unitID"></param>
    /// <param name="position"></param
    public static void SpawnUnit(int userID, int unitID, Vector3 position)
    {
        /*todo
         * 1. ���� ���Ϳ��� ���� ����� �� Ž��
         * 2. �ش� ���� Ÿ�������ϴ� ���� ����
         * 3. ������ �迭�� �߰�
         */
        //unitList.Add(instanceID, usingUnit[unitID]);

        Debug.Log("������ ���� �ν��Ͻ� ��ȣ" + instanceID);
        SendEvent.HplayerSpawnedUnit(userID, unitID, instanceID, position, 0);
        unitList.Add(instanceID, usingUnit[unitID]);
        GameObject obj = Instantiate(usingUnit[unitID], position, Quaternion.identity).gameObject;

        unitList.TryGetValue(0, out Damageable damageable);
        obj.GetComponent<Unit>().InitBatch(userID, 0, damageable.transform.position);
        //���⿡ ���� �迭�� �߰��� �ʱ�ȭ
        Debug.LogFormat("Ŭ�󿡰� ���� �����޽��� ���� �Ϸ�");
        instanceID++;
        Debug.Log(instanceID);
    }

    public static void InputUnitMove_Vector(int unitInstanceID, Vector3 position)
    {
        /*todo
         * 1. �ڽ��� ������ �������� Ȯ��
         * 2. �ùٸ� �Է��̸� ���� �������� Ŭ���̾�Ʈ���� ������
         * 3. �迭�� ������ �������� �ٲ��ش�.
         */
        UnitMove_Vector(unitInstanceID, position);
    }
    #endregion
}