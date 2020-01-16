using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderMovement : MonoBehaviour
{
    public float DetectionDistance = 50.0f;
    public float RunDistance = 30.0f;
    public float AttackDistance = 5.0f;
    public float WalkSpeed = 4;
    public float RunSpeed = 8;
    public GameObject target;
    public GameObject player;
    NavMeshAgent agent;
    Animation anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animation>();
    }
    
    void Update()
    {
        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (AttackDistance > dist)
        {
            anim.Play("attack1");
            agent.SetDestination(target.transform.position);
        }
        else if (dist < RunDistance)
        {
            transform.LookAt(target.transform);
            anim.Play("run");
            agent.speed = RunSpeed;
            agent.SetDestination(target.transform.position);
        }
        else if (dist < DetectionDistance)
        {
            transform.LookAt(target.transform);
            anim.Play("walk");
            agent.speed = WalkSpeed;
            agent.SetDestination(target.transform.position);
        }

    }

    void OnMouseDown()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance < 15.0f)
        {
            target = this.gameObject;
            StartCoroutine(ReEnableNavmesh(3f));
        }
    }

    private IEnumerator ReEnableNavmesh(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        target = player;
    }
}