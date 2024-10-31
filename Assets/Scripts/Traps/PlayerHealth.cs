using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private float currentHealth;

    void Start()
    {
        // Initialize player's health
        currentHealth = maxHealth;
    }

    // Method to reduce player's health over time
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage, current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Handle player's death
    private void Die()
    {
        Debug.Log("Player is dead!");
        // Handle player death (e.g., respawn, game over)
        // For now, just destroy the player object or trigger death animation
        Destroy(gameObject);
    }

    // Method to instantly kill the player
    public void InstantKill()
    {
        currentHealth = 0;
        Die();
    }

}