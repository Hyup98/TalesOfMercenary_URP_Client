using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    public enum UnitType
    {
        Nexus,
        Normal,
        Neutral
    }
    [CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable/UnitData", order = int.MaxValue)]
    public class UnitScriptable : ScriptableObject
    {
        [Header("Info")]
        [Tooltip("현재 레벨")]  
        public int level = 1;                   //현재 레벨
        [Tooltip("이름")]
        public string unitName;

        [Header("Stats")]
        [Tooltip("최대체력")]
        public int maxHP;                       // 최대 체력
        [Tooltip("방어력")]
        public int def;                         // 방어력
        [Tooltip("마나")] 
        public int mp;                          // 마나
        [Tooltip("공격력")]
        public int str;                         // 공격력
        [Tooltip("이동속도")]
        public float speed;                     // 이동속도

        [Tooltip("속성")]
        public EElement element;                // 속성

        [Header("Additional Stats")]
        [Tooltip("크리티컬율")]
        public int criticalRate = 0;            //크리티컬율
        [Tooltip("크리티컬 데미지")]
        public int criticalDamage = 0;          // 크리티컬 데미지
        [Tooltip("공격 사거리")]
        public float attackRange = 0.8f;        // 공격 사거리
        [Tooltip("공격 속도")]
        public float attackSpeed = 1.5f;        // 공격 속도
        [Tooltip("도착 거리")]
        public float movementRange = 0.2f;
        [Tooltip("유닛 타입")]
        public UnitType unitType;
        [Tooltip("유닛 인스턴스ID")]
        public string UUID;
        [Tooltip("유닛 인스턴스ID")]
        public int skillDamage;                 //스킬 데미지
    }
}
