using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodPool : MonoBehaviour
{
    public GameObject player; // Reference to the player
    public int damagePerSecond = 20; // Damage dealt per second while in the pool
    public float maxTimeInPool = 5f; // Maximum time the player can stay in the pool before instant death
    public bool causeInstantDeath = false; // Whether the pool causes instant death after maxTimeInPool

    private bool playerInPool = false; // Track if the player is in the blood pool
    private float timeInPool = 0f; // Track how long the player has been in the pool

    void Update()
    {
        // Continuously apply damage if the player is in the blood pool
        if (playerInPool)
        {
            timeInPool += Time.deltaTime;

            // Damage the player over time
            DealDamageOverTime();

            // If the player exceeds the max time in the pool and instant death is enabled, kill the player
            if (timeInPool >= maxTimeInPool && causeInstantDeath)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.InstantKill(); // Instantly kill the player
                }
            }
        }
    }

    // Deal damage to the player over time
    private void DealDamageOverTime()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damagePerSecond * Time.deltaTime); // Damage over time
        }
    }

    // When the player enters the blood pool
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            playerInPool = true;
            timeInPool = 0f; // Reset the timer when the player enters the pool
        }
    }

    // When the player exits the blood pool
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            playerInPool = false;
            timeInPool = 0f; // Reset the timer when the player exits the pool
        }
    }
}
