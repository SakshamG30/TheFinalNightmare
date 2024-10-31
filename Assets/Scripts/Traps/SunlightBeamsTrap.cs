using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunlightBeamsTrap : MonoBehaviour
{
    // Variables for movement
    public float moveSpeed = 2f; // Speed of the beam's movement
    public float moveDistance = 5f; // Distance the beam moves back and forth
    private Vector3 initialPosition; // Starting position of the beam
    private bool movingRight = true; // Direction of movement

    // Rotation variables
    public float rotationSpeed = 90f; // Speed of rotation in degrees per second
    private float currentRotationZ;   // Current Z rotation of the beam
    private bool rotatingClockwise = true; // Flag for direction of rotation

    // Damage variables
    public float damagePerSecond = 25f; // How much damage the beam does per second
    public bool instantDeath = false; // Toggle for instant death
    private bool playerInBeam = false; // Is the player currently in the sunlight beam?

    // Reference to the player object
    private GameObject player;
    private PlayerHealth playerHealth; // Script handling the player's health

    void Start()
    {
        // Save the initial position of the beam
        initialPosition = transform.position;

        

        // Find the player object and its health script (assuming the player has a tag "Player")
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        // Move the sunlight beam in a predictable pattern (back and forth)
        //MoveBeam();

        RotateBeam();

        // If the player is in the beam, apply damage
        if (playerInBeam && playerHealth != null)
        {
            ApplyDamage();
        }
    }

    // Function to move the sunlight beam
    void MoveBeam()
    {
        if (movingRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            if (transform.position.x >= initialPosition.x + moveDistance)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            if (transform.position.x <= initialPosition.x - moveDistance)
            {
                movingRight = true;
            }
        }
    }

    // Function to rotate the sunlight beam between +90 and -90 degrees
    void RotateBeam()
    {
        // Update the current rotation angle based on the direction
        if (rotatingClockwise)
        {
            currentRotationZ += rotationSpeed * Time.deltaTime;
            if (currentRotationZ >= 90f)
            {
                currentRotationZ = 90f; // Clamp to 90 degrees
                rotatingClockwise = false; // Switch direction
            }
        }
        else
        {
            currentRotationZ -= rotationSpeed * Time.deltaTime;
            if (currentRotationZ <= -90f)
            {
                currentRotationZ = -90f; // Clamp to -90 degrees
                rotatingClockwise = true; // Switch direction
            }
        }

        // Apply the rotation to the beam
        transform.rotation = Quaternion.Euler(0f, 0f, currentRotationZ);
    }

    // Function to apply damage to the player
    void ApplyDamage()
    {
        if (instantDeath)
        {
            // If instant death is enabled, kill the player immediately
            playerHealth.TakeDamage(playerHealth.maxHealth);
        }
        else
        {
            // Gradually reduce player's health based on damage per second
            playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    // Detect when the player enters the sunlight beam
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            playerInBeam = true;
        }
    }

    // Detect when the player exits the sunlight beam
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            playerInBeam = false;
        }
    }

    // Function to trigger the beams when a door or window is opened
    public void TriggerBeam()
    {
        // You can add conditions here to activate the sunlight beams when certain doors/windows are opened
        // For now, we simply enable the beam if triggered
        gameObject.SetActive(true);
    }

    // Deactivate the beam (used if the player closes a door/window)
    public void DeactivateBeam()
    {
        gameObject.SetActive(false);
    }
}
