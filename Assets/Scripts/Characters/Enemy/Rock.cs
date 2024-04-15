using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum RockStates { HitPlayer, HitEnemy, HitNothing }

public class Rock : MonoBehaviour
{
    private Rigidbody rb;
    public RockStates rockStates;

    [Header("Basic Setting")]

    public float force;

    //ʯͷ�������˺�
    public int damage;
    public GameObject target;
    private Vector3 direction;

    //ʯͷ����
    public GameObject breakEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }

    //��ʯͷ�ٶ�С��1ʱ��ʯͷ��״̬��ΪHitNothing
    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1)
        {
            rockStates = RockStates.HitNothing;
        }
    }
    //�������
    public void FlyToTarget()
    {
        if (target == null)
            target = FindObjectOfType<PlayerControl>().gameObject;

        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);

    }

    void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;

                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharacterStates>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStates>());

                    rockStates = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStates>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;

        }
    }
}
