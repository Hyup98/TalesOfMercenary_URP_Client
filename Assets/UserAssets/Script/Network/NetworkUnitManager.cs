/*using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NetworkUnitManager : MonoBehaviour
{
    public static Unit[] usingUnit = new Unit[4];
    public static Dictionary<ushort, Unit> unitList = new();

    void Start()
    {
        usingUnit = FindObjectOfType<TempUnitData>().GetUnitData();
    }
    public void TakeDamage(ushort unitID, int amount)
    {
    }

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

    #endregion
}*/