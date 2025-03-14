using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeamMember : MonoBehaviour
{
    GameObject leader;
    Animator anim;
    AnimatorStateInfo info;
    float distanceToLeader;
    NavMeshAgent agent;
    public GameObject target;
    float distanceToTarget;
    [SerializeField] private bool FollowLeader = false;
    public int health = 10;
    GameObject targetBeingAttacked;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (!FollowLeader) leader = GameObject.Find("player");
        else leader = GameObject.Find("Leader");
    }
    private float timer = 0f;
    private float interval = 1f;
    void Update()
    {
        timer += Time.deltaTime;

        if (health < 0)
        {
            Destroy(gameObject);
        }
        info = anim.GetCurrentAnimatorStateInfo(0);
        distanceToLeader = Vector3.Distance(transform.position, leader.transform.position);
        if (distanceToLeader < 5) anim.SetBool("closeToLeader", true);
        else anim.SetBool("closeToLeader", false);
        if (info.IsName("AttackTarget"))
        {
            if (targetBeingAttacked != null && timer >= interval) { targetBeingAttacked.GetComponent<TeamMember>().health--; timer = 0f; }
            if (targetBeingAttacked == null)
            {
                anim.SetBool("TargetDestoyed", true);
            }
        }
        if (info.IsName("GoToTarget"))
        {
            if (target != null)
            {
                agent.isStopped = false;
                agent.SetDestination(target.transform.position);
                distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
                if (distanceToTarget < 1f)
                {
                    anim.SetBool("closeToTarget", true);
                    agent.isStopped = true;
                }
                else
                {
                    anim.SetBool("closeToTarget", false);

                }
            }
            else anim.SetBool("TargetDestoyed", false);

        }
        if (info.IsName("Idle"))
        {
            agent.isStopped = true;
        }
        if (info.IsName("MoveTowardsLeader"))
        {
            agent.isStopped = false;
            agent.SetDestination(leader.transform.position);
        }
    }
    bool switchAttack = true;
    public void Attack(GameObject t)
    {
        if (t != null)
        {
            if (switchAttack)
            {
                targetBeingAttacked = t;
                anim.SetBool("TargetDestoyed", false);
                anim.SetTrigger("attackOneToOne");
                target = t;
                switchAttack = !switchAttack;
            }
            else
            {
                switchAttack = !switchAttack;
                anim.SetBool("TargetDestoyed", true);
            }
        }
        else
        {
            Debug.Log("hgi");
            anim.SetBool("TargetDestoyed", false);
        }
    }
}
