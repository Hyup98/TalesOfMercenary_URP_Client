using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit :  Damageable
{
    #region Object info
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Slider HPbar;
    [SerializeField] private UnitAnimationController m_UnitAnimationController;

    private Animator animator;
    private Transform cachedTransfrom;
    private NavMeshAgent navMeshAgent;
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
    [SerializeField] private float detectRange = 2.5f; // ��� ���� ���� �Ÿ�
    [SerializeField] private float attackSpeed = 1.5f; // ���� �ӵ�
    [SerializeField] private float attackDelay; // ���� �ӵ� ����

    //�Ӽ�
    [SerializeField] private EElement element;  // �Ӽ�
    #endregion

    #region logic Info
    private float defaultStoppingDistance;

    private bool isPlayer = false; // �ӽÿ�
    private bool isDetectEnemy;
    private bool isPointMove;
    private bool isCliked;
    private bool isAlive;
    private bool isBatch;
    private bool m_IsMoving;

    private Vector3 finalDestination;
    private Vector3 currentDestination;
    #endregion

    #region Property
    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public bool IsClicked { get => isCliked; set => isCliked = value; }
    public bool IsEnemy { get; private set; }
    public int OwnerID { get; set; }
    public int InstanceID { get; private set; }

    #endregion


    private int m_Priority;


    private void Awake()
    {
        cachedTransfrom = transform;
        m_UnitAnimationController = GetComponent<UnitAnimationController>();
    }

    public void Start()
    {
        isAlive = true;
        HPbar.maxValue = HPbar.value = currentHp = maxHP;
    }
    
    private void FixedUpdate()
    {
        if (!isBatch) return;
        attackDelay += Time.deltaTime;

        if (navMeshAgent.remainingDistance > attackRange)
        {
            navMeshAgent.isStopped = false;
            m_IsMoving = true;
            navMeshAgent.avoidancePriority = m_Priority;
        }
        else
        {
            navMeshAgent.isStopped = true;
            m_IsMoving = false;
            navMeshAgent.avoidancePriority = 0;
        }
        m_UnitAnimationController.PlayMoveAnimation(m_IsMoving);
    }

    //public void InitBatch()
    //{
    //    //Debug.Log("���� ��ġ �Ϸ�");
    //    navMeshAgent = GetComponent<NavMeshAgent>();

    //    HPbar.maxValue = HPbar.value = currentHp = maxHP;
    //    //����� �� ������ Start�Լ��� �ߺ��Ǵ� �κ� ����

    //    isBatch = true;
    //    isAlive = true;
    //    navMeshAgent.enabled = true;

    //    defaultStoppingDistance = navMeshAgent.stoppingDistance;

    //    //SetAffiliationUnit();
    //    currentDestination = finalDestination;

    //    SetDestination(finalDestination);
    //    //Find �Լ� ���� ������
    //}

    public void InitBatch(int ownerID, int instanceID, Vector3 finalDestination)
    {
        //Debug.Log("���� ��ġ �Ϸ�");
        navMeshAgent = GetComponent<NavMeshAgent>();
        m_Priority = navMeshAgent.avoidancePriority;
        HPbar.maxValue = HPbar.value = currentHp = maxHP;
        //����� �� ������ Start�Լ��� �ߺ��Ǵ� �κ� ����

        OwnerID = ownerID;
        InstanceID = instanceID;
        this.finalDestination = finalDestination;

        
        isAlive = true;
        navMeshAgent.enabled = true;
        
        defaultStoppingDistance = navMeshAgent.stoppingDistance;

        //SetAffiliationUnit();
        currentDestination = finalDestination;

        SetDestination(finalDestination);
        isBatch = true;
        //Find �Լ� ���� ������
    }

    //private void SetAffiliationUnit()
    //{
    //    if (OwnerID == NetworkManager.NetworkManagerSingleton.Client.Id)
    //    {
    //        HPbar.targetGraphic.color = Color.green;
    //        IsEnemy = false;
    //        gameObject.layer = 16;
    //    }
    //    else
    //    {
    //        HPbar.targetGraphic.color = Color.red;
    //        IsEnemy = true;
    //        gameObject.layer = 17;
    //    }

    //    if (NetworkManager.NetworkManagerSingleton.IsReversed) finalDestination = GameManager.player1Nexus;
    //    else finalDestination = GameManager.player2Nexus;
    //}

    #region ���� + Ž�� // �ڵ� ���� �ʿ���
    private void Attack(Unit enemy)
    {
        //Debug.Log(transform.name + "Attack");
        attackDelay = 0;
        //animator.SetBool("attack",true);
        enemy.Hit(str);
    }

    private void Attack(Nexus nexus)
    {
        attackDelay = 0;
        nexus.Hit(str);
    }

    private void Attack(NeutralUnit neutralUnit)
    {
        attackDelay = 0;
        neutralUnit.Hit(str);
    }
    #endregion

    public override void Hit(int damage)
    {
        //animator.SetBool("hit",true);
        HPbar.value = (currentHp -= damage);
    }

    public void Die()
    {
        HPbar.value = 0;
        isAlive = false;
        isBatch = false;

        Destroy(gameObject);
        //pool return
        //navMeshAgent.enabled = false;
    }

    public void SetDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }

    private void DefaultMove()
    {
        navMeshAgent.SetDestination(currentDestination);
        if (!isDetectEnemy) currentDestination = finalDestination;
    }

    public void PointMove()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            isPointMove = false;
            navMeshAgent.stoppingDistance = defaultStoppingDistance;
        }
    }

    public void PointMove(Vector3 pos)
    {
        //isPointMove = true;
        //navMeshAgent.stoppingDistance = 0;
        //navMeshAgent.SetDestination(pos);

        //NetworkUnitManager.SendDestinationInput(InstanceID, pos);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position, detectRange);

        Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(transform.position, attackRange);
    }
#endif
}
