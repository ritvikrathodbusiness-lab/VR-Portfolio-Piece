using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] GameObject HitUI;
    [SerializeField] GameObject DieUI;
    [SerializeField] CharacterController characterController;


    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthSlider.value = currentHealth;

        HitUI.SetActive(false);
        HitUI.SetActive(true); // Show hit UI

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        DieUI.SetActive(true); // Show death UI
        characterController.enabled = false; // Disable player movement
        Debug.Log("Player has died.");
    }
}
