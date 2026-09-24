using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
     [Tooltip("Units per second the projectile travels.")]
    public float speed = 15f;

      [Tooltip("Seconds before the projectile is destroyed if it hits nothing.")]
    public float lifetime = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyAI>().TakeDamage(25f);
            Destroy(gameObject);
        }
    }
}
