using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Leader : MonoBehaviour
{
    GameObject[] teamMemebers, allTargets;
    int nbTeamMembers, nbTargets;
    [SerializeField] bool player = false;
    NavMeshAgent navMeshAgent;
    int WPCount = 0;
    public GameObject target;
    public GameObject[] waypoints;
    AnimatorStateInfo info;
    Animator anim;
    bool attacking = false;
    GameObject Player;

    void Start()
    {
        if (!player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            anim = GetComponent<Animator>();
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i].GetComponent<Renderer>().enabled = false;
            }
            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        }
        teamMemebers = GameObject.FindGameObjectsWithTag("teamMember");
        nbTeamMembers = teamMemebers.Length;
    }
    void Attack()
    {
        Debug.Log("attack");
        allTargets = GameObject.FindGameObjectsWithTag("target");
        nbTargets = allTargets.Length;
        for (int i = 0; i < nbTargets; i++)
        {
            teamMemebers[i].GetComponent<TeamMember>().Attack(allTargets[i]);
        }
    }
    void Update()
    {
        if (player)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Attack();
            }
        }
        else
        {
            Type2Move();
            if (info.IsName("FollowWaypoints"))
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(target.transform.position);
            }
            if (info.IsName("Idle"))
            {
                navMeshAgent.isStopped = true;
            }
            if (info.IsName("Attacking"))
            {
                navMeshAgent.isStopped = true;
                if (!attacking)
                {
                    Attack();
                    attacking = true;
                }
                navMeshAgent.isStopped = true;
            }
            else
            {
                if (attacking)
                {
                    Attack();
                }
                attacking = false;
            }
        }
    }
    void Type2Move()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 1f)
        {
            WPCount = UnityEngine.Random.Range(0, waypoints.Length);
            target = waypoints[WPCount];
            navMeshAgent.SetDestination(target.transform.position);
        }
        float distanceToTarget = Vector3.Distance(Player.transform.position, transform.position);
        if (distanceToTarget < 4f)
        {
            anim.SetBool("followWaypoints", false);
            anim.SetBool("attck", true);
        }
        else
        {
            anim.SetBool("followWaypoints", true);
            anim.SetBool("attck", false);
        }
    }
}
