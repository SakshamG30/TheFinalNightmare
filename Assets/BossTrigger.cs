using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public static class GameData
{
    public static Vector3 playerPosition;
}

public class BossTrigger : MonoBehaviour
{

    public float targetSize = 7.0f;       // Target camera size after collision
    public float transitionDuration = 2f;
    public GameObject blackBars;
    public GameObject bossHealthBar;
    GameObject player;
    GameObject boss;
    public Vector2 playerTargetPosition;
    public float playerMoveDuration = 2f;
    public float dialogueDuration = 8f;
    AstroScript astro;
    BossMovement bossMove;
    GameObject logicObj;
    LogicScript logic;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        astro = player.GetComponent<AstroScript>();
        boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss != null)
            bossMove = boss.GetComponent<BossMovement>();
        if (SceneManager.GetActiveScene().name == "BossScene")
        {
            player.transform.position = GameData.playerPosition;
        }
        logicObj = GameObject.FindGameObjectWithTag("Logic");

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.layer == 3) || (collision.gameObject.layer == 0))
        {
            Debug.Log("Collide");
            if (astro.isGravityInverted)
            {
                astro.InvertGravity();
                astro.bats.SetActive(false);
                astro.heroRenderer.enabled = true;
                astro.Flip();
            }
            blackBars.SetActive(true);
            bossHealthBar.SetActive(true);
            StartCoroutine(ChangeCameraSize(targetSize, transitionDuration));
            StartCoroutine(MovePlayer(playerTargetPosition, playerMoveDuration));
        }
    }

    private IEnumerator ChangeCameraSize(float targetSize, float duration)
    {
        Camera mainCamera = Camera.main;
        float startSize = mainCamera.orthographicSize;
        float elapsed = 0f;
        astro.animator.SetTrigger("HBoost");

        while (elapsed < duration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final size is set precisely
        mainCamera.orthographicSize = targetSize;
    }

    private IEnumerator MovePlayer(Vector2 targetPosition, float duration)
    {
        astro.cutSceneEnabled = true;
        astro.animator.SetTrigger("HBoost");
        Vector3 startPosition = player.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            player.transform.position = Vector2.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.transform.position = targetPosition; // Ensure player reaches the target position precisely

        astro.cutSceneEnabled = false;
        astro.isGrounded = true;
        astro.animator.SetBool("IsGrounded", astro.isGrounded);
        StartCoroutine(BeginDialogue(dialogueDuration));
        
    }

    private IEnumerator BeginDialogue(float duration)
    {
        Debug.Log("Hi");
        
        astro.cutSceneEnabled = true;
        bossMove.cutSceneEnabled = true;
        float elapsed = 0f;
        StartCoroutine(bossMove.ShowTextForSecond("You've come to challenge me?", 3f));
        bool flag = true;
        while (elapsed < duration)
        {
            
            elapsed += Time.deltaTime;
            if (elapsed > 5f && flag)
            {
                flag = false;
                StartCoroutine(bossMove.ShowTextForSecond("But it will be I! The Inquisitor, who will vanquish you, vampire!", 3f));
            }
            yield return null;
        }
        astro.cutSceneEnabled = false;
        bossMove.cutSceneEnabled = false;
        blackBars.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        GameData.playerPosition = player.transform.position;
        if (logicObj != null)
        {
            logic = logicObj.GetComponent<LogicScript>();
            logic.LoadNextLevel("BossScene");
        }
        else
            SceneManager.LoadScene("BossScene", LoadSceneMode.Single);
    }
}
