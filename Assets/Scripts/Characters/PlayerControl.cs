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
        //攻击冷却的衰减
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

    //移动到物体面前并且进行攻击
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
    //协程
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStates.attackData.attackRange;

        //人物转向敌人
        transform.LookAt(attackTarget.transform);
        //用循环判断人物与敌人之间的距离，当两者之间的距离小于1时，退出循环
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStates.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        //退出循环后，人物停止，不再向前
        agent.isStopped = true;
        //攻击动画：攻击冷却到达0时，才可以发起下一次攻击
        if (lastAttackTime < 0)
        {
            animator.SetBool("Critical", characterStates.isCritical);
            animator.SetTrigger("Attack");
            //重置冷却时间
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

