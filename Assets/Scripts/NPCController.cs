using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class NPCController : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    public GameObject[] waypoints;
    public GameObject target;
    int WPCount = 0;
    Animator anim;
    AnimatorStateInfo info;
    float distance;
    float distanceToClosestPack;
    bool isInTheFieldOfView;
    GameObject[] allBCs;
    Vector3 direction;
    [Range(0, 100)]
    public int health;
    float healthTimer;
    GameObject[] healthPacks;
    GameObject[] AmmoPacks;

    public GameObject player;
    public GameObject ambushStart;
    Vector3 initailPosition;
    public int enemyType = 0;
    public GameObject bullet;
    public bool ambushAreaTriggered = false;
    [Range(0, 10)]
    public int ammo = 10;
    bool foundClosestAmmoPack = false;
    bool healthPacksAvailable = false;
    void Start()
    {
        anim = GetComponent<Animator>();
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i].GetComponent<Renderer>().enabled = false;
        }
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        // anim.SetBool("healthPackAvailable", true);
        if (enemyType == 1 || enemyType == 2)
        {
            anim.SetTrigger("FollowWaypoints");
            target = waypoints[WPCount];
        }
        SetHealth(100);
    }
    private float timer = 0f;
    private float interval = 3f;
    void Update()
    {
        anim.SetInteger("health", health);
        anim.SetInteger("ammo", ammo);
        timer += Time.deltaTime;

        info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("FollowPlayer") || info.IsName("Hunter"))
        {
            target = player;
            navMeshAgent.isStopped = false;
            // NavMeshPath path = new NavMeshPath();
            // navMeshAgent.CalculatePath(target.transform.position, path);
            // navMeshAgent.SetPath(path);
            navMeshAgent.SetDestination(target.transform.position);
        }
        if (info.IsName("FollowWaypoint"))
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(target.transform.position);
        }
        if (info.IsName("Idle"))
        {
            navMeshAgent.isStopped = true;
        }
        if (info.IsName("Flee"))
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 20)
            {
                Vector3 awayFromPlayer = (transform.position - player.transform.position).normalized;
                Vector3 fleePosition = transform.position + awayFromPlayer * 5;

                navMeshAgent.SetDestination(fleePosition);
                navMeshAgent.isStopped = false;
            }
        }
        if (info.IsName("GoToAmbush") && enemyType == 3)
        {
            navMeshAgent.isStopped = false;
            target = ambushStart;
            navMeshAgent.SetDestination(target.transform.position);
            navMeshAgent.speed = 5.5f;
            float distanceToAmbushStart = Vector3.Distance(gameObject.transform.position, target.transform.position);
            if (distanceToAmbushStart < 2.5f)
            {
                anim.SetTrigger("reachedAmbush");
            }
        }
        if (info.IsName("BackToStart"))
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(initailPosition);
            if (Vector3.Distance(transform.position, initailPosition) < 2) anim.SetTrigger("reachedStartingPosition");
        }

        if (info.IsName("LookForHealthPack"))
        {
            navMeshAgent.isStopped = false;
            healthPacks = GameObject.FindGameObjectsWithTag("healthPack");
            if (healthPacks.Length == 0)
            {
                anim.SetBool("healthPackAvailable", false);
                if (enemyType == 2) anim.SetTrigger("startToFlee");
                else anim.SetTrigger("FollowWaypoints");
            }
            else
            {
                anim.SetBool("healthPackAvailable", true);
                SelectHealthPackToReachFor2();
                // GetComponent<NavMeshAgent>().SetDestination(target.transform.position);
            }
            if (target != null && target.tag == "healthPack")
            {
                navMeshAgent.SetDestination(target.transform.position);
                if (Vector3.Distance(transform.position, target.transform.position) < 2)
                {
                    SetHealth(100);
                    Destroy(target);
                    anim.SetTrigger("FollowWaypoints");

                }
            }
        }
        if (info.IsName("LookForAmmoPack"))
        {
            navMeshAgent.isStopped = false;
            AmmoPacks = GameObject.FindGameObjectsWithTag("ammo");

            if (AmmoPacks.Length == 0 || (navMeshAgent.remainingDistance < 1f) && target.tag == "ammo")
            {
                ammo = ammo + 10;
                foundClosestAmmoPack = false;
                if (enemyType == 3) anim.SetBool("Hunter", true);
                else anim.SetTrigger("FollowWaypoints");
                target = waypoints[0];
            }
            else if (!foundClosestAmmoPack)
            {
                SearchForAmmoPack();
            }
            navMeshAgent.SetDestination(target.transform.position);
        }
        switch (enemyType)
        {
            case 1:
                Type1Move();
                look();
                Smell();
                listen();
                Shoot();
                break;
            case 2:
                if (!info.IsName("LookForHealthPack") && !info.IsName("LookForAmmoPack"))
                {
                    Type2Move();
                    look();
                    Smell();
                    listen();
                    Shoot();
                }
                if (ammo == 0)
                {
                    anim.SetTrigger("lookForAmmoPack");
                }
                if (health < 20 && !info.IsName("Flee"))
                {
                    anim.SetTrigger("lookForHealthPack");
                }
                break;
            case 3:
                Type3Move();
                Shoot();
                if (ammo == 0)
                {
                    anim.SetTrigger("lookForAmmoPack");
                }
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
        }
        healthTimer += Time.deltaTime;
        // if (healthTimer > 2)
        // {
        //     SetHealth(health - 2);
        //     healthTimer = 0;
        // }
        if (health < 0) Destroy(gameObject);
    }

    private void SearchForAmmoPack()
    {
        float distanceToClosestAmmo = Mathf.Infinity;
        int closestIndex = 0;

        for (int i = 0; i < AmmoPacks.Length; i++)
        {
            float distanceToCurrentAmmo = Vector3.Distance(transform.position, AmmoPacks[i].transform.position);
            if (distanceToCurrentAmmo < distanceToClosestAmmo)
            {
                distanceToClosestAmmo = distanceToCurrentAmmo;
                closestIndex = i;
            }
        }
        foundClosestAmmoPack = true;
        target = AmmoPacks[closestIndex];
        navMeshAgent.SetDestination(target.transform.position);
        Debug.Log($"Setting NavMeshAgent destination to {target.transform.position}");
    }


    void SetHealth(int newHealth)
    {
        health = newHealth;
        anim.SetInteger("health", health);
    }
    private void Shoot()
    {
        if (timer >= interval && (info.IsName("FollowPlayer") || info.IsName("Hunter")))
        {
            GameObject currentBullet = Instantiate(bullet, transform.position + (transform.forward * 2f), Quaternion.identity);
            currentBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 10f, ForceMode.Impulse);
            currentBullet.GetComponent<Rigidbody>().useGravity = false;
            Destroy(currentBullet, 5f);
            timer = 0f;
            ammo--;
        }
    }

    private void SelectHealthPackToReachFor2()
    {
        distanceToClosestPack = 1000;
        int rankOfClosestPack = 100;
        for (int i = 0; i < healthPacks.Length - 1; i++)
        {
            navMeshAgent.SetDestination(healthPacks[i].transform.position);
            float distanceToCurrentPack = navMeshAgent.remainingDistance;
            if (distanceToCurrentPack < distanceToClosestPack)
            {
                distanceToClosestPack = distanceToCurrentPack;
                rankOfClosestPack = i;
            }
        }
        target = healthPacks[rankOfClosestPack];

    }
    private void SelectHealthPackToReachFor()
    {
        distanceToClosestPack = 1000;
        int rankOfClosestPack = 100;
        for (int i = 0; i < healthPacks.Length; i++)
        {
            float distanceToCurrentPack = Vector3.Distance(transform.position, healthPacks[i].transform.position);
            if (distanceToCurrentPack < distanceToClosestPack)
            {
                distanceToClosestPack = distanceToCurrentPack;
                rankOfClosestPack = i;
            }
        }
        // target = GameObject.Find("healthPack");
        target = healthPacks[rankOfClosestPack];

    }
    private void Smell()
    {
        allBCs = GameObject.FindGameObjectsWithTag("BC");
        float minDistance = 2;
        bool detectedBC = false;
        for (int i = 0; i < allBCs.Length; i++)
        {
            if (Vector3.Distance(transform.position, allBCs[i].transform.position) < minDistance)
            {
                detectedBC = true;
                break;
            }
        }
        if (detectedBC) anim.SetBool("canSmellPlayer", true);
        else anim.SetBool("canSmellPlayer", false);
    }

    void look()
    {
        direction = (player.transform.position - transform.position).normalized;
        isInTheFieldOfView = (Vector3.Dot(transform.forward.normalized, direction) > .7);
        Debug.DrawRay(transform.position, direction * 100, Color.green);
        Debug.DrawRay(transform.position, transform.forward * 100, Color.blue);
        Debug.DrawRay(transform.position, (transform.forward - transform.right) * 100, Color.red);
        Debug.DrawRay(transform.position, (transform.forward + transform.right) * 100, Color.yellow);

        // if (isInTheFieldOfView) anim.SetBool("canSeePlayer", true);
        // else anim.SetBool("canSeePlayer", false);

        Ray ray = new Ray();
        RaycastHit hit;
        ray.origin = transform.position + Vector3.up * 0.5f;
        string objectInSight = "";
        float castingDistance = 30;
        ray.direction = transform.forward * castingDistance;

        if (Physics.Raycast(ray.origin, direction, out hit, castingDistance))
        {
            objectInSight = hit.collider.gameObject.name;
            if (objectInSight == "target" && isInTheFieldOfView) anim.SetBool("canSeePlayer", true);
            else anim.SetBool("canSeePlayer", false);
        }

    }
    void listen()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < 3)
        {
            anim.SetBool("canHearPlayer", true);
            Debug.Log("can hear");
        }
        else
        {
            anim.SetBool("canHearPlayer", false);
        }
    }
    void Type1Move()
    {
        if (info.IsName("FollowWapoints"))
        {
            target = waypoints[WPCount];
            navMeshAgent.SetDestination(target.transform.position);
            if (Vector3.Distance(transform.position, target.transform.position) < 1)
            {
                WPCount++;
                if (WPCount > waypoints.Length - 1) WPCount = 0;
            }
        }
        else if (info.IsName("FollowPlayer"))
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer > 20)
            {
                anim.SetTrigger("FollowWaypoints");
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
    }
    void Type3Move()
    {
        if (!info.IsName("LookForAmmoPack"))
        {
            target = player;
            anim.SetBool("Hunter", true);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            health--;
        }
    }
}
