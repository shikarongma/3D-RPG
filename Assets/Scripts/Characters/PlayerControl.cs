using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControl : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    private GameObject attackTarget;
    private float lastAttackTime;
    private CharacterStates characterStates;

    private float stopDistance;

    private bool isDeath;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterStates = GetComponent<CharacterStates>();
        stopDistance = agent.stoppingDistance;
    }

    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;

        GameManager.Instance.RigisterPlayer(this.characterStates);
    }

    private void Update()
    {
        isDeath = characterStates.CurrentHealth == 0;

        if (isDeath)
        {
            GameManager.Instance.NotifyObservers();
        }
        SwitchAnimation();
        //������ȴ��˥��
        lastAttackTime -= Time.deltaTime;
    }

    void SwitchAnimation()
    {
        animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
        animator.SetBool("Death", isDeath);
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (isDeath) return;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }

    //�ƶ���������ǰ���ҽ��й���
    private void EventAttack(GameObject target)
    {
        if (isDeath) return;


        if (target != null)
        {
            attackTarget = target;
            characterStates.isCritical = UnityEngine.Random.value < characterStates.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }
    //Э��
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStates.attackData.attackRange;

        //����ת�����
        transform.LookAt(attackTarget.transform);
        //��ѭ���ж����������֮��ľ��룬������֮��ľ���С��1ʱ���˳�ѭ��
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStates.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        //�˳�ѭ��������ֹͣ��������ǰ
        agent.isStopped = true;
        //����������������ȴ����0ʱ���ſ��Է�����һ�ι���
        if (lastAttackTime < 0)
        {
            animator.SetBool("Critical", characterStates.isCritical);
            animator.SetTrigger("Attack");
            //������ȴʱ��
            lastAttackTime = characterStates.attackData.coolDown;
        }

    }

    //Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>()&& attackTarget.GetComponent<Rock>().rockStates == RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStates = RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }


        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStates>();

            targetStats.TakeDamage(characterStates, targetStats);
        }
    }
}    

