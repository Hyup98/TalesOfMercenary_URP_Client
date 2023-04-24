using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    #region ����
    [SerializeField] protected int mMaxHP;         // �ִ� ü��
    protected int mHp;     // ���� ü��
    [SerializeField] protected int mDef;           // ����
    [SerializeField] protected int mMp;            // ����
    [SerializeField] protected int mStr;           // ���ݷ�
    [SerializeField] protected float mSpeed;         // �̵��ӵ�
    [Header("Additional stats")]
    [SerializeField] protected int mCriticalRate;    // ũ��Ƽ����
    [SerializeField] protected int mCriticalDamage;  // ũ��Ƽ�� ������
    [SerializeField] protected float mAttackRange = 0.8f; // ���� ��Ÿ�
    [SerializeField] protected float mAttackSpeed = 1.5f; // ���� �ӵ�
    [SerializeField] protected EElement mEelement;  
    private bool mIsAlive { get; set; }
    #endregion
    public string mName;

    public int getCriticalRate()
    {
        return mCriticalRate;
    }

    public bool isAlive()
    {
        return mIsAlive;
    }

    public int getDef()
    {
        return mDef;
    }
    public int getCriticalDamage()
    {
        return mCriticalDamage;
    }
    public int getHp()
    {
        return mHp;
    }

    public int getStr()
    {
        return mStr;
    }

    public abstract void getDamage(int damage);
}
