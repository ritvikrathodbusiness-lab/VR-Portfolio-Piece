using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class SmallRobot : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public float engageRange = 8f;
    private float pathUpdateCooldown;
    public float pathUpdateInterval = 0.2f;
    public float turnSpeed = 180f;

    public Animator anim;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning($"{name}: No object tagged 'Player' found in scene. " +
                                  "Tag your XR player reference object as 'Player'.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (player == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        pathUpdateCooldown -= Time.deltaTime;

        if (pathUpdateCooldown <= 0f)
        {
            agent.SetDestination(player.position);
            pathUpdateCooldown = pathUpdateInterval;
            FacePlayer();
        }

         if (distanceToPlayer <= engageRange)
        {
            // Stop moving and face the player directly to aim

            if(agent.isStopped == false)
            {
                agent.isStopped = true;
                anim.SetBool("isAttacking", true);
            }

        }
        else
        {
            if(agent.isStopped == true)
            {
                agent.isStopped = false;
                anim.SetBool("isAttacking", false);
            }
        }
    }

    private void FacePlayer()
    {
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0f; // don't tilt up/down toward the player's head height

        if (lookDirection.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
}
