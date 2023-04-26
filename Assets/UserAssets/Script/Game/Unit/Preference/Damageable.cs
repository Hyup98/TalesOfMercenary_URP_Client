using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Damageable : MonoBehaviour
{
    [SerializeField] public Scriptable.UnitScriptable mUnitScriptable;
    [SerializeField] protected Slider HPbar;
    public int Hp { get; protected set; } //���� ü��
    public bool IsAlive { get; protected set; }

    public abstract void GetDamage(int damage);
}
