using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Next_Level : MonoBehaviour
{
    public Canvas cutSceneCanvas;
    public Canvas blackCanvas;
    public Animator animator;
    GameObject logicObj;
    LogicScript logic;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        blackCanvas.gameObject.SetActive(false);
        cutSceneCanvas.gameObject.SetActive(true);
        logicObj = GameObject.FindGameObjectWithTag("Logic");

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.layer == 3) || (collision.gameObject.layer == 10) || (collision.gameObject.layer == 0))
        {
            Coroutine_function();
        }
    }
    public void Coroutine_function()
    {
        StartCoroutine(WaitForFadeAndLoadScene());
    }

    private IEnumerator WaitForFadeAndLoadScene()
    {
        blackCanvas.gameObject.SetActive(true);
        animator.SetTrigger("StartFade");
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(6f);
        if (logicObj != null)
        {
            logic = logicObj.GetComponent<LogicScript>();
            logic.LoadNextLevel("Aditya_scene");
        }
        else
            SceneManager.LoadScene("Aditya_scene", LoadSceneMode.Single);
    }
}
