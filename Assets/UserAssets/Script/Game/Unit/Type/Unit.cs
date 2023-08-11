using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

/// <summary>
/// target�� �ٲ𶧸��� settarget()�Լ��� ȣ���ؼ� ��� �������� Ÿ�� ������ ����ȭ����� �Ѵ�.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Unit : Damageable
{
    #region Object info
    [SerializeField] private UnitUIController mUnitUIController;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private string mTargetUUID;
    [SerializeField] private float dis;
    [SerializeField] private bool IsLIVE;
    [SerializeField] private int hp;
    [SerializeField] private string mUUID;

    private EElement mtargetElement;
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
    #endregion

    #region Property
    public bool IsClicked { get; set; }
    public bool IsEnemy { get; private set; }

    public override string getUUID()
    {
        return mUUID;
    }
    #endregion

    #region Animation string
    private const string MoveState = "IsMove";
    private const string NormalAttackState = "NormalAttack";
    private const string SkillAttackState = "SkillAttack";
    private const string DieState = "IsDie";
    private const string HitState = "Hit";
    #endregion

    #region Init & Awake
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
        mUUID = MyUUIDGeneration.GenrateUUID();
        mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others, mUUID);
        {
            hp = mUnitScriptable.maxHP;
            NetworkUnitManager.AddmyUnit(mUUID, this);
            IsLIVE = true;
        }
    }

    [PunRPC]
    public void SyncInitBatch(string uuid) //���� ��ȯ�� ���� �ʱ�ȭ
    {
        {
            NetworkUnitManager.AddEnemyUnit(uuid, this);
            hp = mCurrentHp;
            IsLIVE = true;
        }
        mCurrentHp = mUnitScriptable.maxHP;
        mUnitUIController.Init(mUnitScriptable.maxHP, mPhotonView.IsMine);
        gameObject.layer = GameManager.mEnemyUnitLayer;
        mUUID = uuid;
        IsAlive = true;
    }
    #endregion

    private void FixedUpdate()
    {
        if (!mIsBatch) return;
        if (!mPhotonView.IsMine) return;
        mAttackDelay += Time.deltaTime;

        if (!NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID))
        {
            mTarget = null;
            mTargetUUID = "";
        }

        if (mTarget != null) // Ÿ���� ���� ��
        {

            if (!NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID) || !mTarget.IsAlive) // Ÿ�� ��� Ȯ�� 
            {
                Debug.Log("Ÿ�� ����� Ÿ�� �� Ž��   :   " + mTarget != null);
                Findenemy();
                return;
            }
            TargetMove();
        }
        else NonTargetMove();
    }
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
                    mtargetElement = key.Value.mUnitScriptable.element;
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
        Debug.Log("���ο� Ÿ�� �߰� : " + mTarget.mUnitScriptable.UUID + ",     Ÿ�� : " + mTarget.mUnitScriptable.unitType);
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

    void SetTarget(string uuid)
    {
        mTarget = NetworkUnitManager.enemyUnitList[uuid];
    }

    #region move
    private void TargetMove()
    {
        float dist = Vector3.Distance(mTarget.transform.position, transform.position);
        dis = dist;
        transform.LookAt(mTarget.transform.position);
        if (dist <= mUnitScriptable.attackRange) // Ÿ���� ���� ���� ������ �������� -> �����ϰ� ����
        {
            mNavMeshAgent.avoidancePriority = mFightPriority;
            mNavMeshAgent.SetDestination(transform.position);

            if (mAttackDelay >= mUnitScriptable.attackSpeed)
            {
                AttackType attackType = mAttack.Attack(getUUID(), mUnitScriptable.element, mTargetUUID, mtargetElement);
                mAttackDelay = 0;
                mPhotonView.RPC(nameof(mUnitAnimationController.PlayTriggerAnimation), RpcTarget.All, TypeToString(attackType));
            }
            mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, MoveState, false);
        }
        else//Ÿ���� ���� �������� �ֶ�
        {
            mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, MoveState, true);
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
    }

    private void NonTargetMove()
    {
        if (mTargetUUID == "")
        {
            Findenemy();
            return;
        }
        float dist = Vector3.Distance(mVectorDestination, transform.position);
        dis = dist;
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
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mVectorDestination);
        }
    }

    public void PointMove(Vector3 pos)
    {
        mTarget = null;
        mTargetUUID = "Vector";
        mVectorDestination = pos;
        mNavMeshAgent.stoppingDistance = 0.15f;
        mNavMeshAgent.SetDestination(mVectorDestination);
    }
    #endregion


    #region Damage
    public override void GetDamage(int damage, string attackUnitUUID, string attackedUnitUUID) //���� Ŭ�󿡼� ȣ��
    {
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
        if (mCurrentHp <= damage)
        {
            Die(mUUID);
            mPhotonView.RPC(nameof(DieRPC), RpcTarget.Others, mUUID);
            return;
        }
        else mCurrentHp -= damage;
        {
            hp = mCurrentHp;
        }
        mUnitUIController.GetDamage(mCurrentHp);

        Debug.Log(mTarget.mUnitScriptable.unitType);

        if (mTarget.mUnitScriptable.unitType == Scriptable.UnitType.Nexus)
        {
            if (NetworkUnitManager.enemyUnitList.ContainsKey(attackUnit))
            {
                Debug.Log("�ؼ��� ���� �� �ǰ�");
                if (!NetworkUnitManager.enemyUnitList[attackUnit].IsAlive) return;
                transform.LookAt(NetworkUnitManager.enemyUnitList[attackUnit].transform.position);
                mTarget = NetworkUnitManager.enemyUnitList[attackUnit];
                mTargetUUID = attackUnit;
            }
        }
    }
    #endregion

    #region DIE
    public void Die(string unit)//�ڽ��� Ŭ�󿡼� ȣ��
    {

        Debug.Log("Die() " + this.mUUID);
        {
            IsLIVE = false;
            NetworkUnitManager.RemoveMyUnit(unit);
            mNavMeshAgent.enabled = false;
            IsAlive = false;
            mIsBatch = false;
        }
        //DieAnimation();
        mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, DieState, true);

        Destroy(gameObject, 3f);
    }

    [PunRPC]
    public void DieRPC(string unit)//���� Ŭ�󿡼� ȣ��
    {
        Debug.Log("DieRPC() " + this.mUUID);
        {
            IsLIVE = false;
            NetworkUnitManager.RemoveEnemyUnit(unit);
            //mNavMeshAgent.enabled = false;
            IsAlive = false;
            mIsBatch = false;
            Destroy(gameObject, 3f);
        }
    }
    #endregion
}
