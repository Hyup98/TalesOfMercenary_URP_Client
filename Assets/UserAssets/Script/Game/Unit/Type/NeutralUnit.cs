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
    [SerializeField] private UnitUIController mUnitUIController;

    private UnitAnimationController mUnitAnimationController;
    private NavMeshAgent mNavMeshAgent;
    private Damageable mTarget;
    private PhotonView mPhotonView;
    private Attackable mAttack;
    private List<string> mRemoveList = new List<string>();

    private Vector3 mDestPos;

    private string mTargetUUID;
    
    private int mPriority;

    private float mInitTime = 3.0f;
    private float mAttackDelay;

    private bool mIsBatch;
    private bool mDoAttackHost;

    private const int mFightPriority = 5;
    #region Animation sting
    private const string IdleState = "IsIdle";
    private const string MoveState = "IsMove";
    private const string NormalAttackState = "NormalAttack";
    private const string SkillAttackState = "SkillAttack";
    private const string DieState = "Die";
    #endregion
    private void Awake()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mPhotonView = GetComponent<PhotonView>();
    }

    #region �ʱ�ȭ
    public void Init(Vector3 spawnPos)
    {
        mUnitScriptable.UUID = MyUUIDGeneration.GenrateUUID();
        mIsBatch = true;
        mCurrentHp = mUnitScriptable.maxHP;
        gameObject.layer = GameManager.mMyUnitLayer;
        mAttack = GetComponent<Attackable>();
        mDestPos = spawnPos;
        mUnitUIController.Init(mCurrentHp, false);
    }

    private void Start()
    {
        if (mPhotonView.IsMine) StartCoroutine(LandingCoroutine(3));
    }

    [PunRPC]
    public void SyncInit(string UUID) //���� ��ȯ�� ���� �ʱ�ȭ
    {
        mCurrentHp = mUnitScriptable.maxHP;
        mUnitUIController.Init(mCurrentHp, mPhotonView.IsMine);
        NetworkUnitManager.enemyUnitList.Add(UUID, this);
        gameObject.layer = GameManager.mEnemyUnitLayer;
        mUnitScriptable.UUID = UUID;
        IsAlive = true;
    }
    #endregion

    public void Update()
    {
        if (!mPhotonView.IsMine) return;
        if (!IsAlive) return;

        mAttackDelay += Time.deltaTime;

        //attack
        if (mTarget == null || !mTarget.IsAlive || !NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID)) // Ÿ�� ��� Ȯ�� 
        {
            Debug.Log("Ÿ�� ����� Ÿ�� �� Ž��");
            Findenemy();
            return;
        }
        TargetMove();
    }

    private IEnumerator LandingCoroutine(float duration)
    {
        Vector3 startPos = transform.position;
        float currentTime = 0;
        while (currentTime <= duration)
        {
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, mDestPos, currentTime / duration);
            yield return null;
        }
        mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, IdleState, true);

        yield return new WaitForSeconds(0.3f);

        IsAlive = true; // �� ������ ���� ������ ����

        mNavMeshAgent.enabled = true;
        NetworkUnitManager.myUnitList.Add(mUnitScriptable.UUID, this);
        mUnitUIController.Init(mUnitScriptable.maxHP, false);
        mPhotonView.RPC(nameof(SyncInit), RpcTarget.Others, mUnitScriptable.UUID);
        mPriority = mNavMeshAgent.avoidancePriority;
        Findenemy();
    }

    #region DIE
    private void Die()
    {
        mPhotonView.RPC(nameof(mUnitAnimationController.PlayTriggerAnimation), RpcTarget.All, DieState);
        IsAlive = false;
        mIsBatch = false;
        Debug.Log("�巡�� ���");
        NetworkUnitManager.myUnitList.Remove(this.mUnitScriptable.UUID);
        Destroy(gameObject, 3);
    }

    [PunRPC]
    public void DieRPC()
    {
        Debug.Log("�巡�� ���");
        mIsBatch = false;
        IsAlive = false;
        NetworkUnitManager.enemyUnitList.Remove(this.mUnitScriptable.UUID);
        Destroy(gameObject, 3);
    }
    #endregion

    #region DAMAGE
    [PunRPC]
    public void GetDamageRPC(int damage, string attackUnit)
    {
        Debug.Log("�巡�� ���ݴ��� ���� ���� id: " + attackUnit);
        if (damage <= 0) return;

        if (mCurrentHp <= damage)
        {
            mPhotonView.RPC(nameof(DieRPC), RpcTarget.Others);
            Die();
            return;
        }
        else mCurrentHp -= damage;
        mUnitUIController.GetDamage(mCurrentHp);
        if (mTarget.mUnitScriptable.unitType == Scriptable.UnitType.Nexus)
        {
            if (NetworkUnitManager.enemyUnitList.ContainsKey(attackUnit))
            {
                if (!NetworkUnitManager.enemyUnitList[attackUnit].IsAlive) return;
                transform.LookAt(NetworkUnitManager.enemyUnitList[attackUnit].transform.position);
                mTarget = NetworkUnitManager.enemyUnitList[attackUnit];
            }
        }
    }

    public override void GetDamage(int damage, string attackUnitUUID)
    {
        if (mCurrentHp <= damage) mCurrentHp = 0;
        else mCurrentHp -= damage;

        mUnitUIController.GetDamage(mCurrentHp);
        mPhotonView.RPC(nameof(GetDamageRPC), RpcTarget.Others, damage, attackUnitUUID);
    }
    #endregion

    private void TargetMove()
    {
        float dist = Vector3.Distance(mTarget.transform.position, transform.position);
        //Debug.Log("Ÿ�� Ÿ�� : " + mTarget.mUnitScriptable.unitName + "���� �Ÿ�: " + dist);
        if (dist <= mUnitScriptable.attackRange) // Ÿ���� ���� ���� ������ �������� -> �����ϰ� ����
        {
            mNavMeshAgent.avoidancePriority = mFightPriority;
            mNavMeshAgent.SetDestination(transform.position);
            if (mAttackDelay >= mUnitScriptable.attackSpeed)
            {
                AttackType attackType = mAttack.Attack(this, mTarget);
                mAttackDelay = 0;
                mPhotonView.RPC(nameof(mUnitAnimationController.PlayTriggerAnimation), RpcTarget.All, TypeToString(attackType));
            }
            mPhotonView.RPC(nameof(mUnitAnimationController.PlayBoolAnimation), RpcTarget.All, MoveState, false);
            //IdleAnimation();
        }
        else//Ÿ���� ���� �������� �ֶ�
        {
            //Debug.Log("�� Ÿ������ �̵� ��");
            //WalkAnimation();
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

    private void Findenemy() // ���� �������� ���� ��Ÿ��� �� Ž�� null��ȯ �� ���� ����
    {
        Debug.Log("�巡�� �� Ž����..");
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

    public override string getUUID()
    {
        return mUnitScriptable.UUID;
    }
}
