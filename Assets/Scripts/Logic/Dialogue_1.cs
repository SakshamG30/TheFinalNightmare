using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Dialogue_1 : MonoBehaviour
{
    public Canvas dialogueCanvas; // Assign the dialogue UI (e.g., a Panel with Text inside)
    public float displayDuration = 4f; // Time to display the dialogue
    AstroScript astroScript;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            astroScript = player.GetComponent<AstroScript>();
        }
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        // Ensure the dialogue box is hidden at the start
        dialogueCanvas.gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3) // Make sure the object is the "Player"
        {
            StartCoroutine(ShowDialogue());
        }
    }


    private IEnumerator ShowDialogue()
    {
        // Show the dialogue box
        dialogueCanvas.gameObject.SetActive(true);
        astroScript.animator.SetFloat("Velocity", 0);
        astroScript.cutSceneEnabled = true;
        // Wait for the specified display duration
        yield return new WaitForSeconds(displayDuration);
        astroScript.cutSceneEnabled = false;
        // Hide the dialogue box after the duration
        dialogueCanvas.gameObject.SetActive(false);
    }
}
