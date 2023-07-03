using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;

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
    [SerializeField] private string targetUUID;
    [SerializeField] private float dis;
    [SerializeField] private float attackRANGE;
    [SerializeField] private bool IsLIVE;
    [SerializeField] private int hp;
    [SerializeField] private string _uuid;

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

    #region Animation string
    private const string MoveState = "IsMove";
    private const string NormalAttackState = "NormalAttack";
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
        //mNavMeshAgent.speed = mUnitScriptable.speed;
        mCurrentHp = mUnitScriptable.maxHP;

        IsAlive = true;
        mNavMeshAgent.enabled = true;
        gameObject.layer = GameManager.mMyUnitLayer;

        mIsBatch = true;
        mUnitUIController.Init(mUnitScriptable.maxHP, true);


        Findenemy();
        mNavMeshAgent.SetDestination(mTarget.transform.position);
        mUnitScriptable.UUID = MyUUIDGeneration.GenrateUUID();
        _uuid = mUnitScriptable.UUID;
        //NetworkUnitManager.myUnitList.Add(mUnitScriptable.UUID, this);
        mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others, mUnitScriptable.UUID);
        {
            attackRANGE = mUnitScriptable.attackRange;
            hp = mUnitScriptable.maxHP;
            NetworkUnitManager.AddmyUnit(mUnitScriptable.UUID, this);
            IsLIVE = true;
        }
    }

    [PunRPC]
    public void SyncInitBatch(string uuid) //���� ��ȯ�� ���� �ʱ�ȭ
    {
        //NetworkUnitManager.enemyUnitList.Add(uuid, this);
        {
            NetworkUnitManager.AddEnemyUnit(uuid, this);
            hp = mCurrentHp;
            IsLIVE = true;
        }
        mCurrentHp = mUnitScriptable.maxHP;
        mUnitUIController.Init(mUnitScriptable.maxHP, mPhotonView.IsMine);
        gameObject.layer = GameManager.mEnemyUnitLayer;
        mUnitScriptable.UUID = uuid;
        _uuid = uuid;
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
        mAttackDelay += Time.deltaTime;

        Debug.Log("Ÿ�� uiuid : " + mTarget.getUUID());

        if (mTarget != null) // Ÿ���� ���� ��
        {

            if (!NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID) || !mTarget.IsAlive) // Ÿ�� ��� Ȯ�� 
            {
                Debug.Log("Ÿ�� ����� Ÿ�� �� Ž��   :   "+ mTarget != null);
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
        dis = dist;
        transform.LookAt(mTarget.transform.position);
        if (dist <= mUnitScriptable.attackRange) // Ÿ���� ���� ���� ������ �������� -> �����ϰ� ����
        {
            //Debug.Log("����1?, �Ÿ� : " + dis + ",  Ÿ�� uiuid : " + mTarget.getUUID());
            mNavMeshAgent.avoidancePriority = mFightPriority;
            mNavMeshAgent.SetDestination(transform.position);

            if (mAttackDelay >= mUnitScriptable.attackSpeed)
            {
                Debug.Log("����2?");
                AttackType attackType = mAttack.Attack(getUUID(), mTarget.mUnitScriptable.UUID);
                mAttackDelay = 0;
                mPhotonView.RPC(nameof(mUnitAnimationController.PlayTriggerAnimation), RpcTarget.All, TypeToString(attackType));
            }
            mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, MoveState, false);
        }
        else//Ÿ���� ���� �������� �ֶ�
        {
            Debug.Log("�ƴ�, ����?");
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
                attackAnimName = NormalAttackState;
                break;
            case AttackType.Critical:
                attackAnimName = NormalAttackState;
                break;
            case AttackType.Skill:
                attackAnimName = SkillAttackState;
                break;
        }
        return attackAnimName;
    }

    private void NonTargetMove()
    {
        float dist = Vector3.Distance(mVectorDestination, transform.position);
        dis = dist;
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
            mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, MoveState, true);
            //Debug.Log("���ͷ� �̵� ��");
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mVectorDestination);
        }
    }

    public void PointMove(Vector3 pos)
    {
        mTarget = null;
        targetUUID = "Vector";
        mVectorDestination = pos;
        mNavMeshAgent.stoppingDistance = 0.15f;
        mNavMeshAgent.SetDestination(mVectorDestination);
    }
    #endregion

    private void Findenemy()
    {
        Debug.Log("�� Ž��...");
        //Debug.Log("�� ���� ���� : " + NetworkUnitManager.enemyUnitList.Count);
        float minDis = float.MaxValue;
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
        SetTarget(temUUID);
        mTargetUUID = temUUID;
        targetUUID = mTargetUUID;
        Debug.Log("���ο� Ÿ�� �߰� : " + mTarget.mUnitScriptable.UUID + ",     Ÿ�� : " + mTarget.mUnitScriptable.unitType);
    }

    void SetTarget(string uuid)
    {
        mTarget = NetworkUnitManager.enemyUnitList[uuid];
    }

    #region Damage
    public override void GetDamage(int damage, string attackUnitUUID , string attackedUnitUUID) //���� Ŭ�󿡼� ȣ��
    {
        Debug.Log("������ ==" + !mPhotonView.IsMine + "     getDamage() -> �ǰ� ���� :" + attackedUnitUUID);
        //Debug.Log("<������ ����> ����ü�� : " + mCurrentHp + " ������ : " + damage);
        if (mCurrentHp <= damage) mCurrentHp = 0;
        else mCurrentHp -= damage;
        {
            hp = mCurrentHp;
        }
        mUnitUIController.GetDamage(mCurrentHp);
        mPhotonView.RPC(nameof(GetDamageRPC), RpcTarget.Others, damage, attackUnitUUID, attackedUnitUUID);
    }

    [PunRPC]
    public void GetDamageRPC(int damage, string attackUnit, string attackedUnitUUID) // �ڽ��� Ŭ�󿡼� ȣ��
    {
        Debug.Log(mPhotonView.IsMine +"    getDamageRPC() :" + attackedUnitUUID);
        if (damage <= 0) return;

        if (mCurrentHp <= damage || mCurrentHp == 0)
        {
            //Die(attackedUnitUUID);
            //mPhotonView.RPC(nameof(DieRPC), RpcTarget.Others, attackedUnitUUID);
            Die(mUnitScriptable.UUID);
            mPhotonView.RPC(nameof(DieRPC), RpcTarget.Others, mUnitScriptable.UUID);
            return;
        }
        else mCurrentHp -= damage;
        {
            hp = mCurrentHp;
        }
        mUnitUIController.GetDamage(mCurrentHp);

        //Debug.Log("���� ���� : " + damage + ",  ���� ü�� : " + mCurrentHp);
        if (mTarget.mUnitScriptable.unitType == Scriptable.UnitType.Nexus)
        {
            if (NetworkUnitManager.enemyUnitList.ContainsKey(attackUnit))
            {
                if (!NetworkUnitManager.enemyUnitList[attackUnit].IsAlive) return;
                transform.LookAt(NetworkUnitManager.enemyUnitList[attackUnit].transform.position);
                mTarget = NetworkUnitManager.enemyUnitList[attackUnit];
                targetUUID = mTarget.getUUID();
            }
        }
    }
    #endregion

    #region DIE
    public void Die(string unit)//�ڽ��� Ŭ�󿡼� ȣ��
    {

        Debug.Log("Die() " + this.mUnitScriptable.UUID);
        {
            IsLIVE = false;
            NetworkUnitManager.RemoveMyUnit(unit);
        }
        //DieAnimation();

        mNavMeshAgent.enabled = false;

        mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, DieState, true);
        //NetworkUnitManager.myUnitList.Remove(this.mUnitScriptable.UUID);
        IsAlive = false;
        mIsBatch = false;
        Destroy(gameObject, 3f);
    }

    [PunRPC]
    public void DieRPC(string unit)//���� Ŭ�󿡼� ȣ��
    {
        Debug.Log("DieRPC() " + this.mUnitScriptable.UUID);       
        {
            IsLIVE = false;
            NetworkUnitManager.RemoveEnemyUnit(unit);
            mNavMeshAgent.enabled = false;
            IsAlive = false;
            mIsBatch = false;

        }
        Destroy(gameObject, 3f);
        // NetworkUnitManager.enemyUnitList.Remove(unit);
        //Debug.Log("���� ���� -> (���� �� enemyUnitList ���� : " + i + "���� �� :" + NetworkUnitManager.enemyUnitList.Count + ")");
    }
    #endregion
}
