using UnityEngine;

/// <summary>
/// ���� �Ӽ� ����� ���� ������ �������� 
/// <������ ���� ����>
/// ���ݷ� - ����
/// 
/// <ũ��Ƽ�� ���� ����>
/// ���ݷ� * ġ��Ÿ������ - ����
/// </summary>
/// 


public class MeleeAttack : Attackable
{
    private System.Random mRand = new System.Random();
    private float SkillProbability = 20;

    public override AttackType Attack(string attackUnit, string attackedUnit)
    {
        Debug.Log(attackUnit + ": ���� ,      " + attackedUnit + ": �ǰ�");
        AttackType attackType = AttackType.Normal;
        if (NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.level == 3)
        {
            if (SkillProbability > UnityEngine.Random.Range(0, 100))
            {
                Debug.Log("��ų �ߵ�");
                attackType = AttackType.Skill;
                //attackedUnit.SkillAttackAnimation();
                NetworkUnitManager.myUnitList[attackUnit].transform.LookAt(NetworkUnitManager.enemyUnitList[attackedUnit].transform.position);
                SpecialMove(attackUnit, attackedUnit);
            }
            else Attack(attackUnit, attackedUnit, ref attackType);
        }
        else Attack(attackUnit, attackedUnit, ref attackType);

        return attackType;
    }

    private void Attack(string attackUnit, string attackedUnit, ref AttackType attackType)
    {
        Debug.Log("�ǰ� ����: "+ attackedUnit + "  <--->   ���� ����: " + attackUnit);
        if (NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.criticalRate > UnityEngine.Random.Range(0, 100))
        {
            //Debug.Log("ũ��Ƽ�ð���");��
            attackType = AttackType.Critical;
            //attackUnit.CriticalAttackAnimation();
            NetworkUnitManager.myUnitList[attackUnit].transform.LookAt(NetworkUnitManager.enemyUnitList[attackedUnit].transform.position);
            CriticalAttack(attackUnit, attackedUnit);
        }
        else
        {
            //Debug.Log("�Ϲݰ���");
            attackType = AttackType.Critical;
            //attackUnit.NormalAttackAnimation();
            NetworkUnitManager.myUnitList[attackUnit].transform.LookAt(NetworkUnitManager.enemyUnitList[attackedUnit].transform.position);
            NormalAttack(attackUnit, attackedUnit);
        }
    }

    public override void NormalAttack(string attackUnit, string attackedUnit)
        => NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.str - NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def, NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.UUID, NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.UUID);

    public override void CriticalAttack(string attackUnit, string attackedUnit)
        => NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.str * NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.criticalDamage - NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def, NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.UUID, NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.UUID);

    public override void SpecialMove(string attackUnit, string attackedUnit)
        => NetworkUnitManager.enemyUnitList[attackedUnit].GetDamage(NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.skillDamage - NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.def, NetworkUnitManager.myUnitList[attackUnit].mUnitScriptable.UUID, NetworkUnitManager.enemyUnitList[attackedUnit].mUnitScriptable.UUID);
}
