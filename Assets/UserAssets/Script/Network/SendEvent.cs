using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SendEvent : MonoBehaviour
{

    void Start()
    { 
        if (!PhotonNetwork.IsMasterClient)
        {
            int userID = 0;
            int unitID = 1;
            //CspawnUnit(userID, unitID, new Vector3(0, 0, 0));
        }
        else
        {
            //��ü ó�� ��
            //HplayerSpawnedUnit()
        }
    }

    /// <summary>
    /// Clinet �޽��� EventCode�� ���λ�� C 
    /// Host �޽��� EvnetCode�� ���λ�� H
    /// </summary>
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
    /// <summary>
    /// ���� ������ ������
    /// 1. ������ ������ ���� ������ �ִ� �� ����(�ؼ��� ����)���� �ڵ����� Ÿ��Ǿ� �̵��Ѵ�
    /// 2. ���� �߰��� ������ �������� �ٲٸ� �ش� ��ġ�� ������ �̵�
    /// 3. ������ ������ ��ġ���� �̵� �� �ٽ� ���� ������ �ִ� �� �������� �ڵ� Ÿ���Ѵ�.
    /// </summary>
    #region Host Message

    public static void HplayerDrawedCard(int cardID)
    {
        object[] content = new object[] { (int)cardID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HplayerDrawedCard, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void HplayerBuildingUpgraded(int userID, int buildingInfo, int amount_of_exp, byte buildingLevel)
    {
        object[] content = new object[] { (int)userID , (int)buildingInfo, (int)amount_of_exp, (byte)buildingLevel };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HplayerBuildingUpgraded, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void HplayerSpawnedUnit(int userID, int unitID, int unitInstanceID, Vector3 position, int targetInstaceID)
    {
        object[] content = new object[] { (int)userID, (int)unitID, (int)unitInstanceID, (Vector3)position ,(int)targetInstaceID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HplayerSpawnedUnit, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void HplayerNexusUpgraded(int userID)
    {
        object[] content = new object[] { (int)userID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HplayerNexusUpgraded, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void HunitAttack(int attackUnitInstanceID, int attackedUnitInstanceID, int damage)
    {
        object[] content = new object[] { (int)attackUnitInstanceID, (int)attackedUnitInstanceID, (int)damage };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HunitAttack, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void HunitMovement_vector(int unitInstanceID, Vector3 position)
    {
        object[] content = new object[] { (int)unitInstanceID, (Vector3)position };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HunitMovement_vector, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void HunitMovement_target(int unitInstanceID, int targetInstanceID)
    {
        object[] content = new object[] { (int)unitInstanceID, (int)targetInstanceID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HunitMovement_target, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void HunitDied(int unitInstanceID)
    {
        object[] content = new object[] { (int)unitInstanceID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HunitDied, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void HplayerUseMagicCard(int magicID, Vector3 position)
    {
        object[] content = new object[] { (int)magicID, (Vector3)position };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HplayerUseMagicCard, content, raiseEventOptions, SendOptions.SendReliable);
    }

    #endregion

    #region Clinet Message
    public static void CplayerDrawedCard()
    {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent((byte)EEventCode.CplayerDrawedCard, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void CnexusUpgraded()
    {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent((byte)EEventCode.CnexusUpgraded, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void CunitDestinationInput(int unitInstanceID, Vector3 position)
    {
        object[] content = new object[] { (int)unitInstanceID, (Vector3)position };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent((byte)EEventCode.CunitDestinationInput, content, raiseEventOptions, SendOptions.SendReliable);
    }


    public static void CunitMovementInput_vector(int unitInstanceID, Vector3 position)
    {
        object[] content = new object[] { (int)unitInstanceID, (Vector3)position };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HunitMovement_vector, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void CuseMagicCard(int magicID, Vector3 position)
    {
        object[] content = new object[] { (int)magicID, (Vector3)position };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent((byte)EEventCode.CuseMagicCard, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void CbuildingUpgrad(int buildingID)
    {
        object[] content = new object[] { (int)buildingID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent((byte)EEventCode.CbuildingUpgrad, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void CspawnUnit(int userID, int unitID, Vector3 position)
    {
        object[] content = new object[] { userID, unitID, position };
        Debug.LogFormat("{0}, {1}, {2}",(int)content[0] , (int)content[1] , (Vector3)content[2]);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent((byte)EEventCode.CspawnUnit, content, raiseEventOptions, SendOptions.SendReliable);
    }
    #endregion
}
