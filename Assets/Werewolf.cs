using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Werewolf : MonoBehaviour
{
    public float slashInterval; // Interval before the enemy lunges
    public Transform player;
    public float maxRange;
    public TextMesh enemyText;
    public float lungeSpeed;
    public float lungeDuration;
    private bool isLunging = false;
    public float restDuration = 10f; // Rest duration after each lunge
    public bool facingLeft = true; // To track the current facing direction of the enemy
    public bool isInverted = false;
    public string textToShow;
    public TutorialManager tutorialManager;
    public float attackAnimationInterval;
    public float deathAnimationInterval;
    public float retreatDistance = 2f; // How far the enemy moves back when hitting the shield
    public float retreatSpeed = 3f;
    public LogicScript logic;
    AstroShoot astro;
    AstroScript astroPlayer;
    Animator animator;
    public GameObject bossObj;
    Rigidbody2D rb;
    BossMovement bossMove;
    BossAttack boss;
    bool idle;
    private bool isRetreating = false;
    public AudioClip roarSound;
    public AudioClip lungeSound;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        astro = player.GetComponent<AstroShoot>();
        astroPlayer = player.GetComponent<AstroScript>();
        tutorialManager = FindAnyObjectByType<TutorialManager>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        StartCoroutine(LungeAtPlayer());
    }

    private bool withinRange()
    {
        return Vector2.Distance(transform.position, player.position) <= maxRange;
    }

    private void Update()
    {
        CheckDirection(); // Check if the enemy needs to turn around in every frame
        animator.SetBool("Idle", idle);
    }

    public void CheckDirection()
    {
        // Check if the player is on the left or right side of the enemy
        if (player.position.x > transform.position.x && facingLeft)
        {
            Flip();
        }
        else if (player.position.x <= transform.position.x && !facingLeft)
        {
            Flip();
        }
        AdjustText();
    }

    private void Flip()
    {
        // Flip the enemy by inverting its x scale
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;

        // Update facing direction
        facingLeft = !facingLeft;
    }

    private void AdjustText()
    {
        Vector3 textScale = enemyText.transform.localScale;
        if (facingLeft)
        {
            textScale.x = -Mathf.Abs(textScale.x);
        }
        else
        {
            textScale.x = Mathf.Abs(textScale.x);
        }
        if (isInverted)
        {
            textScale.y = -Mathf.Abs(textScale.y); // Flip the text vertically
        }
        else
        {
            textScale.y = Mathf.Abs(textScale.y);  // Keep the text upright
        }
        enemyText.transform.localScale = textScale;
    }

    private IEnumerator LungeAtPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(slashInterval);
            idle = true;
            if (withinRange() && !isLunging)
            {
                if(roarSound != null)
                    EnemyAudioController.instance1.PlayFireSound(roarSound);
                yield return new WaitForSeconds(slashInterval);
                idle = false;
                if (textToShow != "")
                {
                    StartCoroutine(ShowTextForSecond(textToShow));
                }
                StartCoroutine(Lunge());
            }
        }
    }

    private IEnumerator Lunge()
    {
        isLunging = true;
        idle = false;
        animator.SetTrigger("Run");
        // Calculate the direction to the player
        Vector2 direction = (player.position - transform.position).normalized;
        gameObject.layer = 11;
        // Start rotating the sword to 170 degrees during the lunge
        if(lungeSound != null)
            EnemyAudioController.instance1.PlayFireSound(lungeSound);
        StartCoroutine(AttackWithSword());

        // Lunge towards the player
        float elapsed = 0f;
        while (elapsed < lungeDuration)
        {
            Vector2 desiredPosition = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, desiredPosition, lungeSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isLunging = false;

        // Wait for the rest duration before lunging again
        yield return new WaitForSeconds(restDuration);
        gameObject.layer = 15;
    }

    private IEnumerator AttackWithSword()
    {
        animator.SetTrigger("AttackRun");
        yield return new WaitForSeconds(attackAnimationInterval);
    }

    public IEnumerator ShowTextForSecond(string text)
    {
        enemyText.text = text;
        yield return new WaitForSeconds(1);
        enemyText.text = "";
    }

    public void playDeathAnimation()
    {
        animator.Play("Werewolf Dead");
        StartCoroutine(waitForAnimation());
    }

    IEnumerator waitForAnimation()
    {
        yield return new WaitForSeconds(deathAnimationInterval);
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject parentObject = collision.gameObject.transform.root.gameObject;
        astro = player.GetComponent<AstroShoot>();
        Debug.Log("Hello");
        if (astro.isShieldActive)
        {
            Debug.Log("Hi");
            if (collision.gameObject.layer == 3 || collision.gameObject.layer == 10)
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

    public void Activate()
    {
        StartCoroutine(LungeAtPlayer());
    }
    private IEnumerator RetreatEnemy()
    {
        isRetreating = true;
        idle = true;
        Vector3 retreatDirection;

        retreatDirection = astroPlayer.facingRight ? Vector3.right : Vector3.left;

        Transform enemyTransform = transform.root;

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
