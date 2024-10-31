using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    public Canvas titleCanvas;
    public Canvas instructionCanvas;
    public Canvas cutSceneCanvas;
    public string scene;
    public Animator animator;
    public string fadeAnimationName = "cutscene_intro";
    public Canvas blackPanel;
    public static bool HasAlreadyPlayedCutscene = false;
    // Start is called before the first frame update
    void Start()
    {
        animator.Rebind();
        animator.ResetTrigger("StartFade"); // Clear the trigger

        // Now manually set the trigger for the cutscene to play again
        
        titleCanvas.gameObject.SetActive(true);
        blackPanel.gameObject.SetActive(false);
        instructionCanvas.gameObject.SetActive(false);
        cutSceneCanvas.gameObject.SetActive(false);
        Debug.Log("has scene loaded?" + HasAlreadyPlayedCutscene);

        //LoadTitleScene();
        ResetTutorial();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadGameScene()
    {
        Debug.Log("has scene loaded?" + HasAlreadyPlayedCutscene);
        titleCanvas.gameObject.SetActive(false);
        instructionCanvas.gameObject.SetActive(false);
        cutSceneCanvas.gameObject.SetActive(true);
        if (!HasAlreadyPlayedCutscene)
        {
            HasAlreadyPlayedCutscene = true;
            StartCoroutine(WaitForFadeAndLoadScene());
        }
        else 
        {
            SceneManager.LoadScene("Intro_scene", LoadSceneMode.Single);
        }
    }
    // Coroutine to handle fade and scene loading
    private IEnumerator WaitForFadeAndLoadScene()
    {       
            blackPanel.gameObject.SetActive(true);
            animator.SetTrigger("StartFade");
            yield return new WaitForSeconds(1f);
            blackPanel.gameObject.SetActive(false);
            yield return new WaitForSeconds(5.7f);
            
            // Load the game scene after the animation finishes
            SceneManager.LoadScene("Intro_scene", LoadSceneMode.Single);
            
        
    }
    public void LoadTitleScene()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }

    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey("TutorialComplete");
        PlayerPrefs.DeleteKey("MiniTutorial");
        PlayerPrefs.SetFloat("TotalGameTime", 0);
        PlayerPrefs.Save();
    }

    public void GoToInstruction()
    {
        titleCanvas.gameObject.SetActive(false);

        instructionCanvas.gameObject.SetActive(true);
    }

    public void GoToTitle()
    {
        titleCanvas.gameObject.SetActive(true);

        instructionCanvas.gameObject.SetActive(false);
    }
}
