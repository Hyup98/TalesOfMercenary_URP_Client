using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SendEvent
{
    private static SendEvent instance = null;
    private SendEvent()
    {
    }
    public static SendEvent Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new SendEvent();
            }
            return instance;
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
        CspawnUnit = 14,
        #endregion
    }
    /// <summary>
    /// ���� ������ ������
    /// 1. ������ ������ ���� ������ �ִ� �� ����(�ؼ��� ����)���� �ڵ����� Ÿ��Ǿ� �̵��Ѵ�
    /// 2. ���� �߰��� ������ �������� �ٲٸ� �ش� ��ġ�� ������ �̵�
    /// 3. ������ ������ ��ġ���� �̵� �� �ٽ� ���� ������ �ִ� �� �������� �ڵ� Ÿ���Ѵ�.
    /// </summary>
    #region Host Message
    
    private static void HplayerDrawedCard(ushort cardID)
    {
        object[] content = new object[] { (ushort)cardID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HplayerDrawedCard, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private static void HplayerBuildingUpgraded(ushort userID, ushort buildingInfo, ushort amount_of_exp, byte buildingLevel)
    {
        object[] content = new object[] { (ushort)userID , (ushort)buildingInfo, (ushort)amount_of_exp, (byte)buildingLevel };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HplayerBuildingUpgraded, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private static void HplayerSpawnedUnit(ushort userID, ushort unitID, ushort unitInstanceID, Vector3 position, ushort targetInstaceID)
    {
        object[] content = new object[] { (ushort)userID, (ushort)unitID, (ushort)unitInstanceID, (Vector3)position ,(ushort)targetInstaceID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HplayerSpawnedUnit, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private static void HplayerNexusUpgraded(ushort userID)
    {
        object[] content = new object[] { (ushort)userID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HplayerNexusUpgraded, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private static void HunitAttack(ushort attackUnitID, ushort attackedUnitID, uint damage)
    {
        object[] content = new object[] { (ushort)attackUnitID, (ushort)attackedUnitID, (uint)damage };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HunitAttack, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private static void HunitMovement_vector(ushort unitID, Vector3 position)
    {
        object[] content = new object[] { (ushort)unitID, (Vector3)position };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HunitMovement_vector, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private static void HunitMovement_target(ushort unitID, ushort targetID)
    {
        object[] content = new object[] { (ushort)unitID, (ushort)targetID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HunitMovement_target, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private static void HunitDied(ushort unitID)
    {
        object[] content = new object[] { (ushort)unitID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HunitDied, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private static void HplayerUseMagicCard(ushort magicID, Vector3 position)
    {
        object[] content = new object[] { (ushort)magicID, (Vector3)position };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)EEventCode.HplayerUseMagicCard, content, raiseEventOptions, SendOptions.SendReliable);
    }

    #endregion

    #region Clinet Message
    private static void CplayerDrawedCard()
    {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent((byte)EEventCode.CplayerDrawedCard, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private static void CnexusUpgraded()
    {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent((byte)EEventCode.CnexusUpgraded, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private static void CunitDestinationInput(ushort unitID, Vector3 position)
    {
        object[] content = new object[] { (ushort)unitID, (Vector3)position };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent((byte)EEventCode.CunitDestinationInput, content, raiseEventOptions, SendOptions.SendReliable);
    }
    private static void CuseMagicCard(ushort magicID, Vector3 position)
    {
        object[] content = new object[] { (ushort)magicID, (Vector3)position };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent((byte)EEventCode.CuseMagicCard, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private static void CbuildingUpgrad(ushort buildingID)
    {
        object[] content = new object[] { (ushort)buildingID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent((byte)EEventCode.CbuildingUpgrad, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private static void CspawnUnit(ushort unitID, Vector3 position)
    {
        object[] content = new object[] { (ushort)unitID, (Vector3)position };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent((byte)EEventCode.CspawnUnit, content, raiseEventOptions, SendOptions.SendReliable);
    }

    #endregion
}
