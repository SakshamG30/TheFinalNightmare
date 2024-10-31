using UnityEngine;
using UnityEngine.UI;

public class CheckpointManager : MonoBehaviour
{
    public int enemyCount = 0;  // Track the number of enemies killed
    public int requiredKills = 10;  // Number of kills required to open the door
    public GameObject openDoor;  // Assign the open door sprite in the Inspector
    public GameObject closedDoor;  // Assign the door's SpriteRenderer component in the Inspector
    public OpenDoor openDoorScript;  // Reference to the script on the open door
    private bool isEnemyCountActive = false;  // To track whether the enemy count is active
    private bool metRequirement;
    public GameObject exit;
    public Text tutorialText;
    public AudioSource door;

    public void Start()
    {
        // Disable the open door sprite by default
        closedDoor.SetActive(true);
        openDoor.SetActive(false);
        exit.SetActive(false);
    }

    private void Update()
    {
        if (isEnemyCountActive && !metRequirement)
        {
            tutorialText.text = $"Total number of enemies to kill: {requiredKills} \n Enemies Killed: {enemyCount}";
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 3)
        {
            isEnemyCountActive = true;
        }
    }
    public void OnEnemyKilled()
    {
        if (isEnemyCountActive)  // Only track kills after checkpoint is triggered
        {
            // Increment the enemy count
            enemyCount++;

            // Check if the player has killed enough enemies to open the door
            if (enemyCount >= requiredKills)
            {
                closedDoor.SetActive(false);  // Disable the closed door sprite
                // Inform the open door script that the door is open
                openDoor.SetActive(true);
                exit.SetActive(true);
                door.Play();
                metRequirement = true;
                tutorialText.text = $"Door has opened. You may proceed.";
                Debug.Log("All enemies defeated! Door is now open.");
            }
        }
    }
}
