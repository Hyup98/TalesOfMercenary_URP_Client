using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �Ӽ� ����� ���� ������ �������� 
/// <������ ���� ����>
/// ���ݷ� - ����
/// 
/// <ũ��Ƽ�� ���� ����>
/// ���ݷ� * ġ��Ÿ������ - ����
/// </summary>
public class MeleeAttack : Attackable
{
    private System.Random mRand = new System.Random();

    public override void Attack(Damageable attackUnit, Damageable attackedUnit)
    {
        if (attackUnit.getCriticalRate() >= mRand.Next(101))//ũ��Ƽ�� �߻� ��
        {
            CriticalAttack(attackUnit, attackedUnit);
        }
        NormalAttack(attackUnit, attackedUnit);
    }

    public override void NormalAttack(Damageable attackUnit, Damageable attackedUnit)
    {
        attackedUnit.getDamage(attackUnit.getStr() - attackedUnit.getDef());
    }

    public override void CriticalAttack(Damageable attackUnit, Damageable attackedUnit)
    {
        attackedUnit.getDamage(attackUnit.getStr() * attackedUnit.getCriticalDamage() - attackedUnit.getDef());
    }
}
