using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Archer : MonoBehaviour
{

    public GameObject arrowPrefab;
    public float arrowSpeed;
    public float retreatSpeed;
    public float retreatDuration = 2f;
    public float shootingInterval;
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
    bool retreatDirection;
    public LayerMask groundLayer;
    private Rigidbody2D rb;
    private float edgeDetectionDistance = 1.5f;
    float jumpDistanceMultiplier = 1.5f;
    public float horizontalDistanceRate = 0.5f;
    public float verticalDistanceRate = 0.5f;
    public float jumpAnimationDuration = 0.3f;
    bool flag;
    public bool cutSceneEnabled;
    public AudioClip arrowSound;

    bool isGrounded = true;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shootingPoint = transform.Find("ShootingPoint");
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (arrowPrefab == null)
        {
            Debug.LogError("fireballPrefab is not assigned!");
        }
        if (shootingPoint == null)
        {
            Debug.LogError("gunPoint is not found!");
        }
        if (player == null)
        {
            Debug.LogError("Player not found!");
        }
        
        AimAtPlayer();

        StartCoroutine(ShootAtPlayer());
    }
    private bool WithinRange(float maxRange)
    {
        return Vector2.Distance(transform.position, player.position) <= maxRange;
    }
    // Update is called once per frame
    void Update()
    {
        shootingPoint = transform.Find("ShootingPoint");
        if (!tooClose && flag && !edgeOfGround) 
            CheckDirection();

        Retreat();
    }

    private IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootingInterval);

            if (WithinRange(maxRange) && !tooClose && !cutSceneEnabled)
            {
                if (textToShow != "")
                {
                    StartCoroutine(ShowTextForSecond(textToShow));
                }
                if (IsPlayerBelowBy45Degrees())
                {
                    animator.SetTrigger("LowerFire");
                }
                else
                {
                    animator.SetTrigger("Fire");
                }
                yield return new WaitForSeconds(animationInterval);
                Shoot();
            }
        }
    }

    private bool IsPlayerBelowBy45Degrees()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float angleToPlayer = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angleToPlayer > -145f && angleToPlayer < -35;
    }
    private void AimAtPlayer()
    {
        if (player != null && shootingPoint != null)
        {
            Vector2 direction = (player.position - shootingPoint.position).normalized;
            bool stance = false;
            if (transform.localScale.y < 0f)
            {
                stance = true;
            }

            float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            angle += stance ? 270f : 90f;

            shootingPoint.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
    private void Shoot()
    {   
        EnemyAudioController.instance1.PlayFireSound(arrowSound);
        GameObject arrow = Instantiate(arrowPrefab, shootingPoint.position, Quaternion.identity);
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        arrowScript.shooter = this.gameObject;
        float randomVerticalDistance = Random.Range(0f, 0.8f);
        Vector2 targetPosition = new Vector2(player.position.x, player.position.y + randomVerticalDistance);
        Vector2 direction = (targetPosition - (Vector2)shootingPoint.position).normalized;

        // Set rotation based on the direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);


        Rigidbody2D bulletRb = arrow.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = direction * arrowSpeed;
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
    public IEnumerator ShowTextForSecond(string text)
    {
        enemyText.text = text;

        yield return new WaitForSeconds(1);
        enemyText.text = "";
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
        animator.Play("Archer Death");
        StartCoroutine(waitForAnimation(deathAnimationDuration));
    }

    IEnumerator waitForAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (duration == deathAnimationDuration)
        {
            Destroy(this.gameObject);
        }
    }

    private void Retreat()
    {
        // float val = Random.Range(0f, 1f);
        animator.SetBool("TooClose", tooClose);
        if (WithinRange(maxRange / 2) && flag && !edgeOfGround)
        {
            flag = false;
            tooClose = true;
            Flip();
            retreatDirection = facingLeft;
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

                    if (retreatDirection != facingLeft)
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
        return (!retreatDirection && player.position.x > transform.position.x) || (retreatDirection && player.position.x < transform.position.x);
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

        float maxAngle = 60f; // Maximum angle to each side
        float angleIncrement = 10f; // Angle step for each ray
        bool scanAbove = true;

        for (float angle = 0; angle <= maxAngle; angle += angleIncrement)
        {
            float adjustedAngle = scanAbove ? angle : -angle;
            adjustedAngle *= facingLeft ? -1: 1;
            
            Vector2 rayDirection = Quaternion.Euler(0, 0, adjustedAngle) * forwardDirection;

            RaycastHit2D forwardGroundInfo = Physics2D.Raycast(transform.position, rayDirection, forwardDetectionDistance, groundLayer);

            Debug.DrawRay(transform.position, rayDirection * forwardDetectionDistance, Color.blue);

            // If ground is detected in this direction
            if (forwardGroundInfo.collider != null)
            {
                
                GameObject groundObject = forwardGroundInfo.collider.gameObject;

                Transform groundTransform = groundObject.transform;

                string groundName = groundObject.name;
                //Debug.Log("Ground Object Name: " + groundName);

                float distance = Vector2.Distance(transform.position, groundTransform.position);

                Vector2 direction = (groundTransform.position - transform.position).normalized;

                float jumpAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                //Debug.Log("Ground detected in front at an angle of: " + jumpAngle + " degrees, distance: " + distance);

                float groundHeight = groundObject.GetComponent<Collider2D>().bounds.size.y;
                float groundWidth = groundObject.GetComponent<Collider2D>().bounds.size.x
                    ;
                // Determine the start position based on retreat direction
                Vector3 startOfGround = forwardDirection == Vector2.left
                    ? new Vector3(groundTransform.position.x + groundWidth / 2.2f, groundTransform.position.y, transform.position.z)
                    : new Vector3(groundTransform.position.x - groundWidth / 2.2f, groundTransform.position.y, transform.position.z);

                Vector3 targetPosition = new Vector3(startOfGround.x, startOfGround.y + groundHeight, transform.position.z);

                Vector2 jumpDirection = (targetPosition - transform.position).normalized;
                float jumpHeight = distance * jumpDistanceMultiplier;
                float gravityScale = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);

                AdjustJumpRates(distance, jumpAngle, out float horizontalDistanceRate, out float verticalDistanceRate);

                float timeToTarget = Mathf.Sqrt(2f * jumpHeight / gravityScale);
                float horizontalSpeed = (targetPosition.x - transform.position.x) / timeToTarget * horizontalDistanceRate;
                float verticalSpeed = Mathf.Sqrt(2f * gravityScale * jumpHeight) * verticalDistanceRate;

                animator.SetTrigger("Jump");


                yield return new WaitForSeconds(jumpAnimationDuration);

                rb.isKinematic = false;
                rb.freezeRotation = true;

                isGrounded = false;

                // Apply calculated velocity
                rb.linearVelocity = new Vector2(horizontalSpeed, verticalSpeed);

                yield return new WaitUntil(() => isGrounded);
                edgeOfGround = false;
                break;
            }
            scanAbove = !scanAbove;
        }
        yield return new WaitForSeconds(duration);

        flag = true;
    }

    void AdjustJumpRates(float distance, float jumpAngle, out float horizontalDistanceRate, out float verticalDistanceRate)
    {
        // Default rates
        horizontalDistanceRate = 1.0f;
        verticalDistanceRate = 0.5f;

        // Distance-Based Rate Adjustments
        if (distance <= 5f)
        {
            // Closest range, between 0 and ~5
            if (jumpAngle > 90f || jumpAngle < -90f)
            {
                // For backward or steep angles
                horizontalDistanceRate = 1.1f;
                verticalDistanceRate = 0.4f;
            }
            else
            {
                // For forward or less steep angles
                horizontalDistanceRate = 0.9f;
                verticalDistanceRate = 0.5f;
            }
        }
        else if (distance > 5f && distance <= 9.5f)
        {
            // Mid-range distances between ~5 and ~9.5
            if (jumpAngle > -45f && jumpAngle < 45f)
            {
                // Shallow angles
                horizontalDistanceRate = Mathf.Lerp(1.1f, 1.3f, (distance - 5f) / 4.5f); // 1.1 to 1.3
                verticalDistanceRate = Mathf.Lerp(0.4f, 0.2f, (distance - 5f) / 4.5f);   // 0.4 to 0.2
            }
            else if (jumpAngle > 135f || jumpAngle < -135f)
            {
                // Backward steep angles
                horizontalDistanceRate = Mathf.Lerp(1.1f, 0.9f, (distance - 5f) / 4.5f); // 1.1 to 0.9
                verticalDistanceRate = Mathf.Lerp(0.4f, 0.5f, (distance - 5f) / 4.5f);   // 0.4 to 0.5
            }
        }
        else
        {
            // Farther targets, distance over ~9.5
            horizontalDistanceRate = 0.95f;
            verticalDistanceRate = 0.5f;
        }

        // Angle-Based Refinements
        if (jumpAngle > 45f && jumpAngle < 135f)
        {
            // High upward angles
            verticalDistanceRate += 0.1f; // Add to vertical rate for higher arcs
        }
        else if (jumpAngle < -45f && jumpAngle > -135f)
        {
            // Steep downward angles
            horizontalDistanceRate += 0.05f; // Add to horizontal rate for flatter arcs
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            if (!isGrounded)
            {
                rb.linearVelocity = Vector2.zero;
                rb.isKinematic = true;
                isGrounded = true;
            }
        }
    }
}
