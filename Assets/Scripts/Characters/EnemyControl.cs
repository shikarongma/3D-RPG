using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStates))]

public class EnemyControl : MonoBehaviour, IEndGameObserver
{
    protected CharacterStates characterStates;
    private float lastAttackTime;

    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    //控制动画
    private Animator anim;

    private Collider coll;

    //bool值配合动画
    bool isWalk;
    bool isChase;//追赶
    bool isFollow;
    bool isDeath;

    bool playerDeah;

    [Header("Basic Settings")]

    //是否为站桩的敌人
    public bool isGuard;
    //原有的速度
    private float speed;
    //可视范围
    public float sightRadius;
    //敌人的攻击目标
    protected GameObject attackTarget;
    //巡逻观察时间
    public float lookAtTime;
    private float remainLookAtTime;

    //巡逻
    [Header("Patrol State")]
    //巡逻范围
    public float patrolRange;
    //在巡逻范围内随机找一点移动过去
    //1点
    private Vector3 wayPoint;
    //获取初始位置
    //方法1
    private Vector3 guardPos;
    //方法2
    //public GameObject position;
    //若为站桩敌人 获取本身的面向方向(旋转角度）
    private Quaternion guardRotation;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStates = GetComponent<CharacterStates>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }

    void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }

        //FIXME:场景切换后修改掉
        GameManager.Instance.AddObserver(this);

    }
    //切换场景时启用
    //void OnEnable()
    //{
    //    GameManager.Instance.AddObserver(this);
    //}

    void OnDisable()
    {
        if (!GameManager.IsInitialized)
            return;
        GameManager.Instance.RemoveObserver(this);
    }
    void Update()
    {
        if (characterStates.CurrentHealth == 0)
            isDeath = true;

        if (!playerDeah)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }

    }
    //切换动画
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStates.isCritical);
        anim.SetBool("Death", isDeath);
    }

    //转换状态
    void SwitchStates()
    {
        if (isDeath)
        {
            enemyStates = EnemyStates.DEAD;
        }

        //如果发现Player 切换到CHASE
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }
        //敌人的状态
        switch (enemyStates)
        {
            //守卫的敌人
            case EnemyStates.GUARD:
                isChase = false;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                }
                break;
            //巡逻的敌人
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                //Distance函数是判断两点之间的距离
                //
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            //追逐的敌人
            case EnemyStates.CHASE:
                //配合动画
                isWalk = false;
                isChase = true;

                agent.speed = speed;

                if (FoundPlayer())
                {
                    isFollow = true;
                    agent.isStopped = false;

                    agent.destination = attackTarget.transform.position;
                }

                else
                {
                    //拉脱回到上一个状态
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)
                        enemyStates = EnemyStates.GUARD;
                    else
                        enemyStates = EnemyStates.PATROL;
                }

                //在攻击范围内则攻击
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStates.attackData.coolDown;

                        //暴击判断
                        characterStates.isCritical = UnityEngine.Random.value < characterStates.attackData.criticalChance;
                        //执行攻击
                        Attack();
                    }
                }
                break;
            //死亡的敌人
            case EnemyStates.DEAD:
                agent.enabled = false;//直接将agent component关闭
                //coll.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }

    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        }
        //技能攻击动画
        if (TargetInSkillRange())
        {
            anim.SetTrigger("Skill");

        }
    }

    //是否发现玩家
    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStates.attackData.attackRange;
        else
            return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStates.attackData.skillRange;
        else
            return false;
    }

    //随机生成点的函数方法
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        float randomX = UnityEngine.Random.Range(-patrolRange, patrolRange);
        float randomZ = UnityEngine.Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);

        wayPoint = randomPoint;
        //NavMeshHit hit;
        //wayPoint = NavMesh.SamplePosition(randomPoint,out hit,patrolRange,1)?hit.position:transform.position;
    }

    //在scene中画出目标可移动区域
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget != null &&transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStates>();

            targetStats.TakeDamage(characterStates, targetStats);
        }

    }

    public void EndNotify()
    {
        //获胜动画
        //停止所有移动
        //停止Agent
        playerDeah = true;
        anim.SetBool("Win", true);
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
}
