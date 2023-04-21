using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.CanvasScaler;

public class ReceiveEvent : MonoBehaviour, IOnEventCallback
{
    public UnityEvent asdf;
    const byte Client = 0;
    const byte Host = 1;
    public enum EEventCode : byte
    {
        #region Host
        HplayerDrawedCard = 0,
        HplayerBuildingUpgraded = 1,
        HplayerSpawnedUnit = 2,
        HplayerNexusUpgraded = 3,
        HunitAttack = 4,
        HunitMovement_vector = 5,
        HunitMovement_target = 6,
        HunitDied = 7,
        HplayerUseMagicCard = 8,
        #endregion

        #region Client
        CplayerDrawedCard = 9,
        CnexusUpgraded = 10,
        CunitDestinationInput = 11,
        CuseMagicCard = 12,
        CbuildingUpgrad = 13,
        CspawnUnit = 14
        #endregion
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        Debug.Log("�޽��� ���� ( " + "�̺�Ʈ �ڵ�" + eventCode + ") ");
        //object[] data = (object[])photonEvent.CustomData;

        switch(eventCode)
        {
            case (byte)EEventCode.HplayerDrawedCard:
                HplayerDrawedCard((object[])photonEvent.CustomData);
                break;
            case (byte)EEventCode.HplayerBuildingUpgraded:
                HplayerBuildingUpgraded((object[])photonEvent.CustomData);
                break;
            case (byte)EEventCode.HplayerSpawnedUnit:
                HplayerSpawnedUnit((object[])photonEvent.CustomData);
                break;

            case (byte)EEventCode.HplayerNexusUpgraded:
                HplayerNexusUpgraded((object[])photonEvent.CustomData);
                break;
            case (byte)EEventCode.HunitAttack:
                HunitAttack((object[])photonEvent.CustomData);
                break;

            case (byte)EEventCode.HunitMovement_vector:
                HunitMovement_vector((object[])photonEvent.CustomData);
                break;
            case (byte)EEventCode.HunitMovement_target:
                HunitMovement_target((object[])photonEvent.CustomData);
                break;

            case (byte)EEventCode.HunitDied:
                HunitDied((object[])photonEvent.CustomData);
                break;
            case (byte)EEventCode.HplayerUseMagicCard:
                HplayerUseMagicCard((object[])photonEvent.CustomData);
                break;
            case (byte)EEventCode.CplayerDrawedCard:
                CplayerDrawedCard((object[])photonEvent.CustomData);
                break;
            case (byte)EEventCode.CnexusUpgraded:
                CnexusUpgraded((object[])photonEvent.CustomData);
                break;

            case (byte)EEventCode.CunitDestinationInput:
                CunitDestinationInput((object[])photonEvent.CustomData);
                break;
            case (byte)EEventCode.CuseMagicCard:
                CuseMagicCard((object[])photonEvent.CustomData);
                break;

            case (byte)EEventCode.CbuildingUpgrad:
                CbuildingUpgrad((object[])photonEvent.CustomData);
                break;
            case (byte)EEventCode.CspawnUnit:
                CspawnUnit((object[])photonEvent.CustomData);
                break;
            default:
                //Debug.Assert(false);
                break;
        }
    }

    #region Client receive(ȣ��Ʈ�� ���� �޽���)
    //1. �޽����� Ȯ���Ѵ�
    //2. �޽����� �ش��ϴ� �̺�Ʈ�� �����Ѵ�/
    public void HplayerDrawedCard(object[] data)
    {
        int userID = (int)data[0];
        Debug.Log(userID);
    }

    public void HplayerBuildingUpgraded(object[] data)
    {
        int userID = (int)data[0];
        int buildingInfo = (int)data[1];
        int amount_of_exp = (int)data[2];
        byte buildingLevel = (byte)data[3];
    }


    public void HplayerSpawnedUnit(object[] data)
    {
        //step1
        int userID = (int)data[0];
        int unitID = (int)data[1];
        int unitInstanceID = (int)data[2];
        Vector3 position = (Vector3)data[3];
        int targetInstaceID = (int)data[4];

        //step2
        NetworkUnitManager.SpawnUnit(userID, unitID, unitInstanceID, position, targetInstaceID);
    }

    public void HplayerNexusUpgraded(object[] data)
    {
        int userID = (int)data[0];
    }

    public void HunitAttack(object[] data)
    {
        int attackUnitInstanceID = (int)data[0];
        int attackedUnitInstanceID = (int)data[1];
        int damage = (int)data[2];
    }


    public void HunitMovement_vector(object[] data)
    {
        //step1
        int unitInstanceID = (int)data[0];
        Vector3 position = (Vector3)data[3];

        //step2
        NetworkUnitManager.UnitMove_Vector(unitInstanceID, position);
    }

    public void HunitMovement_target(object[] data)
    {
        int unitInstanceID = (int)data[0];
        int targetID = (int)data[1];
    }

    public void HunitDied(object[] data)
    {
        int unitInstanceID = (int)data[0];
    }

    public void HplayerUseMagicCard(object[] data)
    {
        int magicID = (int)data[0];
        Vector3 position = (Vector3)data[3];
    }
    #endregion

    #region Host receive(Ŭ���̾�Ʈ�� ���� �޽���)
    //1. �޽����� Ȯ���Ѵ�-
    //2. �޽����� ���� Ŭ���̾�Ʈ���� ������ ��ȯ�϶�� �޽����� ������
    //3. �޽����� �ش��ϴ� �̺�Ʈ�� �����Ѵ�/
    public void CplayerDrawedCard(object[] data)
    {

    }

    public void CnexusUpgraded(object[] data)
    {

    }

    public void CunitDestinationInput(object[] data)
    {
        int unitID = (int)data[0];
        Vector3 position = (Vector3)data[1];
        SendEvent.HunitMovement_vector(unitID, position);
        NetworkUnitManager.UnitMove_Vector(unitID, position);
    }

    public void CuseMagicCard(object[] data)
    {
        int magicID = (int)data[0];
        Vector3 position = (Vector3)data[1];
        SendEvent.HplayerUseMagicCard(magicID, position);
        //���� ����̺�Ʈ ó��
    }

    public void CbuildingUpgrad(object[] data)
    {
        ushort buildingID = (ushort)data[0];
    }

    public void CspawnUnit(object[] data)
    {
        int userID = (int)data[0];
        int unitID = (int)data[1];
        Vector3 position = (Vector3)data[2];

        NetworkUnitManager.SpawnUnit(userID, unitID, position);
    }
    #endregion
}