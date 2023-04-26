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

    private UnitAnimationController m_UnitAnimationController;
    private Damageable mTarget;
    private NavMeshAgent mNavMeshAgent;
    #endregion

    #region logic Info
    private bool mIsAlive;
    private bool mIsBatch;
    private bool mIsMoving = true;

    private float attackDelay; // ���� �ӵ� ����
    private int mPriority;

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
        m_UnitAnimationController = GetComponent<UnitAnimationController>();
        mPhotonView = GetComponent<PhotonView>();
        mAttack = GetComponent<Attackable>();
    }

    public void InitBatch()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mPriority = mNavMeshAgent.avoidancePriority;
        HPbar.maxValue = HPbar.value = Hp = mUnitScriptable.maxHP;

        mIsAlive = true;
        mNavMeshAgent.enabled = true;

        mIsBatch = true;
        FindenemyOrNull();
        mNavMeshAgent.SetDestination(mTarget.transform.position);

        NetworkUnitManager.myUnitList.Add(this);
        mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others);
    }

    [PunRPC]
    public void SyncInitBatch() //���� ��ȯ�� ���� �ʱ�ȭ
    {
        NetworkUnitManager.enemyUnitList.Add(this);
    }

    private void FixedUpdate()
    {
        if (!mIsBatch) return;
        if (!mPhotonView.IsMine) return;

        attackDelay += Time.deltaTime;
        
        if (mTarget != null) // Ÿ���� ���� ��
        {
            if(!mTarget.IsAlive) // Ÿ�� ��� Ȯ�� 
            {
                FindenemyOrNull();
                return;
            }
            TargetMove();
        }
        else NonTargetMove();
        m_UnitAnimationController.PlayMoveAnimation(mIsMoving);
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
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(transform.position);
            if (attackDelay >= mUnitScriptable.attackSpeed)
            {
                Debug.Log("����");
                mAttack.Attack(this, mTarget);
                attackDelay = 0;
            }
        }
        else//Ÿ���� ���� �������� �ֶ�
        {
            Debug.Log("�� Ÿ������ �̵� ��");
            mIsMoving = true;
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
        if (dist <= mUnitScriptable.attackRange) // �������� ���� ��Ÿ� �� �϶�
        {
            FindenemyOrNull();// ���ο� Ÿ�� Ž�� -> ���н� ��� ���� ���������� �̵�
            mNavMeshAgent.SetDestination(mVectorDestination);
        }
        else
        {
            Debug.Log("���ͷ� �̵� ��");
            mNavMeshAgent.SetDestination(mVectorDestination);
        }
    }

    private void FindenemyOrNull() // ���� �������� ���� ��Ÿ��� �� Ž�� null��ȯ �� ���� ����
    {
        Debug.Log("���ο� ���ݴ�� �߰�(���Ǻ�)");
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
        if (minDis <= mUnitScriptable.attackRange) // ���ݴ�� ����
        {
            Debug.Log("���ο� ���ݴ�� �߰�");
            mTarget = target;
        }
    }

    public override void GetDamage(int damage)
    {
        if (damage <= 0) return;
        if (Hp <= damage) Die();
        else HPbar.value = (Hp -= damage);
    }

    public void Die()
    {
        HPbar.value = 0;
        mIsAlive = false;
        mIsBatch = false;

        Destroy(gameObject);
        //pool return
        //navMeshAgent.enabled = false;
    }

    public void PointMove(Vector3 pos)
    {
        mTarget = null;
        mNavMeshAgent.enabled = true;
        mNavMeshAgent.stoppingDistance = 0.1f;
        mNavMeshAgent.SetDestination(mVectorDestination);
    }
}
