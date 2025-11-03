using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;

    public SpriteRenderer playerSr;
    public playerMovement playerMovement;

    // Play sound on damage taken
    public AudioSource damageSound;

    private void Start()
    {
        currentHealth = maxHealth;
        damageSound = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        damageSound.Play();
        currentHealth -= damage;
        // currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // Debug.Log($"Player took {damage} damage, current health: {currentHealth}");
        if (currentHealth <= 0)
        {
            playerSr.enabled = false;
            playerMovement.enabled = false;
        }
    }
}

