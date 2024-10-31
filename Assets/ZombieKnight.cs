using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieKnight : MonoBehaviour
{
    public float slashInterval; // Interval before the enemy lunges
    public Transform player;
    public Transform arm;
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
    Animator animator;
    Rigidbody2D rb;

    public AudioClip roarSound;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        tutorialManager = FindAnyObjectByType<TutorialManager>();
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (player == null)
        {
            Debug.LogError("Player not found!");
        }
        StartCoroutine(LungeAtPlayer());
    }

    private bool withinRange()
    {
        return Vector2.Distance(transform.position, player.position) <= maxRange;
    }

    private void Update()
    {
        CheckDirection(); // Check if the enemy needs to turn around in every frame
    }

    private void CheckDirection()
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
            if (withinRange() && !isLunging)
            {
                EnemyAudioController.instance1.PlayRoarSound(roarSound);
                animator.SetTrigger("skill_2");
                yield return new WaitForSeconds(slashInterval);
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

        // Calculate the direction to the player
        Vector2 direction = (player.position - transform.position).normalized;

        // Start rotating the sword to 170 degrees during the lunge
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
    }

    private IEnumerator AttackWithSword()
    {
        animator.SetTrigger("skill_1");
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
        animator.Play("death");
        StartCoroutine(waitForAnimation());
    }

    IEnumerator waitForAnimation()
    {
        yield return new WaitForSeconds(deathAnimationInterval);
        Destroy(this.gameObject);
    }
}
