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
    [SerializeField] private int level;         //���� ����

    //��
    [SerializeField] private int maxHP;         // �ִ� ü��
    private int currentHp;     // ���� ü��

    //����
    [SerializeField] private int def;           // ����
    [SerializeField] private int mp;            // ����
    [SerializeField] private int str;           // ���ݷ�
    [SerializeField] private float speed;         // �̵��ӵ�

    [Header("Additional stats")]
    [SerializeField] private float criticalRate;    // ũ��Ƽ����
    [SerializeField] private float criticalDamage;  // ũ��Ƽ�� ������
    [SerializeField] private float attackRange = 0.8f; // ���� ��Ÿ�
    [SerializeField] private float attackSpeed = 1.5f; // ���� �ӵ�

    [SerializeField] private EElement element;  // �Ӽ�



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
        //InitBatch(photonView.IsMine);
    }

    public void Start()
    {
        mIsAlive = true;
        HPbar.maxValue = HPbar.value = currentHp = maxHP;
    }

    private void FixedUpdate()
    {
        if (!mIsBatch) return;
        if (!mPhotonView.IsMine) return;


        attackDelay += Time.deltaTime;
        if (mTarget != null) // Ÿ���� ���� ��
        {
            if (Vector3.Distance(mVectorDestination, transform.position) <= attackRange) // Ÿ���� ���� ���� ������ ��������
            {
                mNavMeshAgent.avoidancePriority = mPriority;
                mIsmoving = false;
                mNavMeshAgent.isStopped = false;
            }
            else//Ÿ���� ���� �������� �ֶ�
            {
                mIsmoving = true;
                mNavMeshAgent.SetDestination(mTarget.transform.position);
            }
            m_UnitAnimationController.PlayMoveAnimation(mNavMeshAgent.enabled);
            return;
        }
        else // Ÿ���� ���� �� -> ���ͷ� �̵� ��
        {
            if (Vector3.Distance(mVectorDestination, transform.position) <= attackRange) // �������� ���� ��Ÿ� �� �϶�
            {
                if (Vector3.Distance(mVectorDestination, transform.position) <= 0.15f)//�������� ��
                {
                    mNavMeshAgent.stoppingDistance = attackRange;
                    Findenemy();
                    m_UnitAnimationController.PlayMoveAnimation(mIsMoving);
                    return;
                }
                FindenemyOrNull(mVectorDestination, attackRange);// ���ο� Ÿ�� Ž�� -> ���н� ��� ���� ���������� �̵�
                m_UnitAnimationController.PlayMoveAnimation(mIsMoving);
            }
        }
    }

    public void InitBatch()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mPriority = mNavMeshAgent.avoidancePriority;
        HPbar.maxValue = HPbar.value = currentHp = maxHP;

        mIsAlive = true;
        mNavMeshAgent.enabled = true;

        defaultStoppingDistance = mNavMeshAgent.stoppingDistance;
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
    }

    private void Findenemy() // �ݵ�� ���� ã�´�.thr
    {
        Debug.Log("���ο� ���ݴ�� �߰�(������)");
        float minDis = float.MaxValue;
        Damageable target = NetworkUnitManager.enemyUnitList[0]; // ����Ʈ �� �ؼ���
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
        mNavMeshAgent.enabled = true;
        mNavMeshAgent.SetDestination(mTarget.transform.position);
    }

    private void FindenemyOrNull(Vector3 destination, float attackrange) // ���� �������� ���� ��Ÿ��� �� Ž�� null��ȯ �� ���� ����
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
        if (minDis <= attackrange) // ���ݴ�� ����
        {
            Debug.Log("���ο� ���ݴ�� �߰�");
            mTarget = target;
            mNavMeshAgent.enabled = true;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
    }

    public override void Hit(int damage)
    {
        //animator.SetBool("hit",true);
        HPbar.value = (currentHp -= damage);
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
        mVectorDestination = pos;
        mTarget = null;
        mNavMeshAgent.enabled = true;
        mNavMeshAgent.stoppingDistance = 0.1f;
        mNavMeshAgent.SetDestination(pos);
    }
}
