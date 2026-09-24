using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

/// <summary>
/// Enemy behavior using NavMeshAgent for pathfinding — moves toward the
/// player around obstacles/walls instead of straight through them, then
/// stops and fires once within engage range. Includes health/UI hookup.
/// Requires: NavMeshAgent component on this object, and a baked NavMesh
/// in the scene (add a Nav Mesh Surface component to your level and Bake).
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    #region Variables
    [Header("Movement")]
    [Tooltip("Distance at which the enemy stops advancing and starts shooting.")]
    public float engageRange = 8f;

    [Tooltip("How often the agent recalculates its path to the player, in seconds. " +
             "Lower = more responsive but more expensive.")]
    public float pathUpdateInterval = 0.2f;

    [Tooltip("How fast the enemy rotates to face the player once in engage range, in degrees/second.")]
    public float turnSpeed = 180f;

    [Header("Shooting")]
    [Tooltip("Prefab spawned when the enemy fires. Needs an EnemyProjectile component.")]
    public GameObject projectilePrefab;

    [Tooltip("Empty child transform marking where projectiles spawn from (e.g. gun muzzle).")]
    public Transform firePoint;

    [Tooltip("Seconds between shots while in engage range.")]
    public float fireRate = 1.5f;

    [Header("Player Reference")]
    [Tooltip("Leave empty to auto-find the object tagged 'Player' at Start.")]
    public Transform player;

    [Header("Health")]
    public float health = 100f;

    [SerializeField] Slider healthBar;

    private NavMeshAgent agent;
    private float fireCooldown;
    private float pathUpdateCooldown;
    #endregion

#region Unity Lifecycle
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
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

        if (healthBar != null)
        {
            healthBar.maxValue = health;
            healthBar.value = health;
        }

        // Let the agent's stopping distance match engage range so it
        // naturally halts at the right spot instead of overshooting.
        agent.stoppingDistance = engageRange;
    }

    private void Update()
    {
        if (player == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Re-path periodically rather than every single frame — cheaper,
        // and the player rarely moves fast enough to need per-frame updates.
        pathUpdateCooldown -= Time.deltaTime;
        if (pathUpdateCooldown <= 0f)
        {
            agent.SetDestination(player.position);
            pathUpdateCooldown = pathUpdateInterval;
        }

        if (distanceToPlayer <= engageRange)
        {
            // Stop moving and face the player directly to aim
            agent.isStopped = true;
            FacePlayer();

            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = fireRate;
            }
        }
        else
        {
            agent.isStopped = false;
        }
    }
#endregion

#region Movement and Shooting
    private void FacePlayer()
    {
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0f; // don't tilt up/down toward the player's head height

        if (lookDirection.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning($"{name}: Missing projectilePrefab or firePoint, can't shoot.");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Make sure the projectile actually points at the player, not just
        // wherever firePoint happens to be rotated.
        Vector3 aimDirection = (player.position - firePoint.position).normalized;
        projectile.transform.rotation = Quaternion.LookRotation(aimDirection);
    }
#endregion

#region  Health Management
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (healthBar != null)
        {
            healthBar.value = health;
        }

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
#endregion

}