using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Attack",menuName ="Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    //������������
    public float attackRange;
    //Զ�̹�������
    public float skillRange;
    //CD��ȴʱ����
    public float coolDown;
    //��С������ֵ
    public int minDamage;
    //��󹥻���ֵ
    public int maxDamage;

    //������ֵ
    public float criticalMultiplier;
    //������
    public float criticalChance;
}
