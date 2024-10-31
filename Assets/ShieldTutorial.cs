using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldTutorial : MonoBehaviour
{
    public TutorialManager tutorialManager;
    public LogicScript logic;
    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            StartCoroutine(StartTutorialShield());
        }
    }
    public IEnumerator StartTutorialShield()
    {
        logic.PauseGame();

        tutorialManager.ShowInstruction("Either flank this shielded enemy, or blow past him using your lance by pressing shift!");

        // Wait until any key is pressed
        yield return new WaitUntil(() => Input.anyKeyDown);

        // Resume game and hide instruction after a key is pressed
        logic.ResumeGame();
        tutorialManager.HideInstruction();
    }

}
