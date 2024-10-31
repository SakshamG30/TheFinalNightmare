using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public LogicScript logic;
    public AstroShoot astro;
    public AstroScript astroPlayer;
    public int ShieldLayer = 10;
    public float retreatDistance = 2f; // How far the enemy moves back when hitting the shield
    public float retreatSpeed = 3f;    // Speed of the retreat movement
    private bool isRetreating = false; // Flag to prevent multiple retreats at the same time
    BossMovement boss;
    public int damageToBoss = 20;
    GameObject checkpointObj;
    public CheckpointManager checkpointManager;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        astro = GameObject.FindGameObjectWithTag("Player").GetComponent<AstroShoot>();
        astroPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<AstroScript>();

        GameObject bossObject = GameObject.FindGameObjectWithTag("Boss");
        if (bossObject != null)
        {
            boss = bossObject.GetComponent<BossMovement>();
        }
        else
        {
            Debug.Log("Boss not present.");
        }
    }
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject parentObject = collision.gameObject.transform.root.gameObject;
        if (collision.gameObject.layer == 7)
        {
            checkpointObj = GameObject.Find("CheckPoint");
            if (checkpointObj != null)
            {
                checkpointManager = checkpointObj.GetComponent<CheckpointManager>();
                checkpointManager.OnEnemyKilled();
            }
            astro.AddBullet();
            StartCoroutine(astro.ShowTextForSecond("Fatality!"));
            Destroy(parentObject);
        }
        if (collision.gameObject.layer == 15)
        {
            StartCoroutine(astro.ShowTextForSecond("Nice Hit!"));
            boss.isHurt(damageToBoss);
        }
        if (astro.isShieldActive)
        {
            if (collision.gameObject.layer == 3 || collision.gameObject.layer == ShieldLayer)
            {
                if (!isRetreating)
                {
                    // Start retreating the enemy
                    StartCoroutine(RetreatEnemy());
                } 
            }
        }
        else
        {
            // If the shield is not active, game over
            if (collision.gameObject.layer == 3)
            {
                logic.GameOver();
            }
        }
    }

    private IEnumerator RetreatEnemy()
    {
        isRetreating = true;
        Vector3 retreatDirection;

        Transform enemyTransform = transform.root;

        if (!astroPlayer.isGravityInverted)
        {
            retreatDirection = astroPlayer.facingRight ? Vector3.right : Vector3.left;
        }
        else
        {
            retreatDirection = astroPlayer.facingRight ? Vector3.left : Vector3.right;
        }

        Vector3 targetPosition = enemyTransform.position + (retreatDirection * retreatDistance);

        while (Vector3.Distance(enemyTransform.position, targetPosition) > 0.1f)
        {
            enemyTransform.position = Vector3.MoveTowards(enemyTransform.position, targetPosition, retreatSpeed * Time.deltaTime);
            yield return null;
        }

        enemyTransform.position = targetPosition;

        isRetreating = false;
    }
}
