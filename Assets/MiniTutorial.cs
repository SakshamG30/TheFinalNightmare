using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniTutorial : MonoBehaviour
{
    public Text tutorialText;
    public GameObject player;
    public float waitAfterPressZ = 2f;
    LogicScript logic;
    AstroScript astro;

    private bool tutorialActive = true;

    private void Start()
    {
        // Check if the tutorial has been completed
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
        {
            // If completed, skip the tutorial
            tutorialActive = false;
            enabled = false;
            return;
        }

        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        astro = player.GetComponent<AstroScript>();
        StartCoroutine(TutorialSequence()); // Start the tutorial sequence
    }

    private IEnumerator TutorialSequence()
    {
        astro.cutSceneEnabled = true;
        // Step 1: Show instruction to press Z
        tutorialText.text = "You have a new ability. Press 'Z' to summon hell fire bombs!";

        // Wait for the player to press Z
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        astro.cutSceneEnabled = false;
        // Wait for animation to play out
        yield return new WaitForSeconds(waitAfterPressZ);
        astro.cutSceneEnabled = true;
        // Step 3: Instruct the player to press Z again
        tutorialText.text = "Now Press 'Z' again to rain down hellfire!";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        astro.cutSceneEnabled = false;
        tutorialText.text = "All good to go!";
        // Wait for a brief moment before resuming
        yield return new WaitForSecondsRealtime(waitAfterPressZ);
        tutorialText.text = "";

        // Mark tutorial as completed in PlayerPrefs
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();

        tutorialActive = false;
    }

    private void Update()
    {
        // Optional: Disable this script after the tutorial is complete
        if (!tutorialActive)
        {
            enabled = false;
        }
    }
}
