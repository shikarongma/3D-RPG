using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Attack",menuName ="Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    //基本攻击距离
    public float attackRange;
    //远程攻击距离
    public float skillRange;
    //CD冷却时间内
    public float coolDown;
    //最小攻击数值
    public int minDamage;
    //最大攻击数值
    public int maxDamage;

    //暴击数值
    public float criticalMultiplier;
    //暴击率
    public float criticalChance;
}
