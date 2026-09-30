using System.Collections;
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

    [Header("Jump Attack")]
    [Tooltip("How long the leap toward the player takes, in seconds.")]
    public float jumpDuration = 0.5f;

    [Tooltip("How high the leap arcs upward at its peak.")]
    public float jumpHeight = 1.5f;

    [Header("Explosion")]
    [Tooltip("Prefab spawned on impact — particle effect, sound, etc. Optional.")]
    public GameObject explosionPrefab;

    [Tooltip("Radius from the impact point that counts as a hit on the player.")]
    public float explosionRadius = 2f;

    [Tooltip("Placeholder damage value — hook this into player health once it exists.")]
    public float explosionDamage = 25f;

    private bool isJumping = false;

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
        if (player == null || agent == null || isJumping) return;

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
            // Stop moving and leap at the player instead of just teleporting to them
            if (agent.isStopped == false)
            {
                agent.isStopped = true;
                anim.SetBool("isAttacking", true);
                StartCoroutine(LeapAtPlayer());
            }
        }
        else
        {
            if (agent.isStopped == true)
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

    private IEnumerator LeapAtPlayer()
    {
        isJumping = true;

        // Snapshot the target position once at launch, rather than tracking a
        // moving player mid-air — feels more like a committed leap, less like homing.
        Vector3 startPos = transform.position;
        Vector3 targetPos = player != null ? player.position : startPos;
        targetPos.y = startPos.y; // keep the landing height consistent with takeoff

        float elapsed = 0f;

        // Disable the NavMeshAgent's own position control while we manually
        // animate the arc — otherwise the two fight over transform.position.
        agent.enabled = false;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;

            // Linear interpolation across the ground plane...
            Vector3 flatPos = Vector3.Lerp(startPos, targetPos, t);

            // ...plus a parabolic arc upward and back down for the "jump" feel.
            float arc = jumpHeight * 4f * t * (1f - t);
            flatPos.y = startPos.y + arc;

            transform.position = flatPos;

            yield return null;
        }

        transform.position = targetPos;
        Explode();
    }

    private void Explode()
    {
        if (explosionPrefab != null)
        {
          var explosion =  Instantiate(explosionPrefab, transform.position, Quaternion.identity);
          Destroy(explosion, 1.5f);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // TODO: replace with a call into your player health system once it exists,
                // e.g. hit.GetComponent<PlayerHealth>()?.TakeDamage(explosionDamage);
                Debug.Log($"Player caught in explosion for {explosionDamage} damage.");
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, engageRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}