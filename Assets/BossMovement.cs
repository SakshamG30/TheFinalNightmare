using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public float retreatSpeed;
    public float retreatDuration = 1f;
    public float animationInterval;
    public Transform player;
    private Transform shootingPoint;
    public float maxRange;
    public TextMesh enemyText;
    public string textToShow;
    public bool isInverted = false;
    public Animator animator;
    public bool facingLeft = true; // To track the current facing direction of the enemy
    public float deathAnimationDuration;
    private bool tooClose;
    private bool edgeOfGround;
    bool retreatDirectionBool;
    public LayerMask groundLayer;
    private Rigidbody2D rb;
    private float edgeDetectionDistance = 1.5f;
    public float teleportAnimationDuration = 0.3f;
    public int maxHealth = 100;
    public int currentHealth;
    public float transformationDuration = 0.5f;
    public string deathLine;
    public OpenPillar pillar;
    
    bool groundFound;

    public BossHealthBar healthBar;

    public bool cutSceneEnabled;

    public AudioClip teleportSound;
    public AudioClip feetLandingSound;

    bool flag;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shootingPoint = transform.Find("ShootingPoint");
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        if (shootingPoint == null)
        {
            Debug.LogError("gunPoint is not found!");
        }
        if (player == null)
        {
            Debug.LogError("Player not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cutSceneEnabled)
        {
            return;
        }
        shootingPoint = transform.Find("ShootingPoint");
        if (!tooClose && flag)
        {
            // Debug.Log("Checking Direction");
            CheckDirection();
        }
        Retreat();
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

    public IEnumerator ShowTextForSecond(string text, float duration = 1f)
    {
        enemyText.text = text;

        yield return new WaitForSeconds(duration);
        enemyText.text = "";
    }

    private void AdjustText()
    {
        Vector3 textScale = enemyText.transform.localScale;
        if (facingLeft)
        {
            textScale.x = Mathf.Abs(textScale.x);
        }
        else
        {
            textScale.x = -Mathf.Abs(textScale.x);
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

    private void Flip()
    {
        // Flip the enemy by inverting its x scale
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
        facingLeft = !facingLeft;
        AdjustText();
    }

    public void playDeathAnimation()
    {
        animator.Play("Inquisitor Death");
        StartCoroutine(waitForAnimation(deathAnimationDuration));
    }

    public IEnumerator waitForAnimation(float duration, string trigger = null)
    {
        if (trigger != null)
        {
            Debug.Log(trigger); 
            animator.SetTrigger(trigger);
        }
        yield return new WaitForSeconds(duration);
        if (duration == deathAnimationDuration)
            Destroy(this.gameObject);
    }

    private bool WithinRange(float maxRange)
    {
        return Vector2.Distance(transform.position, player.position) <= maxRange;
    }

    private void Retreat()
    {
        // float val = Random.Range(0f, 1f);
        animator.SetBool("TooClose", tooClose);
        if (WithinRange(maxRange / 2) && flag && !edgeOfGround)
        {   
            Flip();
            flag = false;
            tooClose = true;
            retreatDirectionBool = facingLeft;
            StartCoroutine(StartRunning());
        }
        else
        {
            tooClose = false;

            if (edgeOfGround)
            {
                if (PlayerHasFlanked())
                {
                    edgeOfGround = false;

                    if (retreatDirectionBool != facingLeft)
                    {
                        Flip();
                    }
                }
                else
                {
                    flag = false;
                }
            }
            else
                flag = true;
        }
    }

    private bool PlayerHasFlanked()
    {
        return (!retreatDirectionBool && player.position.x > transform.position.x) || (retreatDirectionBool && player.position.x < transform.position.x);
    }

    IEnumerator StartRunning()
    {

        float elapsedTime = 0f;
        Vector2 retreatDirection = facingLeft ? Vector2.left : Vector2.right;

        while (elapsedTime < retreatDuration && tooClose)
        {
            Vector2 rayOrigin = (Vector2)transform.position + new Vector2(retreatDirection.x * 0.4f, 0);

            RaycastHit2D groundInfo = Physics2D.Raycast(rayOrigin, Vector2.down, edgeDetectionDistance, groundLayer);
            Debug.DrawRay(rayOrigin, Vector2.down * edgeDetectionDistance, Color.red);
            if (groundInfo.collider == null)
            {
                edgeOfGround = true;
                break;
            }

            // Move in the retreat direction if ground is detected
            transform.Translate(retreatDirection * Time.deltaTime * retreatSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Wait for the rest duration after moving
        yield return new WaitForSeconds(animationInterval);

        if (edgeOfGround)
        {
            OnEdgeOfGroundDetected();
        }

    }

    private void OnEdgeOfGroundDetected()
    {
        StartCoroutine(ScanningGround(2.5f));
    }

    IEnumerator ScanningGround(float duration)
    {
        float forwardDetectionDistance = 15f;
        Vector2 forwardDirection = facingLeft ? Vector2.left : Vector2.right;

        float maxAngle = 30f; // Maximum angle to each side
        float angleIncrement = 10f; // Angle step for each ray
        bool scanAbove = true;

        for (float angle = 0; angle <= maxAngle; angle += angleIncrement)
        {
            float adjustedAngle = scanAbove ? angle : -angle;
            adjustedAngle *= facingLeft ? -1 : 1;

            Vector2 rayDirection = Quaternion.Euler(0, 0, adjustedAngle) * forwardDirection;

            RaycastHit2D forwardGroundInfo = Physics2D.Raycast(transform.position, rayDirection, forwardDetectionDistance, groundLayer);

            Debug.DrawRay(transform.position, rayDirection * forwardDetectionDistance, Color.blue);

            // If ground is detected in this direction
            if (forwardGroundInfo.collider != null)
            {
                StartCoroutine(WaitForAnimationAndTranslate(transformationDuration, "Transform", forwardGroundInfo, forwardDirection));
                EnemyAudioController.instance1.PlayFireSound(feetLandingSound);

                groundFound = true;
            }
            scanAbove = !scanAbove;
        }
        yield return new WaitForSeconds(duration);

        flag = true;
    }


    private IEnumerator WaitForAnimationAndTranslate(float duration, string trigger, RaycastHit2D forwardGroundInfo, Vector2 forwardDirection)
    {

        
        // Get ground object and transform information after waiting
        GameObject groundObject = forwardGroundInfo.collider.gameObject;
        Transform groundTransform = groundObject.transform;
        float groundHeight = groundObject.GetComponent<Collider2D>().bounds.size.y;
        float groundWidth = groundObject.GetComponent<Collider2D>().bounds.size.x;

        // Determine the start position based on retreat direction
        Vector3 startOfGround = forwardDirection == Vector2.left
            ? new Vector3(groundTransform.position.x + groundWidth / 2.2f, groundTransform.position.y, transform.position.z)
            : new Vector3(groundTransform.position.x - groundWidth / 2.2f, groundTransform.position.y, transform.position.z);

        if (trigger != null)
        {
            rb.isKinematic = true;
            animator.SetTrigger(trigger);
        }

        // Wait for the animation duration
        yield return new WaitForSeconds(duration);
        EnemyAudioController.instance1.PlayFireSound(teleportSound);
        rb.isKinematic = false;
        // Move the transform after the animation completes
        transform.position = new Vector3(startOfGround.x, startOfGround.y + groundHeight * 2f, transform.position.z);

        edgeOfGround = false;

        // Trigger the animation
        
    }

    public void isHurt(int points)
    {
        if(currentHealth >= 0)
        {
            if (points >= 30)
            {
                StartCoroutine(HandleHurtAndCheckDeath(points));
            }
            else
            {
                TakeDamage(points);
                isDead();
            }
        }
    }

    private IEnumerator HandleHurtAndCheckDeath(int points)
    {
        yield return StartCoroutine(ShowTextForSecond("AUGH")); // Show "AUGH" first
        TakeDamage(points); // Apply damage after "AUGH" finishes
        isDead(); // Then check if the character is dead
    }
    public void isDead()
    {
        if (currentHealth <= 0)
        {
            StartCoroutine(ShowTextForSecond(deathLine));
            playDeathAnimation();
            pillar.Open();

        }
        else
        {
            animator.SetTrigger("Hurt");
            StartCoroutine(waitForAnimation(0.2f));
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
    }
}
