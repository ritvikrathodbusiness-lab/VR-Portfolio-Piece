using System.Collections;
using UnityEngine;

/// <summary>
/// Basic enemy behavior: moves straight toward the player until within
/// engage range, then stops and fires projectiles at intervals.
/// No pathfinding/NavMesh — fine for primitive placeholder enemies (cubes/spheres)
/// in an open room. Swap in a NavMeshAgent later if you add obstacles/cover.
/// </summary>
[RequireComponent(typeof(Collider))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Units per second the enemy moves toward the player.")]
    public float moveSpeed = 3f;

    [Tooltip("Distance at which the enemy stops advancing and starts shooting.")]
    public float engageRange = 8f;

    [Tooltip("How fast the enemy rotates to face the player, in degrees/second.")]
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

    private float fireCooldown;

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
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    
        Debug.Log($"{name}: Distance to player: {distanceToPlayer:F2}, Engage range: {engageRange}");

        FacePlayer();

        if (distanceToPlayer > engageRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = fireRate;
            }
        }
    }

    private void MoveTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0f; // keep movement on the horizontal plane
        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

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

    // Visualize engage range in the Scene view while the enemy is selected
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, engageRange);
    }
}