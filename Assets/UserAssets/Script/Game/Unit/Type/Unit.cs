using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// target�� �ٲ𶧸��� settarget()�Լ��� ȣ���ؼ� ��� �������� Ÿ�� ������ ����ȭ����� �Ѵ�.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Unit : Damageable
{
    #region Object info
    [SerializeField] private UnitUIController mUnitUIController;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int unitId;        //�ĺ���ȣ

    private UnitAnimationController mUnitAnimationController;
    private Damageable mTarget;
    private NavMeshAgent mNavMeshAgent; 
    private PhotonView mPhotonView;
    private Attackable mAttack;
    private List<string> mRemoveList = new List<string>();

    #endregion

    #region logic Info
    private bool mIsBatch;

    private float mAttackDelay; // ���� �ӵ� ����
    private int mPriority;
    private const int mFightPriority = 5;

    private Vector3 mVectorDestination;

    private string mTargetUUID;
    
    #endregion

    #region Property
    public bool IsClicked { get; set; }
    public bool IsEnemy { get; private set; }
    #endregion

    #region Animation sting
    private const string MoveState = "IsMove";
    private const string AttackState = "Attack";
    private const string SkillAttackState = "SkillAttack";
    private const string DieState = "IsDie";
    private const string HitState = "Hit";
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
        mUnitUIController.Init(mUnitScriptable.maxHP);

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
        mUnitUIController.Init(mUnitScriptable.maxHP);
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
    }

    #region move
    private void TargetMove()
    {
        float dist = Vector3.Distance(mTarget.transform.position, transform.position);

        if (dist <= mUnitScriptable.attackRange) // Ÿ���� ���� ���� ������ �������� -> �����ϰ� ����
        {
            //Debug.Log("Ÿ�� Ÿ�� : " + mTarget.mUnitScriptable.unitName + " ���� �� ����");
            mNavMeshAgent.avoidancePriority = mFightPriority;
            mNavMeshAgent.SetDestination(transform.position);

            if (mAttackDelay >= mUnitScriptable.attackSpeed)
            {
                AttackType attackType = mAttack.Attack(this, mTarget);
                mAttackDelay = 0;
                mPhotonView.RPC(nameof(mUnitAnimationController.PlayTriggerAnimation),RpcTarget.All, TypeToString(attackType));
            }
            mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, MoveState, false);
        }
        else//Ÿ���� ���� �������� �ֶ�
        {
            //Debug.Log("Ÿ�� Ÿ�� : " + mTarget.mUnitScriptable.unitName + "���� �Ÿ�: " + dist);
            mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, MoveState, true);
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
    }

    private string TypeToString(AttackType attackType)
    {
        string attackAnimName = "";
        switch (attackType)
        {
            case AttackType.Normal:
                attackAnimName = AttackState;
                break;
            case AttackType.Critical:
                attackAnimName = AttackState;
                break;
            case AttackType.Skill:
                attackAnimName = SkillAttackState;
                break;
        }
        return attackAnimName;
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
        if (dist <= mUnitScriptable.movementRange) // �������� ���� ��Ÿ� �� �϶�
        {
            Findenemy();
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
        else
        {
            //WalkAnimation();
            mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation),RpcTarget.All,MoveState, true);
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
        //Debug.Log("<������ ����> ����ü�� : " + mCurrentHp + " ������ : " + damage);
        if (mCurrentHp <= damage) mCurrentHp = 0;
        else mCurrentHp -= damage;

        mUnitUIController.GetDamage(mCurrentHp);
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
        mUnitUIController.GetDamage(mCurrentHp);

        Debug.Log("���� ���� : " + damage + ",  ���� ü�� : " + mCurrentHp);
        if (mTarget.mUnitScriptable.unitType == Scriptable.UnitType.Nexus)
        {
            if (NetworkUnitManager.enemyUnitList.ContainsKey(attackUnit))
            {
                if (!NetworkUnitManager.enemyUnitList[attackUnit].IsAlive) return;
                mTarget = NetworkUnitManager.enemyUnitList[attackUnit];
            }
        }
    }
    #endregion

    #region DIE
    public void Die()
    {
        //DieAnimation();
        mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation),RpcTarget.All, DieState, true);
        NetworkUnitManager.myUnitList.Remove(this.mUnitScriptable.UUID);
        IsAlive = false;
        mIsBatch = false;
        Destroy(gameObject, 3f);
    }

    [PunRPC]
    public void DieRPC()
    {
        //int i = NetworkUnitManager.enemyUnitList.Count;
        mIsBatch = false;
        IsAlive = false;
        NetworkUnitManager.enemyUnitList.Remove(this.mUnitScriptable.UUID);
        //Debug.Log("���� ���� -> (���� �� enemyUnitList ���� : " + i + "���� �� :" + NetworkUnitManager.enemyUnitList.Count + ")");
        Destroy(gameObject, 3f);
    }
    #endregion
}
