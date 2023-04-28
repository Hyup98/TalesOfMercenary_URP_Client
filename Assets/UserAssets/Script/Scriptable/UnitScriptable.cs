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
        [Tooltip("���� ����")]
        public int level = 1;         //���� ����
        [Tooltip("�̸�")]
        public string unitName;

        [Header("Stats")]
        [Tooltip("�ִ�ü��")]
        public int maxHP;         // �ִ� ü��
        [Tooltip("����")]
        public int def;           // ����
        [Tooltip("����")]
        public int mp;            // ����
        [Tooltip("���ݷ�")]
        public int str;           // ���ݷ�
        [Tooltip("�̵��ӵ�")]
        public float speed;         // �̵��ӵ�

        [Tooltip("�Ӽ�")]
        public EElement element;  // �Ӽ�

        [Header("Additional Stats")]
        [Tooltip("ũ��Ƽ����")] 
        public int criticalRate = 0;    // ũ��Ƽ����
        [Tooltip("ũ��Ƽ�� ������")]
        public int criticalDamage = 0;  // ũ��Ƽ�� ������
        [Tooltip("���� ��Ÿ�")]
        public float attackRange = 0.8f; // ���� ��Ÿ�
        [Tooltip("���� �ӵ�")]
        public float attackSpeed = 1.5f; // ���� �ӵ�
        [Tooltip("���� �Ÿ�")]
        public float movementRange = 0.2f;
        [Tooltip("���� Ÿ��")]
        public UnitType unitType;
        public string UUID;
    }
}
