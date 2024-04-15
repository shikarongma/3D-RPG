using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStates : MonoBehaviour
{
    //事件           当前血量 满血量
    public event Action<int, int> UpdateHealthBarOnAttack;
    //模板数据
    public CharacterData_SO templateData;

    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    //事件血条变化
    public UnityEvent<CharacterStates> OnHealthChange;

    [HideInInspector]
    public bool isCritical;
    private void Update()
    {
        OnHealthChange?.Invoke(this);
    }
    private void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
           
    }

    #region Read from Data_SO

    public int MaxHealth
    {
        get
        {
            if (characterData != null)
                return characterData.maxHealth;
            else
                return 0;
        }
        set
        {
            characterData.maxHealth = value;
        }

    }
    public int CurrentHealth
    {
        get
        {
            if (characterData != null)
                return characterData.currentHealth;
            else
                return 0;
        }
        set
        {
            characterData.currentHealth = value;
        }

    }
    public int BaseDefence
    {
        get
        {
            if (characterData != null)
                return characterData.baseDefence;
            else
                return 0;
        }
        set
        {
            characterData.baseDefence = value;
        }

    }
    public int CurrentDefence
    {
        get
        {
            if (characterData != null)
                return characterData.currentDefence;
            else
                return 0;
        }
        set
        {
            characterData.currentDefence = value;
        }

    }
    #endregion

    #region Character Combat

    //受到攻击的攻击数值
    public void TakeDamage(CharacterStates attacker/*攻击者*/,CharacterStates defener/*受攻击者*/)
    {
        //受到的伤害为：攻击者的实际伤害减去防御者的防御力
        int damage = (int)MathF.Max(attacker.CurrentDamage() - defener.CurrentDefence , 0);
        //生命值
        defener.CurrentHealth = (int)MathF.Max(CurrentHealth - damage, 0);
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        //Updata UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        //经验updata
        if (CurrentHealth <= 0)
            attacker.characterData.UpdateExp(characterData.killPoint);

        OnHealthChange?.Invoke(this);
    }

    //函数的重载
    public void TakeDamage(int damage/*基础伤害*/,CharacterStates defener)
    {
        int currentDamge = Mathf.Max(damage - defener.CurrentDefence, 0);
        defener.CurrentHealth = Mathf.Max(CurrentHealth - currentDamge, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

    }


    private int CurrentDamage()
    {
        //核心伤害
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)//如果暴击了 伤害乘以倍率
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击" + coreDamage);
        }

        return (int)coreDamage;
    }

    #endregion
}
