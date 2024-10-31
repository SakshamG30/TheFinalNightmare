using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public Text tutorialText;
    private bool waitingForInput = false;
    public LogicScript logic;
    public bool tutorialComplete = false;
    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        tutorialComplete = PlayerPrefs.GetInt("TutorialComplete", 0) == 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator RunTutorials()
    {
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(StartTutorialShoot());
        yield return new WaitForSeconds(1.4f);
        yield return StartCoroutine(StartTutorialBlock());
        tutorialComplete = true;
        PlayerPrefs.SetInt("TutorialComplete", 1);
        PlayerPrefs.Save();
    }

    public IEnumerator StartTutorialShoot()
    {
        logic.PauseGame();

        ShowInstruction("Press Left Click to Shoot\nShoot the Red Junction box");

        yield return StartCoroutine(WaitForLeftClick());

        HideInstruction();

    }

    public IEnumerator StartTutorialBlock()
    {
        waitingForInput = false;
        logic.PauseGame();
        // Display the instruction text
        ShowInstruction("Press down Right Click to deploy Shield");

        yield return StartCoroutine(WaitForRightClick());

        HideInstruction();
    }

    public void ShowInstruction(string instruction)
    {
        tutorialText.text = instruction;
        tutorialText.enabled = true;
    }

    public void HideInstruction()
    {
        tutorialText.enabled = false; // Hide the text
    }

    IEnumerator WaitForLeftClick()
    {
        waitingForInput = true;

        // Wait until the player presses the left mouse button
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null; // Wait for the next frame
        }

        waitingForInput = false;
        logic.ResumeGame();
    }

    IEnumerator WaitForRightClick()
    {
        waitingForInput = true;

        // Wait until the player presses the left mouse button
        while (!Input.GetMouseButtonDown(1))
        {
            yield return null; // Wait for the next frame
        }

        waitingForInput = false;
        logic.ResumeGame();
    }

    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey("TutorialComplete");
        PlayerPrefs.Save();
    }
}
