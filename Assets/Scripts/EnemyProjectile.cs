using UnityEngine;

/// <summary>
/// Straight-line projectile fired by EnemyAI. Moves forward, self-destructs
/// after a lifetime, and logs a hit on the player. Wire this up to real
/// player health once that system exists — for now it just logs.
/// </summary>
[RequireComponent(typeof(Collider))]
public class EnemyProjectile : MonoBehaviour
{
    [Tooltip("Units per second the projectile travels.")]
    public float speed = 15f;

    [Tooltip("Seconds before the projectile is destroyed if it hits nothing.")]
    public float lifetime = 5f;

    [Tooltip("Placeholder damage value — hook this into player health later.")]
    public int damage = 10;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            // Ignore other enemies so they don't shoot each other on spawn overlap
            if (other.gameObject.tag != "Enemy")
            {
                // Hit level geometry or anything else — destroy on impact
                Destroy(gameObject);
            }
        }
    }
}