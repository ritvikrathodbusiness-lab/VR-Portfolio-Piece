using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    #region Variables
    [Header("Movement")]
    [Tooltip("Distance at which the enemy stops advancing and starts shooting.")]
    public float engageRange = 8f;

    [Tooltip("How often the agent recalculates its path to the player, in seconds.")]
    public float pathUpdateInterval = 0.2f;

    [Tooltip("How fast the enemy rotates to face the player once in engage range, in degrees/second.")]
    public float turnSpeed = 180f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    [SerializeField] private Transform gunTrans;
    [SerializeField] private Animator anim;
    public Transform firePoint;
    public float fireRate = 1.5f;

    [Header("Player Reference")]
    public Transform player;

    [Header("Health & UI")]
    public float health = 100f;
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject healthBarCanvas; // Drag the world-space UI Canvas here to hide it on death

    private NavMeshAgent agent;
    private Collider mainCollider;
    private Rigidbody mainRigidbody;
    private float fireCooldown;
    private float pathUpdateCooldown;

    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;
    private bool isDead = false;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCollider = GetComponent<Collider>();
        mainRigidbody = GetComponent<Rigidbody>();

        // Cache all child Rigidbodies and Colliders (excluding the root/main components)
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Ensure ragdoll starts disabled while alive
        SetRagdollState(false);
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
                Debug.LogWarning($"{name}: No object tagged 'Player' found in scene.");
            }
        }

        if (healthBar != null)
        {
            healthBar.maxValue = health;
            healthBar.value = health;
        }

        agent.stoppingDistance = engageRange;
    }

    private void Update()
    {
        if (isDead || player == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        pathUpdateCooldown -= Time.deltaTime;
        if (pathUpdateCooldown <= 0f)
        {
            agent.SetDestination(player.position);
            pathUpdateCooldown = pathUpdateInterval;
        }

        if (distanceToPlayer <= engageRange)
        {
            if (!agent.isStopped)
            {
                agent.isStopped = true;
                if (gunTrans != null)
                {
                    gunTrans.localPosition = new Vector3(-0.0939f, 0.0963f, 0.1088f);
                    gunTrans.localRotation = Quaternion.Euler(-154.933f, -63.444f, 255.3f);
                }
            }

            if (anim != null) anim.SetBool("isShooting", true);
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
            if (agent.isStopped)
            {
                agent.isStopped = false;
                if (gunTrans != null)
                {
                    gunTrans.localPosition = new Vector3(-0.126f, 0.075f, 0.03f);
                    gunTrans.localRotation = Quaternion.Euler(-153.915f, -111.94f, 261.492f);
                }
            }

            if (anim != null) anim.SetBool("isShooting", false);
        }
    }
    #endregion

    #region Movement and Shooting
    private void FacePlayer()
    {
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Vector3 aimDirection = (player.position - firePoint.position).normalized;
        projectile.transform.rotation = Quaternion.LookRotation(aimDirection);
    }
    #endregion

    #region Health Management & Ragdoll
    public void TakeDamage(float amount)
    {
        if (isDead) return;

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

    private void SetRagdollState(bool active)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb != mainRigidbody)
            {
                rb.isKinematic = !active;
            }
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col != mainCollider)
            {
                col.enabled = active;
            }
        }
    }

    private void Die()
    {
        isDead = true;

        // 1. Disable Animator & Agent components
        if (anim != null) anim.enabled = false;
        if (agent != null) agent.enabled = false;

        // 2. Disable root capsule collider & main rigidbody
        if (mainCollider != null) mainCollider.enabled = false;
        if (mainRigidbody != null) mainRigidbody.isKinematic = true;

        // 3. Enable limb Ragdoll physics
        SetRagdollState(true);

        // 4. Hide Healthbar UI
        if (healthBarCanvas != null)
        {
            healthBarCanvas.SetActive(false);
        }
        else if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        // Optional: Destroy corpse after 10 seconds to save performance on Quest
        Destroy(gameObject, 10f);
    }
    #endregion
}