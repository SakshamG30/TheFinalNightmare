using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer_1 : MonoBehaviour
{
    public Transform destinationDoor; // Assign the other door's Transform in the Inspector
    
    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.layer == 3) || (collision.gameObject.layer == 7) || (collision.gameObject.layer == 10) || (collision.gameObject.layer == 0))   // Check for Player or Enemy collision
        {
            Teleport(collision.gameObject.transform.root.gameObject); // Call the teleport function when the player enters
        }
    }

    private void Teleport(GameObject player)
    {
        // Move the player to the position of the destination door
        player.transform.position = destinationDoor.position;

    }
}
