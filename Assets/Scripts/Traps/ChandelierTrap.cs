using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ChandelierTrap : MonoBehaviour
{
    // Public variables to set in Unity Editor
    public GameObject player; // Reference to the player GameObject
    public float fallSpeed = 10f; // Speed at which chandelier falls
    public float triggerDelay = 1f; // Delay before chandelier starts falling
    public bool isTriggeredBySwitch = false; // If the chandelier is triggered by a switch
    public int damage = 100; // Amount of damage chandelier does (adjust for instant kill or heavy damage)

    private bool isFalling = false; // To track if the chandelier is falling
    private Rigidbody2D rb; // Rigidbody2D component of the chandelier
    private Collider2D proximityTrigger; // Proximity trigger collider

    void Start()
    {
        // Get references to components
        rb = GetComponent<Rigidbody2D>();
        proximityTrigger = GetComponent<Collider2D>();

        // Ensure the Rigidbody2D is set to Kinematic so it doesn't fall by default
        rb.isKinematic = true;
    }

    // This method will trigger the chandelier falling either by proximity or switch
    public void TriggerChandelier()
    {
        if (!isFalling)
        {
            StartCoroutine(ChandelierFall());
        }
    }

    // Coroutine to handle the falling of the chandelier
    private IEnumerator ChandelierFall()
    {
        isFalling = true;

        // Wait for the specified delay before the chandelier starts falling
        yield return new WaitForSeconds(triggerDelay);

        // Set the Rigidbody2D to dynamic so it starts falling
        rb.isKinematic = false;
        rb.linearVelocity = new Vector2(0, -fallSpeed); // Set falling speed
    }

    // Detect when the player enters the proximity trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player && !isTriggeredBySwitch)
        {
            TriggerChandelier(); // Trigger the chandelier when the player walks underneath
        }
    }

    // Handle collision with the player to apply damage
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player)
        {
            // Apply damage or kill the player
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Optionally, destroy the chandelier after it hits the player
            Destroy(gameObject);
        }
    }
}
