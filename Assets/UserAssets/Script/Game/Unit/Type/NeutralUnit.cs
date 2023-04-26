using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Photon.Pun;
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

                NetworkUnitManager.myUnitList.Add(this);
                mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others);
                Findenemy();
                mPriority = mNavMeshAgent.avoidancePriority;

            }
            return;
        }

        mAttackDelay += Time.deltaTime;

        //attack
        {
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
        Debug.Log("Ÿ�� Ÿ�� : " + mTarget.mUnitScriptable.unitName + "���� �Ÿ�: " + dist);
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

    public override void GetDamage(int damage, Damageable attackUnit)
    {
        if (!IsAlive) return;
        if (Hp <= 0)
        {
            Die();
            return;
        }
        HPbar.value = Hp;
        if(mTarget.mUnitScriptable.unitType == Scriptable.UnitType.Nexus)
        {
            if(attackUnit.IsAlive)
            {
                mTarget = attackUnit;
            }
        }
    }

    [PunRPC]
    public void SyncInitBatch() //���� ��ȯ�� ���� �ʱ�ȭ
    {
        NetworkUnitManager.enemyUnitList.Add(this);
        IsAlive = true;
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
            tem = (transform.position - key.transform.position).sqrMagnitude;
            if (minDis > tem)
            {
                minDis = tem;
                target = key;
            }
        }
        mTarget = target;
    }
}
