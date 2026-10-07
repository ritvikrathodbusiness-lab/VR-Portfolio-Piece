using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] GameObject muzzleFlashPrefab;

    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [Range(0f, 1f)] [SerializeField] private float shootVolume = 0.8f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        StartCoroutine(MuzzleFlash());

        if (shootClip != null)
        {
            audioSource.PlayOneShot(shootClip, shootVolume);
        }
    }

    IEnumerator MuzzleFlash()
    {
        muzzleFlashPrefab.SetActive(false);
        muzzleFlashPrefab.SetActive(true);
        yield return new WaitForSeconds(1f);
        muzzleFlashPrefab.SetActive(false);
    }
}