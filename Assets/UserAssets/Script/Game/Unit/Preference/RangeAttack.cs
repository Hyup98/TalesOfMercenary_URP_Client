using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ���� �Ӽ� ����� ���� ������ �������� 
/// <������ ���� ����>
/// ���ݷ� - ����
/// 
/// <ũ��Ƽ�� ���� ����>
/// ���ݷ� * ġ��Ÿ������ - ����
/// </summary>
public class RangeAttack : Attackable
{
    private static System.Random mRand = new System.Random();

    public override void Attack(Damageable attackUnit, Damageable attackedUnit)
    {
        if (attackUnit.getCriticalRate() >= mRand.Next(101))//ũ��Ƽ�� �߻� ��
        {
            CriticalAttack(attackUnit, attackedUnit);
        }
        NormalAttack(attackUnit, attackedUnit);
    }
    //�Ӽ� �߰����ش� ���� ����
    public override void NormalAttack(Damageable attackUnit, Damageable attackedUnit)
    {
        attackedUnit.getDamage(attackUnit.getStr() - attackedUnit.getDef());
    }
    public override void CriticalAttack(Damageable attackUnit, Damageable attackedUnit)
    {
        attackedUnit.getDamage(attackUnit.getStr() * attackedUnit.getCriticalDamage() - attackedUnit.getDef());
    }
}
