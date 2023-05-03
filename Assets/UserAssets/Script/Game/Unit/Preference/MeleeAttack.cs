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

    private float SkillProbability = 20;

    public override void Attack(Damageable attackUnit, Damageable attackedUnit)
    {
        if (attackedUnit.mUnitScriptable.level == 3)
        {
            if (SkillProbability > UnityEngine.Random.Range(0, 100))
            {
                Debug.Log("��ų �ߵ�");
                CriticalAttack(attackUnit, attackedUnit);
            }
            else
            {
                if (attackUnit.mUnitScriptable.criticalRate > UnityEngine.Random.Range(0, 100))
                {
                    Debug.Log("ũ��Ƽ�ð���");
                    CriticalAttack(attackUnit, attackedUnit);
                }
                else
                {
                    Debug.Log("�Ϲݰ���");
                    NormalAttack(attackUnit, attackedUnit);
                }
            }
        }
        else
        {
            if (attackUnit.mUnitScriptable.criticalRate > UnityEngine.Random.Range(0, 100))
            {
                Debug.Log("ũ��Ƽ�ð���");
                CriticalAttack(attackUnit, attackedUnit);
            }
            else
            {
                Debug.Log("�Ϲݰ���");
                NormalAttack(attackUnit, attackedUnit);
            }
        }
    }

    public override void NormalAttack(Damageable attackUnit, Damageable attackedUnit)
    {
        attackedUnit.GetDamage(attackUnit.mUnitScriptable.str - attackedUnit.mUnitScriptable.def, attackUnit.mUnitScriptable.UUID);
    }

    public override void CriticalAttack(Damageable attackUnit, Damageable attackedUnit)
        => attackedUnit.GetDamage(attackUnit.mUnitScriptable.str * attackedUnit.mUnitScriptable.criticalDamage - attackedUnit.mUnitScriptable.def, attackUnit.mUnitScriptable.UUID);
}