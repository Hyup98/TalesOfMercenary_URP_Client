using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class Receive : MonoBehaviour, IOnEventCallback
{
    public enum EventCode : byte
    {
        EsPlayerDrawedCardMessage = 0,
        EsPlayerBuildingUpgradedMessage = 1,
        EsPlayerSpawnedUnitMessage = 2,
        EsPlayerNexusUpgradedMessage = 3,
        EsUnitAttackMessage = 4,
        EsUnitMovementMessage = 5,
        EsUnitDiedMessage = 6,
        EsPlayerUseMagicCardMessage = 7,
        EsUnitTrackMovemntMessage = 8,
        EsGameStartMessage = 9,
        EcPlayerDrawedCardMessage = 10,
        EcNexusUpgradedMessage = 11,
        EcUnitDestinationInputMessage = 12,
        EcUseMagicCardMessage = 13,
        EcBuildingUpgradeMessage = 14,
        EcSpawnUnitMessage = 15
    }
    private void OnEnable()
    {
        Debug.Log("�ݹ� ���");
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        //Debug.Log(RoomManager.Instance.m_PhotonView.IsMine + "@@@@@@@@@@@@@@@@@@@@@@@@");
        Debug.Log("�޽��� ����");
        Debug.Log("�̺�Ʈ �ڵ�" + eventCode);
        if (eventCode == (byte)EventCode.EsPlayerBuildingUpgradedMessage)
        {
            //Debug.Log("1�� ���� ");
            object[] data = (object[])photonEvent.CustomData;
            //Debug.Log("������ ����" + data.Length);
            Vector3 targetPosition = (Vector3)data[0];
            byte id1 = (byte)data[1];
            byte id2 = (byte)data[2];
            Debug.Log(id2 + "attack" + id1 + "move point -> " + targetPosition + "�޾Ҵ�!!!!!!!!!");
        }
        else if (eventCode == (byte)EventCode.EsPlayerSpawnedUnitMessage)
        {
            //Debug.Log("2�� ����");
            object[] data = (object[])photonEvent.CustomData;
            Debug.Log((byte)data[0] + " ������ ���� ������ �޾Ҵ�!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }
}