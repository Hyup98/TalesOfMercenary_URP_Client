using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : Damageable
{
    #region Object info
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Slider HPbar;
    [SerializeField] private UnitAnimationController m_UnitAnimationController;

    private Animator mAnimator;
    private Transform mCachedTransfrom;
    private Damageable mTarget;
    private NavMeshAgent mNavMeshAgent;
    #endregion

    #region Stat Info
    [SerializeField] private int unitId;        //�ĺ���ȣ
    [Header("Stats")]
    [SerializeField] private int level;         //���� 
    private float attackDelay; // ���� �ӵ� ����
    #endregion

    #region logic Info
    private float defaultStoppingDistance;
    private bool mIsPointMove;
    private bool mIsCliked;
    private bool mIsAlive;
    private bool mIsBatch;
    private bool mIsMoving;

    private float mPointMoveDist = 0.3f;
    private bool mIsmoving;
    private int mPriority;
    private Vector3 mVectorDestination;
    private PhotonView mPhotonView;
    private Attackable mAttack;
    #endregion

    #region Property
    public bool IsAlive { get => mIsAlive; set => mIsAlive = value; }
    public bool IsClicked { get => mIsCliked; set => mIsCliked = value; }
    public bool IsEnemy { get; private set; }

    #endregion

    protected virtual void Awake()
    {
        mCachedTransfrom = transform;
        m_UnitAnimationController = GetComponent<UnitAnimationController>();
        mPhotonView = GetComponent<PhotonView>();
        mAttack = GetComponent<Attackable>();
        //InitBatch(photonView.IsMine);
    }

    public void Start()
    {
        mName = "����";
        mIsAlive = true;
        HPbar.maxValue = HPbar.value = mHp = mMaxHP;
        Debug.Log("��Ÿ��� : " + mAttackRange);
    }

    private void FixedUpdate()
    {
        if (!mIsBatch) return;
        if (!mPhotonView.IsMine) return;

        attackDelay += Time.deltaTime;

        if (mTarget != null) // Ÿ���� ���� ��
        {
            if(!mTarget.isAlive()) // Ÿ�� ��� Ȯ�� 
            {
                FindenemyOrNull(transform.position);
                return;
            }
            Debug.Log("Ÿ�� Ÿ�� : " + mTarget.mName + "���� �Ÿ�: " + Vector3.Distance(mTarget.transform.position, transform.position));
            if (Vector3.Distance(mTarget.transform.position, transform.position) <= mAttackRange) // Ÿ���� ���� ���� ������ �������� -> �����ϰ� ����
            {
                mNavMeshAgent.avoidancePriority = mPriority;
                mNavMeshAgent.SetDestination(transform.position);
                if (attackDelay >= mAttackSpeed) 
                {
                    Debug.Log("����");
                    mAttack.Attack(this, mTarget);
                    attackDelay = 0;
                }
            }
            else//Ÿ���� ���� �������� �ֶ�
            {
                Debug.Log("�� Ÿ������ �̵� ��");
                mIsmoving = true;
                mNavMeshAgent.SetDestination(mTarget.transform.position);
            }
        }
        else // Ÿ���� ���� �� -> ���ͷ� �̵� ��
        {
            Debug.Log("���� �Ÿ�: " + Vector3.Distance(mVectorDestination, transform.position));
            if (Vector3.Distance(mVectorDestination, transform.position) <= mAttackRange) // �������� ���� ��Ÿ� �� �϶�
            {
                FindenemyOrNull(transform.position);// ���ο� Ÿ�� Ž�� -> ���н� ��� ���� ���������� �̵�
                mNavMeshAgent.SetDestination(mVectorDestination);
            }
            else
            {
                Debug.Log("���ͷ� �̵� ��");
                mNavMeshAgent.SetDestination(mVectorDestination);
            }
            m_UnitAnimationController.PlayMoveAnimation(mIsMoving);
        }
    }

    public void InitBatch()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mPriority = mNavMeshAgent.avoidancePriority;
        HPbar.maxValue = HPbar.value = mHp = mMaxHP;

        mIsAlive = true;
        mNavMeshAgent.enabled = true;

        defaultStoppingDistance = mNavMeshAgent.stoppingDistance;
        mIsBatch = true;
        FindenemyOrNull(transform.position);
        mNavMeshAgent.SetDestination(mTarget.transform.position);

        NetworkUnitManager.myUnitList.Add(this);
        mPhotonView.RPC(nameof(SyncInitBatch), RpcTarget.Others);
    }

    [PunRPC]
    public void SyncInitBatch() //���� ��ȯ�� ���� �ʱ�ȭ
    {
        NetworkUnitManager.enemyUnitList.Add(this);
    }

    private void FindenemyOrNull(Vector3 destination) // ���� �������� ���� ��Ÿ��� �� Ž�� null��ȯ �� ���� ����
    {
        Debug.Log("���ο� ���ݴ�� �߰�(���Ǻ�)");
        float minDis = float.MaxValue;
        Damageable target = null;
        float tem;
        foreach (var key in NetworkUnitManager.enemyUnitList)
        {
            tem = (destination - key.transform.position).sqrMagnitude;
            if (minDis > tem)
            {
                minDis = tem;
                target = key;
            }
        }
        if (minDis <= mAttackRange) // ���ݴ�� ����
        {
            Debug.Log("���ο� ���ݴ�� �߰�");
            mTarget = target;
        }
    }

    public override void getDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        if (mHp <= damage)//���
        {
            Die();
        }
        else
        {
            HPbar.value = (mHp -= damage);
        }
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

    public void PointMove()
    {
        if (mNavMeshAgent.remainingDistance <= mNavMeshAgent.stoppingDistance)
        {
            mIsPointMove = false;
            mNavMeshAgent.stoppingDistance = defaultStoppingDistance;
        }
    }

    public void PointMove(Vector3 pos)
    {
        mTarget = null;
        mNavMeshAgent.enabled = true;
        mNavMeshAgent.stoppingDistance = 0.1f;
        mNavMeshAgent.SetDestination(mVectorDestination);
    }

}
