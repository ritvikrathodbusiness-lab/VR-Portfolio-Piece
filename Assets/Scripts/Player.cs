using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] GameObject HitUI;
    [SerializeField] GameObject DieUI;
    [SerializeField] CharacterController characterController;
    [SerializeField] TextMeshProUGUI countDownText;

    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Audio")]
    [SerializeField] private AudioClip hitClip;
    [Range(0f, 1f)] [SerializeField] private float hitVolume = 0.8f;

    bool isDead = false;

    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthSlider.value = currentHealth;

        HitUI.SetActive(false);
        HitUI.SetActive(true); // Show hit UI

        if (hitClip != null)
        {
            audioSource.PlayOneShot(hitClip, hitVolume);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Prevent multiple death triggers
        isDead = true;
        DieUI.SetActive(true); // Show death UI
        characterController.enabled = false; // Disable player movement
        StartCoroutine(CountdownToRestart(8f)); // Start countdown to restart
        Debug.Log("Player has died.");
    }

    IEnumerator CountdownToRestart(float countdownTime)
    {
        float remainingTime = countdownTime;
        while (remainingTime > 0)
        {
            countDownText.text = "Restarting in: " + Mathf.Ceil(remainingTime).ToString();
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }
        // Restart the game or load the scene here
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}