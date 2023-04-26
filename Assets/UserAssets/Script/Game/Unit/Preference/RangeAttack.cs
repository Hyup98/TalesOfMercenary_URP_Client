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
        //ũ��Ƽ�� �߻� ��
        if (attackUnit.mUnitScriptable.criticalRate >= mRand.Next(101))
            CriticalAttack(attackUnit, attackedUnit);
        
        NormalAttack(attackUnit, attackedUnit);
    }
    //�Ӽ� �߰����ش� ���� ����
    public override void NormalAttack(Damageable attackUnit, Damageable attackedUnit)
        => attackedUnit.GetDamage(attackUnit.mUnitScriptable.str - attackedUnit.mUnitScriptable.def);

    public override void CriticalAttack(Damageable attackUnit, Damageable attackedUnit)
        => attackedUnit.GetDamage(attackUnit.mUnitScriptable.str * attackedUnit.mUnitScriptable.criticalDamage - attackedUnit.mUnitScriptable.def);
}
