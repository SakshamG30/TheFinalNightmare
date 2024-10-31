using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyWaterSprinkler : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public float sprayDuration = 2f; // Duration of each holy water spray
    public float sprayInterval = 2; // Time between sprays
    public int damagePerSecond = 10; // Damage dealt per second
    public bool isTriggeredByPlayer = false; // Whether the trap is triggered by the player entering an area

    private bool isSpraying = false; // To track if the sprinkler is currently spraying
    private Collider2D sprayArea; // Collider representing the area affected by the sprinkler
    private bool playerInSpray = false; // To track if the player is in the spray area

    void Start()
    {
        // Get the Collider2D component that defines the spray area
        sprayArea = GetComponent<Collider2D>();

        // Start the sprinkler if it's not triggered by the player
        if (!isTriggeredByPlayer)
        {
            StartCoroutine(SprinklerRoutine());
        }
    }

    // Handle periodic spraying of holy water
    private IEnumerator SprinklerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(sprayInterval); // Wait before starting the spray

            StartSpraying();

            yield return new WaitForSeconds(sprayDuration); // Spray for the specified duration

            StopSpraying();
        }
    }

    // Function to start spraying holy water
    private void StartSpraying()
    {
        Debug.Log("Holy water spraying!");
        isSpraying = true;

        // Optionally play spray animation or particle effects here

        // Continuously deal damage to player if they are in the spray while it's active
        if (playerInSpray)
        {
            StartCoroutine(DealDamageOverTime());
        }
    }

    // Function to stop spraying holy water
    private void StopSpraying()
    {
        Debug.Log("Holy water stopped.");
        isSpraying = false;

        // Optionally stop animation or particle effects here
    }

    // Coroutine to deal continuous damage while the player is in the spray
    private IEnumerator DealDamageOverTime()
    {
        while (isSpraying && playerInSpray)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damagePerSecond);
            }

            // Wait for 1 second before applying damage again
            yield return new WaitForSeconds(1f);
        }
    }

    // Trigger the sprinkler when the player enters the trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            playerInSpray = true;

            // If the sprinkler is triggered by player proximity, start spraying
            if (isTriggeredByPlayer && !isSpraying)
            {
                StartSpraying();
                StartCoroutine(StopSprayingAfterDelay());
            }

            // Start dealing damage immediately if the sprinkler is already spraying
            if (isSpraying)
            {
                StartCoroutine(DealDamageOverTime());
            }
        }
    }

    // Stop damaging the player when they leave the spray area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            playerInSpray = false;
            StopAllCoroutines(); // Stop the damage-over-time coroutine
        }
    }

    // If triggered by the player, stop spraying after the spray duration
    private IEnumerator StopSprayingAfterDelay()
    {
        yield return new WaitForSeconds(sprayDuration);
        StopSpraying();
    }
}
