using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Host = 1
/// Client = 0
/// </summary>
public class NetworkUnitManager : MonoBehaviour
{

    private void Awake()
    {
        //�ؼ����� ���� ����Ʈ�� �ֱ�
    }
    public static Unit[] usingUnit = new Unit[4];
    /// <summary>
    /// instanceID�� ���� �ʵ忡�ִ� ���� ������ ��� ������ ��ȣ�� �����ϰ� �ʹ�.
    /// �ؼ����� 0,1�� �������� �Ҵ��ϰ� ���� 
    /// HOST �ؼ��� = 0
    /// CLIENT �ؼ��� = 1
    /// </summary>
    static int instanceID = 2;
    //{ "���� �ν��Ͻ� ���̵�": "����" }
    private static Dictionary<int, Unit> unitList = new();

    void Start()
    {
        //usingUnit = FindObjectOfType<TempUnitData>().GetUnitData();
    }
    public void TakeDamage(int unitID, int amount)
    {

    }

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
        Debug.LogFormat("UnitSpwan : , {0} , {1}, {2}, {4}, {5}", userID, unitID, unitInstanceID, position, targetInstaceID);
    }
    /// <summary>
    /// ȣ��Ʈ�� �Լ�
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
        int targetInstaceID = 10;
        Debug.Log("������ ���� �ν��Ͻ� ��ȣ" + instanceID);
        SendEvent.HplayerSpawnedUnit(userID, unitID, instanceID, position, targetInstaceID);
        Debug.LogFormat("Ŭ�󿡰� ���� �����޽��� ���� �Ϸ�");
        instanceID++;
        Debug.Log(instanceID);
    }

    public static void UnitMove_Vector(int unitInstanceID, Vector3 position)
    { 
        Debug.LogFormat("SendMovement() {0}, {1}", instanceID, position);
    }
    /*
        #region Massage
        [MessageHandler((ushort)ClientToServerId.spawnUnit)]
        private static void SpawnUnit(ushort ownerID, Message message)
        {
            byte unitDataNum = message.GetByte();
            Vector3 spawnPosition = message.GetVector3();
            Vector3 finalDestination;
            if (ownerID != 1) finalDestination = GameManager.player1Nexus;
            else finalDestination = GameManager.player2Nexus;

            Debug.LogFormat("getSpawnUnit(), {0}, {1}", unitDataNum, spawnPosition);
            //��� Ŭ���̾�Ʈ���� �����϶�� �޽��� 
            Message newMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.playerSpawnedUnit);
            newMessage.AddUShort(ownerID);//������ ������ Ŭ���̾�Ʈ ID
            newMessage.AddByte(unitDataNum);//�����ͺ��̽� ID
            newMessage.AddUShort(instanceID);//�ν��Ͻ� ���̵�
            newMessage.AddVector3(spawnPosition); //���� ��ȯ ��ġ
            newMessage.AddVector3(finalDestination); //���� �̵� ��ġ

            NetworkManager.NetworkManagerSingleton.Server.SendToAll(newMessage);
            Debug.LogFormat("sendSpawnUnit(), {0}, {1}, {2}, {3}, {4}", ownerID, unitDataNum, instanceID, spawnPosition, finalDestination);
            //����Ǯ�� ���� ���� 

            Unit unit = Instantiate(usingUnit[unitDataNum], spawnPosition, Quaternion.identity);
            unit.InitBatch(ownerID, instanceID, finalDestination);


            unitList.Add(instanceID, unit);

            foreach (KeyValuePair<ushort, Unit> units in unitList)
            {
                Debug.LogFormat("UnitList : , {0} , {1}", units.Key, units.Value);
            }
            instanceID++;
        }

        [MessageHandler((ushort)ClientToServerId.unitDestinationInput)]
        private static void ReceiveSendMovement(ushort ownerID, Message message)
        {
            ushort instanceID = message.GetUShort();
            Vector3 destination = message.GetVector3();

            Debug.LogFormat("get unitDestinationInput() {0}, {1}", instanceID, destination);

            Message newMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.unitMovement);
            newMessage.AddUShort(instanceID);//�ν��Ͻ� ���̵�
            newMessage.AddVector3(destination);//������
            NetworkManager.NetworkManagerSingleton.Server.SendToAll(newMessage);

            unitList[instanceID].SetDestination(destination);

            Debug.LogFormat("SendMovement() {0}, {1}", instanceID, destination);
        }


        public static void SendUnitTrackMovement(ushort trackingInstanceID, ushort trackedInstanceID)
        {
            Message newMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.unitMovement);
            newMessage.AddUShort(trackingInstanceID);// �� ���� �ν��Ͻ� ���̵�
            newMessage.AddUShort(trackedInstanceID);// ���� ���� �ν��Ͻ� ���̵�
            NetworkManager.NetworkManagerSingleton.Server.SendToAll(newMessage);
        }

        public static void SendUnitMovement(ushort instanceID, Vector3 destination)
        {
            Message newMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.unitMovement);
            newMessage.AddUShort(instanceID);//�ν��Ͻ� ���̵�
            newMessage.AddVector3(destination);//������
            NetworkManager.NetworkManagerSingleton.Server.SendToAll(newMessage);
        }

        public static void SendUnitAttack(ushort attackingInstanceID, ushort attackedInstanceID, int calcDamage)
        {
            Message newMessage = Message.Create(MessageSendMode.Reliable, ServerToClientId.unitAttack);
            newMessage.AddUShort(attackingInstanceID);// �����ϴ� ����
            newMessage.AddUShort(attackedInstanceID);// ���ݴ��ϴ� ����
            newMessage.AddInt(calcDamage);
            NetworkManager.NetworkManagerSingleton.Server.SendToAll(newMessage);
            Debug.LogFormat("SendUnitAttack() {0}, {1}", attackingInstanceID, attackedInstanceID, calcDamage);
        }

        public static void SendUnitDied(ushort unitInstanceID)
        {
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.unitDied);
            message.AddUShort(unitInstanceID); //���� ���� �ν��Ͻ� ���̵�
            NetworkManager.NetworkManagerSingleton.Server.SendToAll(message);

            Debug.LogFormat("SendUnitDied() {0}", unitInstanceID);
        }

        #endregion*/
}