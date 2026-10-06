using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
     [Tooltip("Units per second the projectile travels.")]
    public float speed = 15f;

      [Tooltip("Seconds before the projectile is destroyed if it hits nothing.")]
    public float lifetime = 5f;
    public GameObject impactEffectPrefab;
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
            if(other.GetComponent<EnemyAI>() != null)
            {
                other.GetComponent<EnemyAI>().TakeDamage(50);

            }else if(other.GetComponent<SmallRobot>() != null)
            {
                other.GetComponent<SmallRobot>().TakeDamage(50);
            }

            GetComponent<MeshRenderer>().enabled = false;
            if (impactEffectPrefab != null)
            {
              var spark =  Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
              Destroy(spark, 0.5f);

            }
        }
        
        Destroy(gameObject, 0.5f);
    }
}
