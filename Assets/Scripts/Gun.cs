using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] GameObject muzzleFlashPrefab;
    
     public void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        StartCoroutine(MuzzleFlash());
    }

    IEnumerator MuzzleFlash()
    {
        muzzleFlashPrefab.SetActive(false);
        muzzleFlashPrefab.SetActive(true);
        yield return new WaitForSeconds(1f);
        muzzleFlashPrefab.SetActive(false);
    }

}
