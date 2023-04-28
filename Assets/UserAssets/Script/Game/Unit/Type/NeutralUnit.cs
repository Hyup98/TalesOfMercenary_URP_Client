using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Photon.Pun;
using Scriptable;
/// <summary>
/// ���� ����
/// 
/// </summary>
public class NeutralUnit : Damageable
{
    private Animator animator;
    private NavMeshAgent mNavMeshAgent;
    private Damageable mTarget;
    private PhotonView mPhotonView;
    private Attackable mAttack;

    #region logic Info
    private Vector3 destPos;

    private const int mFightPriority = 5;
    private int mPriority;

    private float initTime = 3.0f;
    private float mAttackDelay;

    private bool mIsBatch;
    private bool mIsMoving = true;
    private bool doAttackHost;

    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mPhotonView = GetComponent<PhotonView>();
    }

    public void Init(Vector3 spawnPos)
    {
        mUnitScriptable.UUID = MyUUIDGeneration.GenrateUUID();
        mIsBatch = true;
        Hp = mUnitScriptable.maxHP;
        mAttack = GetComponent<Attackable>();
        destPos = spawnPos;
    }

    public void Update()
    {
        if (!mPhotonView.IsMine) return;
        if (!IsAlive)
        {
            transform.position = Vector3.MoveTowards(transform.position, destPos, Time.deltaTime * initTime);
            if (transform.position == destPos)
            {
                IsAlive = true; // �� ������ ���� ������ ����
                mNavMeshAgent.enabled = true;
                animator.SetBool("isIdle", true);
                NetworkUnitManager.myUnitList.Add(mUnitScriptable.UUID, this);
                mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others, mUnitScriptable.UUID);
                Findenemy();
                mPriority = mNavMeshAgent.avoidancePriority;
            }
            return;
        }

        mAttackDelay += Time.deltaTime;

        //attack
        {
            if (mTarget == null)
            {
                Debug.Log("Ÿ���� ���̴�.");
            }
            else
            {
                Debug.Log("�� Ÿ�� ���� : " + mTarget.mUnitScriptable.unitType);
            }

            if (mTarget == null || !mTarget.IsAlive) // Ÿ�� ��� Ȯ�� 
            {
                Debug.Log("Ÿ�� ����� Ÿ�� �� Ž��");
                Findenemy();
                return;
            }
            TargetMove();
        }
    }

    private void Die()
    {
        IsAlive = false;
    }

    private void TargetMove()
    {
        float dist = Vector3.Distance(mTarget.transform.position, transform.position);
        //Debug.Log("Ÿ�� Ÿ�� : " + mTarget.mUnitScriptable.unitName + "���� �Ÿ�: " + dist);
        if (dist <= mUnitScriptable.attackRange) // Ÿ���� ���� ���� ������ �������� -> �����ϰ� ����
        {
            mIsMoving = false;
            mNavMeshAgent.avoidancePriority = mFightPriority;
            mNavMeshAgent.SetDestination(transform.position);
            if (mAttackDelay >= mUnitScriptable.attackSpeed)
            {
                Debug.Log("����");
                mAttack.Attack(this, mTarget);
                mAttackDelay = 0;
            }
        }
        else//Ÿ���� ���� �������� �ֶ�
        {
            Debug.Log("�� Ÿ������ �̵� ��");
            mIsMoving = true;
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
    }

    [PunRPC]
    public void GetDamageRPC(int damage, string attackUnit)
    {
        Debug.Log("�巡�� ���ݴ��� ���� ���� id: " + attackUnit);
        //if (damage <= 0) return;
        if (Hp <= damage) Die();
        else HPbar.value = (Hp -= damage);
        if (mTarget.mUnitScriptable.unitType == Scriptable.UnitType.Nexus)
        {
            if (NetworkUnitManager.enemyUnitList[attackUnit].IsAlive)
            {
                mTarget = NetworkUnitManager.enemyUnitList[attackUnit];
            }
        }
    }

    [PunRPC]
    public void SyncInitBatch(string UUID) //���� ��ȯ�� ���� �ʱ�ȭ
    {
        NetworkUnitManager.enemyUnitList.Add(UUID, this);
        mUnitScriptable.UUID = UUID;
        IsAlive = true;
    }

    public override void GetDamage(int damage, string attackUnitUUID)
    {
        mPhotonView.RPC(nameof(GetDamageRPC), RpcTarget.Others, damage, attackUnitUUID);
    }

    private void Findenemy() // ���� �������� ���� ��Ÿ��� �� Ž�� null��ȯ �� ���� ����
    {
        Debug.Log("���ο� ���ݴ�� �߰�(���Ǻ�)");
        Debug.Log(NetworkUnitManager.enemyUnitList.Count);
        float minDis = float.MaxValue;
        Damageable target = null;
        float tem;
        foreach (var key in NetworkUnitManager.enemyUnitList)
        {
            tem = (transform.position - key.Value.transform.position).sqrMagnitude;
            if (minDis > tem)
            {
                minDis = tem;
                target = key.Value;
            }
        }
        mTarget = target;
    }
}
