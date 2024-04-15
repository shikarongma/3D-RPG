using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStates : MonoBehaviour
{
    //�¼�           ��ǰѪ�� ��Ѫ��
    public event Action<int, int> UpdateHealthBarOnAttack;
    //ģ������
    public CharacterData_SO templateData;

    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    //�¼�Ѫ���仯
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

    //�ܵ������Ĺ�����ֵ
    public void TakeDamage(CharacterStates attacker/*������*/,CharacterStates defener/*�ܹ�����*/)
    {
        //�ܵ����˺�Ϊ�������ߵ�ʵ���˺���ȥ�����ߵķ�����
        int damage = (int)MathF.Max(attacker.CurrentDamage() - defener.CurrentDefence , 0);
        //����ֵ
        defener.CurrentHealth = (int)MathF.Max(CurrentHealth - damage, 0);
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        //Updata UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        //����updata
        if (CurrentHealth <= 0)
            attacker.characterData.UpdateExp(characterData.killPoint);

        OnHealthChange?.Invoke(this);
    }

    //����������
    public void TakeDamage(int damage/*�����˺�*/,CharacterStates defener)
    {
        int currentDamge = Mathf.Max(damage - defener.CurrentDefence, 0);
        defener.CurrentHealth = Mathf.Max(CurrentHealth - currentDamge, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

    }


    private int CurrentDamage()
    {
        //�����˺�
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)//��������� �˺����Ա���
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("����" + coreDamage);
        }

        return (int)coreDamage;
    }

    #endregion
}
