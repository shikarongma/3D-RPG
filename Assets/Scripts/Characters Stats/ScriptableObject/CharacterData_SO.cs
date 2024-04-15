using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data", menuName ="Character Stats/Data")]


public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]

    //最大生命值
    public int maxHealth;
    //当前生命值  
    public int currentHealth;
    //基础防御
    public int baseDefence;
    //当前防御
    public int currentDefence;

    [Header("Kill")]
    //怪物死亡的经验值
    public int killPoint;

    [Header("Level")]
    //当前等级
    public int currentLevel;
    //最高等级
    public int maxLevel;
    //基础经验值
    public int baseExp;
    public int currentExp;
    //BUFF增加多少
    public float levelBuff;

    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }

    //提升百分比 BUFF乘以多少
    public void UpdateExp(int point)
    {
        currentExp += point;

        if (currentExp >= baseExp)
            LevelUp();
    }

    private void LevelUp()
    {
        //所有你想提升的数据方法
        currentLevel = (int)MathF.Min(currentLevel + 1,maxLevel);
        currentExp = 0;
        baseExp += (int)(baseExp*LevelMultiplier);

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;

        Debug.Log("Level Up!" + currentLevel + "Max Health:" + maxHealth);

    }
}
