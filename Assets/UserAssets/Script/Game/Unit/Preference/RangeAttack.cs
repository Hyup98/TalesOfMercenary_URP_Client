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
    private System.Random mRand = new System.Random();

    private float SkillProbability = 20;

    public override AttackType Attack(Damageable attackUnit, Damageable attackedUnit)
    {
        Debug.Log(attackUnit.getUUID() + " : ���� ,      " + attackedUnit.getUUID()+"   :�ǰ�");
        AttackType attackType = AttackType.Normal;
        if (attackedUnit.mUnitScriptable.level == 3)
        {
            if (SkillProbability > UnityEngine.Random.Range(0, 100))
            {
                //Debug.Log("��ų �ߵ�");
                attackType = AttackType.Skill;
                //attackedUnit.SkillAttackAnimation();
                attackUnit.transform.LookAt(attackedUnit.transform.position);
                SpecialMove(attackUnit, attackedUnit);
            }
            else Attack(attackUnit, attackedUnit, ref attackType);
        }
        else Attack(attackUnit, attackedUnit, ref attackType);

        return attackType;
    }

    private void Attack(Damageable attackUnit, Damageable attackedUnit, ref AttackType attackType)
    {
        if (attackUnit.mUnitScriptable.criticalRate > UnityEngine.Random.Range(0, 100))
        {
            //Debug.Log("ũ��Ƽ�ð���");
            attackType = AttackType.Critical;
            //attackUnit.CriticalAttackAnimation();
            attackUnit.transform.LookAt(attackedUnit.transform.position);
            CriticalAttack(attackUnit, attackedUnit);
        }
        else
        {
            //Debug.Log("�Ϲݰ���");
            attackType = AttackType.Normal;
            //attackUnit.NormalAttackAnimation();
            attackUnit.transform.LookAt(attackedUnit.transform.position);
            NormalAttack(attackUnit, attackedUnit);
        }
    }

    public override void NormalAttack(Damageable attackUnit, Damageable attackedUnit)
        => attackedUnit.GetDamage(attackUnit.mUnitScriptable.str - attackedUnit.mUnitScriptable.def, attackUnit.mUnitScriptable.UUID, attackedUnit.mUnitScriptable.UUID);


    public override void CriticalAttack(Damageable attackUnit, Damageable attackedUnit)
        => attackedUnit.GetDamage(attackUnit.mUnitScriptable.str * attackedUnit.mUnitScriptable.criticalDamage - attackedUnit.mUnitScriptable.def, attackUnit.mUnitScriptable.UUID, attackedUnit.mUnitScriptable.UUID);

    public override void SpecialMove(Damageable attackUnit, Damageable attackedUnit)
        => attackedUnit.GetDamage(attackUnit.mUnitScriptable.skillDamage - attackedUnit.mUnitScriptable.def, attackUnit.mUnitScriptable.UUID, attackedUnit.mUnitScriptable.UUID);
}
