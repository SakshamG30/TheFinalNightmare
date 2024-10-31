using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungingEnemy : MonoBehaviour
{
    public float slashInterval; // Interval before the enemy lunges
    public Transform player;
    public Transform arm;
    public float maxRange;
    public TextMesh enemyText;
    public float lungeSpeed;
    public float lungeDuration;
    private bool isLunging = false;
    public float swordRotationAngle = 170f; // The sword's rotation angle
    public float restDuration = 10f; // Rest duration after each lunge
    private bool facingLeft = true; // To track the current facing direction of the enemy
    public bool isInverted = false;
    public string textToShow;
    public TutorialManager tutorialManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        tutorialManager = FindAnyObjectByType<TutorialManager>();
        arm = transform.Find("ArmPivot");

        if (player == null)
        {
            Debug.LogError("Player not found!");
        }

        if (arm == null)
        {
            Debug.LogError("ArmPivot not found!");
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

    private IEnumerator LungeAtPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(slashInterval);
            if (withinRange() && !isLunging)
            {
                if (textToShow!="")
                {
                    StartCoroutine(ShowTextForSecond(textToShow));
                    yield return new WaitForSeconds(2f);
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
        StartCoroutine(RotateSword());

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

    private IEnumerator RotateSword()
    {
        float initialAngle = arm.eulerAngles.z;
        float targetAngle;

        // If the enemy is facing left, rotate the sword normally.
        // If the enemy is not facing left, reverse the sword rotation direction.
        if (!isInverted) {
            targetAngle = facingLeft ? initialAngle + swordRotationAngle : initialAngle - swordRotationAngle;
        }
        else
        {
            targetAngle = facingLeft ? initialAngle - swordRotationAngle : initialAngle + swordRotationAngle;
        }

        float rotationSpeed = swordRotationAngle / lungeDuration;

        float elapsed = 0f;
        while (elapsed < lungeDuration)
        {
            float angle = Mathf.Lerp(initialAngle, targetAngle, elapsed / lungeDuration);
            arm.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset the sword rotation after the lunge
        arm.rotation = Quaternion.Euler(new Vector3(0, 0, initialAngle));
    }

    public IEnumerator ShowTextForSecond(string text)
    {
        enemyText.text = text;
        yield return new WaitForSeconds(1);
        enemyText.text = "";
    }
}
