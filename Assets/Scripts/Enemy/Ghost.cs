using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float speed = 2f;                     // Floating speed of the Ghost
    public float playerDetectionRange = 10f;     // Range at which the Ghost detects the player
    public int health = 100;                     // Ghost's initial health
    public LayerMask playerLayer;                // Player layer for detecting the player
    public LayerMask magicWeaponLayer;           // Magic weapon layer for detecting hits

    private Transform player;                    // Reference to the player's transform
    private bool isPlayerInRange = false;        // True if the player is in detection range

    private void Start()
    {
        // Find the player in the scene by tag (assuming the player has a "Player" tag)
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogError("Player object not found. Make sure the player has the 'Player' tag.");
    }

    private void Update()
    {
        DetectPlayer();

        if (isPlayerInRange)
        {
            MoveTowardsPlayer();
        }
    }

    // Detect the player within the specified range
    private void DetectPlayer()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            isPlayerInRange = distanceToPlayer <= playerDetectionRange;
        }
    }

    // Move the Ghost toward the player, phasing through obstacles
    private void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    // Collision detection for magic weapons
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the ghost is hit by a magic weapon
        if (magicWeaponLayer == (magicWeaponLayer | (1 << collision.gameObject.layer)))
        {
            TakeDamage(50);  // Assume magic weapons deal 50 damage
        }
    }

    // Take damage when hit by magic weapons
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);  // Destroy the ghost if health drops to 0
        }
    }

    // Optional: Add visual effects when the ghost is damaged or destroyed
    private void OnDestroy()
    {
        // Placeholder for a death animation or sound effect
        Debug.Log("Ghost destroyed!");
    }
}

