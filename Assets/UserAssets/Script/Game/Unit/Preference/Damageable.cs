using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Damageable : MonoBehaviour
{

    [SerializeField] public Scriptable.UnitScriptable mUnitScriptable;
    public int mCurrentHp { get; protected set; } //���� ü��

    public bool IsAlive { get; protected set; }

    public abstract void GetDamage(int damage, string attackUnitUUID);

    public abstract string getUUID();
}

