using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperBlock : MonoBehaviour
{
    public float moveSpeed = 2f;
    public LogicScript logic;
    private bool lowerGroundReached = false;
    private bool playerTriggered = false; // Flag to check if player triggered the movement
    private bool hitMiddle = false;

    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTriggered && !lowerGroundReached)
        {
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
        }
    }

    // Trigger detection for the player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.layer==3) || (collision.gameObject.layer==0))
        {
            playerTriggered = true; // Start moving the block
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if it collides with the middle block (layer 12)
        if (collision.gameObject.layer == 12)
        {
            hitMiddle = true;
            OnMiddleBlockCollision(collision.gameObject);
        }
        // Check if it collides with the enemy (layer 7)
        else if ((collision.gameObject.layer == 7) || (collision.gameObject.layer == 0))
        {
            OnEnemyCollision(collision.gameObject);
        }
        else if (collision.gameObject.layer == 6)
        {
            Debug.Log("Upper block collided with the ground!");
            lowerGroundReached = true;
        }
        if ((collision.gameObject.layer == 3) || (collision.gameObject.layer == 10))
        { 
            if (hitMiddle)
            {
                logic.GameOver();
            }
        }
    }

    private void OnMiddleBlockCollision(GameObject middle)
    {
        Destroy(middle);
        hitMiddle = true;
    }

    // Event triggered on collision with enemy
    private void OnEnemyCollision(GameObject enemy)
    {
        Debug.Log("Upper block collided with an enemy, destroying it!");
        Destroy(enemy.transform.root.gameObject);
    }
}
