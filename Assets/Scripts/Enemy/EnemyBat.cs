using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBat : MonoBehaviour
{
    public float speed = 2f;                // Base speed of the bat
    public float erraticRange = 1f;         // Range of erratic movement
    public float infinityScale = 1f;        // Scale of the infinity movement
    public int health = 1;                  // Bats are weak and have low health
    public int damageToPlayer = 10;         // Damage dealt to the player on contact
    public float patrolDuration = 5f;       // Duration of the bat's patrol before changing direction

    private Vector2 originalPosition;       // Bat's original position to oscillate around
    private float patrolTimer = 0f;         // Timer to change direction

    private Rigidbody2D rb;                 // Rigidbody2D for physics-based movement
    private PlayerHealth playerHealth;      // Reference to player's health script

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;

        // Assign a reference to the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        // Handle bat's patrol movement
        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolDuration)
        {
            ChangeDirection();
            patrolTimer = 0f; // Reset patrol timer
        }

        // Generate infinity (lemniscate) pattern movement
        Vector2 infinityMovement = GenerateInfinityMovement(Time.time * speed);

        // Apply the infinity movement to the bat's position
        rb.MovePosition(originalPosition + infinityMovement);
    }

    // Change direction after a certain time to make movement unpredictable
    private void ChangeDirection()
    {
        // Reverse the direction
        speed = -speed;
    }

    // Function to generate infinity (lemniscate) pattern movement
    private Vector2 GenerateInfinityMovement(float t)
    {
        // Parametric equations for an infinity symbol (lemniscate)
        float x = Mathf.Sin(t) * infinityScale;
        float y = Mathf.Sin(t * 2f) * infinityScale / 2f; // Vertical component moves twice as fast

        return new Vector2(x, y);
    }

    // Handle collision with the player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerHealth != null)
        {
            // Deal damage to the player when bat collides with them
            playerHealth.TakeDamage(damageToPlayer);
        }
    }

    // Method to damage or kill the bat
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // Handle bat death
    private void Die()
    {
        Debug.Log("Bat killed!");
        // You could add a death animation or particle effect here
        Destroy(gameObject);
    }
}
