using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data", menuName ="Character Stats/Data")]


public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]

    //�������ֵ
    public int maxHealth;
    //��ǰ����ֵ  
    public int currentHealth;
    //��������
    public int baseDefence;
    //��ǰ����
    public int currentDefence;

    [Header("Kill")]
    //���������ľ���ֵ
    public int killPoint;

    [Header("Level")]
    //��ǰ�ȼ�
    public int currentLevel;
    //��ߵȼ�
    public int maxLevel;
    //��������ֵ
    public int baseExp;
    public int currentExp;
    //BUFF���Ӷ���
    public float levelBuff;

    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }

    //�����ٷֱ� BUFF���Զ���
    public void UpdateExp(int point)
    {
        currentExp += point;

        if (currentExp >= baseExp)
            LevelUp();
    }

    private void LevelUp()
    {
        //�����������������ݷ���
        currentLevel = (int)MathF.Min(currentLevel + 1,maxLevel);
        currentExp = 0;
        baseExp += (int)(baseExp*LevelMultiplier);

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;

        Debug.Log("Level Up!" + currentLevel + "Max Health:" + maxHealth);

    }
}
