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
    //���ƶ���
    private Animator anim;

    private Collider coll;

    //boolֵ��϶���
    bool isWalk;
    bool isChase;//׷��
    bool isFollow;
    bool isDeath;

    bool playerDeah;

    [Header("Basic Settings")]

    //�Ƿ�Ϊվ׮�ĵ���
    public bool isGuard;
    //ԭ�е��ٶ�
    private float speed;
    //���ӷ�Χ
    public float sightRadius;
    //���˵Ĺ���Ŀ��
    protected GameObject attackTarget;
    //Ѳ�߹۲�ʱ��
    public float lookAtTime;
    private float remainLookAtTime;

    //Ѳ��
    [Header("Patrol State")]
    //Ѳ�߷�Χ
    public float patrolRange;
    //��Ѳ�߷�Χ�������һ���ƶ���ȥ
    //1��
    private Vector3 wayPoint;
    //��ȡ��ʼλ��
    //����1
    private Vector3 guardPos;
    //����2
    //public GameObject position;
    //��Ϊվ׮���� ��ȡ�����������(��ת�Ƕȣ�
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

        //FIXME:�����л����޸ĵ�
        GameManager.Instance.AddObserver(this);

    }
    //�л�����ʱ����
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
    //�л�����
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStates.isCritical);
        anim.SetBool("Death", isDeath);
    }

    //ת��״̬
    void SwitchStates()
    {
        if (isDeath)
        {
            enemyStates = EnemyStates.DEAD;
        }

        //�������Player �л���CHASE
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }
        //���˵�״̬
        switch (enemyStates)
        {
            //�����ĵ���
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
            //Ѳ�ߵĵ���
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                //Distance�������ж�����֮��ľ���
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
            //׷��ĵ���
            case EnemyStates.CHASE:
                //��϶���
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
                    //���ѻص���һ��״̬
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

                //�ڹ�����Χ���򹥻�
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStates.attackData.coolDown;

                        //�����ж�
                        characterStates.isCritical = UnityEngine.Random.value < characterStates.attackData.criticalChance;
                        //ִ�й���
                        Attack();
                    }
                }
                break;
            //�����ĵ���
            case EnemyStates.DEAD:
                agent.enabled = false;//ֱ�ӽ�agent component�ر�
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
            //����������
            anim.SetTrigger("Attack");
        }
        //���ܹ�������
        if (TargetInSkillRange())
        {
            anim.SetTrigger("Skill");

        }
    }

    //�Ƿ������
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

    //������ɵ�ĺ�������
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

    //��scene�л���Ŀ����ƶ�����
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
        //��ʤ����
        //ֹͣ�����ƶ�
        //ֹͣAgent
        playerDeah = true;
        anim.SetBool("Win", true);
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
}
