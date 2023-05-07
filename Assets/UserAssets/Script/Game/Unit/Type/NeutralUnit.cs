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
    private Animator animator;
    private NavMeshAgent mNavMeshAgent;
    private Damageable mTarget;
    private PhotonView mPhotonView;
    private Attackable mAttack;
    private string mTargetUUID;

    #region logic Info
    private Vector3 destPos;

    private const int mFightPriority = 5;
    private int mPriority;

    private float initTime = 3.0f;
    private float mAttackDelay;

    private bool mIsBatch;
    private bool mIsMoving = true;
    private bool doAttackHost;
    private List<string> mRemoveList = new List<string>();

    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
        destPos = spawnPos;
    }
    [PunRPC]
    public void SyncInit(string UUID) //���� ��ȯ�� ���� �ʱ�ȭ
    {
        NetworkUnitManager.enemyUnitList.Add(UUID, this);
        gameObject.layer = GameManager.mEnemyUnitLayer;
        mUnitScriptable.UUID = UUID;
        IsAlive = true;
    }
    #endregion

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
                NetworkUnitManager.myUnitList.Add(mUnitScriptable.UUID, this);
                mPhotonView.RPC(nameof(SyncInit), RpcTarget.Others, mUnitScriptable.UUID);
                Findenemy();
                mPriority = mNavMeshAgent.avoidancePriority;
            }
            return;
        }

        mAttackDelay += Time.deltaTime;

        //attack
        {
            if (mTarget == null || !mTarget.IsAlive || !NetworkUnitManager.enemyUnitList.ContainsKey(mTargetUUID)) // Ÿ�� ��� Ȯ�� 
            {
                Debug.Log("Ÿ�� ����� Ÿ�� �� Ž��");
                Findenemy();
                return;
            }
            TargetMove();
        }
    }

    #region DIE
    private void Die()
    {
        DieAnimation();
        IsAlive = false;
        mIsBatch = false;
        Debug.Log("�巡�� ���");
        NetworkUnitManager.myUnitList.Remove(this.mUnitScriptable.UUID);
        Destroy(gameObject);
    }

    [PunRPC]
    public void DieRPC()
    {
        Debug.Log("�巡�� ���");
        mIsBatch = false;
        IsAlive = false;
        NetworkUnitManager.enemyUnitList.Remove(this.mUnitScriptable.UUID);
        Destroy(gameObject);
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

        if (mTarget.mUnitScriptable.unitType == Scriptable.UnitType.Nexus)
        {
            if (NetworkUnitManager.enemyUnitList.ContainsKey(attackUnit))
            {
                if (NetworkUnitManager.enemyUnitList[attackUnit].IsAlive)
                {
                    mTarget = NetworkUnitManager.enemyUnitList[attackUnit];
                }
            }
        }
    }

    public override void GetDamage(int damage, string attackUnitUUID)
    {
        mPhotonView.RPC(nameof(GetDamageRPC), RpcTarget.Others, damage, attackUnitUUID);
    }
    #endregion

    private void TargetMove()
    {
        float dist = Vector3.Distance(mTarget.transform.position, transform.position);
        //Debug.Log("Ÿ�� Ÿ�� : " + mTarget.mUnitScriptable.unitName + "���� �Ÿ�: " + dist);
        if (dist <= mUnitScriptable.attackRange) // Ÿ���� ���� ���� ������ �������� -> �����ϰ� ����
        {
            mIsMoving = false;
            mNavMeshAgent.avoidancePriority = mFightPriority;
            mNavMeshAgent.SetDestination(transform.position);
            if (mAttackDelay >= mUnitScriptable.attackSpeed)
            {
                mAttack.Attack(this, mTarget);
                mAttackDelay = 0;
            }
            IdleAnimation();
        }
        else//Ÿ���� ���� �������� �ֶ�
        {
            Debug.Log("�� Ÿ������ �̵� ��");
            WalkAnimation();
            mIsMoving = true;
            mNavMeshAgent.avoidancePriority = mPriority;
            mNavMeshAgent.SetDestination(mTarget.transform.position);
        }
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

    #region Animation
    public override void IdleAnimation()
    {
        Debug.Log("IdleAnimation");
        mPhotonView.RPC(nameof(IdleAnimationRPC), RpcTarget.Others);
    }

    [PunRPC]
    public void IdleAnimationRPC()
    {
        Debug.Log("IdleAnimation");
    }

    public override void NormalAttackAnimation()
    {
        Debug.Log("NormalAttackAnimation");
        mPhotonView.RPC(nameof(NormalAttackAnimationRPC), RpcTarget.Others);
    }

    [PunRPC]
    public void NormalAttackAnimationRPC()
    {
        Debug.Log("NormalAttackAnimation");
    }

    public override void CriticalAttackAnimation()
    {
        Debug.Log("CriticalAttackAnimation");
        mPhotonView.RPC(nameof(CriticalAttackAnimationRPC), RpcTarget.Others);
    }

    [PunRPC]
    public void CriticalAttackAnimationRPC()
    {
        Debug.Log("CriticalAttackAnimation");

    }


    public override void SkillAttackAnimation()
    {
        Debug.Log("SkillAttackAnimation");
        mPhotonView.RPC(nameof(SkillAttackAnimationRPC), RpcTarget.Others);
    }

    [PunRPC]
    public void SkillAttackAnimationRPC()
    {
        Debug.Log("SkillAttackAnimation");

    }

    public void WalkAnimation()
    {
        Debug.Log("WalkAnimation");
        mPhotonView.RPC(nameof(WalkAnimationRPC), RpcTarget.Others);
    }


    [PunRPC]
    public void WalkAnimationRPC()
    {
        Debug.Log("WalkAnimation");
    }

    public override void DieAnimation()
    {
        Debug.Log("DieAnimation");
        mPhotonView.RPC(nameof(DieAnimationRPC), RpcTarget.Others);
    }

    [PunRPC]
    public void DieAnimationRPC()
    {
        Debug.Log("DieAnimation");
    }
    #endregion
}
