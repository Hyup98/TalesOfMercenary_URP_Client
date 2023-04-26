using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
        HPbar.maxValue = HPbar.value = Hp = mUnitScriptable.maxHP;

        IsAlive = true;
        mNavMeshAgent.enabled = true;

        mIsBatch = true;
        Findenemy();
        mNavMeshAgent.SetDestination(mTarget.transform.position);

        NetworkUnitManager.myUnitList.Add(this);
        mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others);
    }

    [PunRPC]
    public void SyncInitBatch() //���� ��ȯ�� ���� �ʱ�ȭ
    {
        NetworkUnitManager.enemyUnitList.Add(this);
        IsAlive = true;
    }

    private void FixedUpdate()
    {
        if (!mIsBatch) return;
        if (!mPhotonView.IsMine) return;

        mAttackDelay += Time.deltaTime;

        if (mTarget != null) // Ÿ���� ���� ��
        {
            if (!mTarget.IsAlive) // Ÿ�� ��� Ȯ�� 
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

    /// <summary>
    /// Ÿ���� �ְ� ������� ��
    /// </summary>
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

    /// <summary>
    /// Ÿ���� ���� �� -> ���ͷ� �̵� ��
    /// </summary>
    private void NonTargetMove()
    {
        float dist = Vector3.Distance(mVectorDestination, transform.position);
        Debug.Log("���� �Ÿ�: " + dist);
        mIsMoving = true;
        if (dist <= mUnitScriptable.movementRange) // �������� ���� ��Ÿ� �� �϶�
        {
            Findenemy();// ���ο� Ÿ�� Ž�� -> ���н� ��� ���� ���������� �̵�
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

    public override void GetDamage(int damage, Damageable attackUnit)
    {
        if (damage <= 0) return;
        if (Hp <= damage) Die();
        else HPbar.value = (Hp -= damage);
        if (mTarget.mUnitScriptable.unitType == Scriptable.UnitType.Nexus)
        {
            if (attackUnit.IsAlive)
            {
                mTarget = attackUnit;
            }
        }
    }

    public void Die()
    {
        HPbar.value = 0;
        IsAlive = false;
        mIsBatch = false;

        Destroy(gameObject);
        //pool return
        //navMeshAgent.enabled = false;
    }

    public void PointMove(Vector3 pos)
    {
        mTarget = null;
        mVectorDestination = pos;
        mNavMeshAgent.stoppingDistance = 0.15f;
        mNavMeshAgent.SetDestination(mVectorDestination);
    }
}
