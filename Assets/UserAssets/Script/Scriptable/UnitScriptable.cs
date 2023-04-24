using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable/UnitData", order = 0)]
    public class UnitScriptable : ScriptableObject
    {
        [Header("Info")]
        [Tooltip("���� ����")]
        public int level;         //���� ����

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
        public float criticalRate = 0;    // ũ��Ƽ����
        [Tooltip("ũ��Ƽ�� ������")]
        public float criticalDamage = 0;  // ũ��Ƽ�� ������
        [Tooltip("���� ��Ÿ�")]
        public float attackRange = 0.8f; // ���� ��Ÿ�
        [Tooltip("���� �ӵ�")]
        public float attackSpeed = 1.5f; // ���� �ӵ�
    }
}
