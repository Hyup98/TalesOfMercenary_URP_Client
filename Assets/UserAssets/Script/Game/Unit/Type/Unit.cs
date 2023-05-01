using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using Scriptable;
using UnityEditor;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
/// <summary>
/// target�� �ٲ𶧸��� settarget()�Լ��� ȣ���ؼ� ��� �������� Ÿ�� ������ ����ȭ����� �Ѵ�.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Unit : Damageable
{
    #region Object info
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int unitId;        //�ĺ���ȣ

    private UnitAnimationController mUnitAnimationController;
    private Damageable mTarget;
    private NavMeshAgent mNavMeshAgent;
    #endregion

    #region logic Info
    private bool mIsBatch;
    private bool mIsMoving = true;

    private float mAttackDelay; // ���� �ӵ� ����
    private int mPriority;
    private const int mFightPriority = 5;

    private Vector3 mVectorDestination;
    private PhotonView mPhotonView;
    private Attackable mAttack;
    private string mTargetUUID;
    private List<string> mRemoveList = new List<string>();
    #endregion

    #region Property
    public bool IsClicked { get; set; }
    public bool IsEnemy { get; private set; }
    #endregion

    protected virtual void Awake()
    {
        mUnitAnimationController = GetComponent<UnitAnimationController>();
        mPhotonView = GetComponent<PhotonView>();
        mAttack = GetComponent<Attackable>();
    }

    public void InitBatch()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mPriority = mNavMeshAgent.avoidancePriority;
        mCurrentHp = mUnitScriptable.maxHP;

        IsAlive = true;
        mNavMeshAgent.enabled = true;
        gameObject.layer = GameManager.mMyUnitLayer;

        mIsBatch = true;
        Findenemy();
        mNavMeshAgent.SetDestination(mTarget.transform.position);
        mUnitScriptable.UUID = MyUUIDGeneration.GenrateUUID();

        NetworkUnitManager.myUnitList.Add(mUnitScriptable.UUID, this);
        mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others, mUnitScriptable.UUID);
    }

    [PunRPC]
    public void SyncInitBatch(string uuid) //���� ��ȯ�� ���� �ʱ�ȭ
    {
        NetworkUnitManager.enemyUnitList.Add(uuid, this);
        mCurrentHp = mUnitScriptable.maxHP;
        gameObject.layer = GameManager.mEnemyUnitLayer;
        mUnitScriptable.UUID = uuid;
        IsAlive = true;
    }

    public override string getUUID()
    {
        return mUnitScriptable.UUID;
    }

    private void FixedUpdate()
    {
        if (!mIsBatch) return;
        if (!mPhotonView.IsMine) return;
        if (IsAlive == false)
        {
            Die();
            mPhotonView.RPC(nameof(DieRPC), RpcTarget.Others);
            return;
        }
        mAttackDelay += Time.deltaTime;

        if (mTarget != null) // Ÿ���� ���� ��
        {
            if (!NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID) || !mTarget.IsAlive) // Ÿ�� ��� Ȯ�� 
            {
                Debug.Log("Ÿ�� ����� Ÿ�� �� Ž��");
                Findenemy();
                return;
            }
            TargetMove();
        }
        else NonTargetMove();
        mUnitAnimationController.PlayMoveAnimation(mIsMoving);
    }

    #region move
    private void TargetMove()
    {
        float dist = Vector3.Distance(mTarget.transform.position, transform.position);

        if (dist <= mUnitScriptable.attackRange) // Ÿ���� ���� ���� ������ �������� -> �����ϰ� ����
        {
            Debug.Log("Ÿ�� Ÿ�� : " + mTarget.mUnitScriptable.unitName + " ���� �� ����");
            mIsMoving = false;
            mNavMeshAgent.avoidancePriority = mFightPriority;
            mNavMeshAgent.SetDestination(transform.position);
            if (mAttackDelay >= mUnitScriptable.attackSpeed)
            {
                mAttack.Attack(this, mTarget);
                mAttackDelay = 0;
            }
        }
        else//Ÿ���� ���� �������� �ֶ�
        {
            //Debug.Log("Ÿ�� Ÿ�� : " + mTarget.mUnitScriptable.unitName + "���� �Ÿ�: " + dist);
            mIsMoving = true;
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
    }

    private void NonTargetMove()
    {
        if (!NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID)) // Ÿ���� �׾�����
        {
            mTarget = null;
            Findenemy();
            return;
        }
        float dist = Vector3.Distance(mVectorDestination, transform.position);
        //Debug.Log("���� �Ÿ�: " + dist);
        mIsMoving = true;
        if (dist <= mUnitScriptable.movementRange) // �������� ���� ��Ÿ� �� �϶�
        {
            Findenemy();
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
        else
        {
            Debug.Log("���ͷ� �̵� ��");
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mVectorDestination);
        }
    }

    public void PointMove(Vector3 pos)
    {
        mTarget = null;
        mVectorDestination = pos;
        mNavMeshAgent.stoppingDistance = 0.15f;
        mNavMeshAgent.SetDestination(mVectorDestination);
    }
    #endregion

    private void Findenemy()
    {
        Debug.Log("�� Ž��...");
        Debug.Log("�� ���� ���� : " + NetworkUnitManager.enemyUnitList.Count);
        float minDis = float.MaxValue;
        Damageable target = null;
        float tem;
        string temUUID = null;
        mRemoveList.Clear();

        foreach (var key in NetworkUnitManager.enemyUnitList)
        {
            if (key.Value.IsAlive)
            {
                tem = (transform.position - key.Value.transform.position).sqrMagnitude;
                if (minDis > tem)
                {
                    minDis = tem;
                    target = key.Value;
                    temUUID = key.Key;
                }
            }
            else
            {
                mRemoveList.Add(key.Key);
                //NetworkUnitManager.enemyUnitList.Remove(key.Key);
            }
        }
        for (int i = 0; i < mRemoveList.Count; i++)
        {
            NetworkUnitManager.enemyUnitList.Remove(mRemoveList[i]);
        }
        mTarget = target;
        mTargetUUID = temUUID;
        Debug.Log("���ο� Ÿ�� Ÿ��" + mTarget.mUnitScriptable.unitType);
    }

    #region Damage
    public override void GetDamage(int damage, string attackUnitUUID)
    {
        mPhotonView.RPC(nameof(GetDamageRPC), RpcTarget.Others, damage, attackUnitUUID);
    }

    [PunRPC]
    public void GetDamageRPC(int damage, string attackUnit)
    {
        if (damage <= 0) return;

        if (mCurrentHp <= damage)
        {
            mPhotonView.RPC(nameof(DieRPC), RpcTarget.Others);
            Die();
            return;
        }
        else mCurrentHp -= damage;
        //else HPbar.value = (mCurrentHp -= damage);

        Debug.Log("���� ���� : " + damage + ",  ���� ü�� : " + mCurrentHp);
        if (mTarget.mUnitScriptable.unitType == Scriptable.UnitType.Nexus)
        {
            if (NetworkUnitManager.enemyUnitList.ContainsKey(attackUnit))
            {
                if (NetworkUnitManager.enemyUnitList[attackUnit].IsAlive)
                {
                    mTarget = NetworkUnitManager.enemyUnitList[attackUnit];
                }
            }

        }
    }
    #endregion

    #region DIE
    public void Die()
    {
        int i;
        i = NetworkUnitManager.myUnitList.Count;
        Debug.Log("���� ��");
        foreach (var key in NetworkUnitManager.enemyUnitList)
        {
            if (key.Value.IsAlive)
            {
                Debug.Log("���� �� ����ִ� ����: " + key.Value.mUnitScriptable.unitType);
            }
            else
            {
                Debug.Log("���� �� ���� ����: " + key.Value.mUnitScriptable.unitType);
            }
        }
        NetworkUnitManager.myUnitList.Remove(this.mUnitScriptable.UUID);
        Debug.Log("���� ��");
        foreach (var key in NetworkUnitManager.enemyUnitList)
        {
            if (key.Value.IsAlive)
            {
                Debug.Log("���� �� ����ִ� ����: " + key.Value.mUnitScriptable.unitType);
            }
            else
            {
                Debug.Log("���� �� ���� ����: " + key.Value.mUnitScriptable.unitType);
            }
        }
        Debug.Log("���� ���� -> (���� �� myUnitList ���� : " + i + "���� �� :" + NetworkUnitManager.myUnitList.Count + ")");
        transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        IsAlive = false;
        mIsBatch = false;
        Destroy(gameObject);
    }

    [PunRPC]
    public void DieRPC()
    {
        int i;
        i = NetworkUnitManager.enemyUnitList.Count;
        transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        mIsBatch = false;
        IsAlive = false;
        NetworkUnitManager.enemyUnitList.Remove(this.mUnitScriptable.UUID);
        Debug.Log("���� ���� -> (���� �� enemyUnitList ���� : " + i + "���� �� :" + NetworkUnitManager.enemyUnitList.Count + ")");
        Destroy(gameObject);
    }
    #endregion
}
