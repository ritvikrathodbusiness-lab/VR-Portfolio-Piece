using UnityEngine;

public class Gun : MonoBehaviour
{

   [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private Transform firePoint;
    
     public void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

}
