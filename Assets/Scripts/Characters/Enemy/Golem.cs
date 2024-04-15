using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyControl
{
    [Header("Skill")]

    public float kickForce = 25;

    public GameObject rockPrefab;
    public Transform handPos;

    //Animation Event
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStates>();

            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            //direction.Normalize();

            targetStats.GetComponent<NavMeshAgent>().isStopped = true;
            targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            //根据个人喜好添加
            targetStats.GetComponent<Animator>().SetTrigger("Dizzy");

            targetStats.TakeDamage(characterStates, targetStats);

        }

    }

    //Animation Event
    //扔石头
    public void ThrowRoch()
    {
        if (attackTarget != null)
        {
            //生成rock
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }

}
