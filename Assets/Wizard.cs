using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour
{

    public GameObject fireballPrefab;
    public float fireballSpeed;
    public float shootingInterval;
    public float animationInterval;
    public Transform player;
    public Transform arm;
    private Transform gunPoint;
    public float maxRange;
    public TextMesh enemyText;
    public string textToShow;
    public bool isInverted = false;
    public Animator animator;
    public bool facingLeft = true; // To track the current facing direction of the enemy
    public float deathAnimationDuration;
    // Start is called before the first frame update

    public AudioClip fireSound;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        arm = transform.Find("ArmPivot");
        gunPoint = arm.transform.Find("GunPoint");
        if (fireballPrefab == null)
        {
            Debug.LogError("fireballPrefab is not assigned!");
        }
        if (gunPoint == null)
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
    private bool withinRange()
    {
        return Vector2.Distance(transform.position, player.position) <= maxRange;
    }
    // Update is called once per frame
    void Update()
    {
        arm = transform.Find("ArmPivot");
        gunPoint = arm.transform.Find("GunPoint");

        CheckDirection();
    }

    private IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootingInterval);
            if (withinRange())
            {
                animator.SetTrigger("Fire");
                yield return new WaitForSeconds(animationInterval);
                if (textToShow != "")
                {
                    StartCoroutine(ShowTextForSecond(textToShow));
                }
                EnemyAudioController.instance1.PlayFireSound(fireSound);
                Shoot();
            }
        }
    }
    private void AimAtPlayer()
    {
        if (player != null && gunPoint != null)
        {
            Vector2 direction = (player.position - gunPoint.position).normalized;
            bool stance = false;
            if (transform.localScale.y < 0f)
            {
                stance = true;
            }

            float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            angle += stance ? 270f : 90f;

            gunPoint.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            arm.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
    private void Shoot()
    {
        GameObject fireball = Instantiate(fireballPrefab, gunPoint.position, Quaternion.identity);
        FireBall fireballScript = fireball.GetComponent<FireBall>();
        fireballScript.shooter = this.gameObject;
        float randomVerticalDistance = Random.Range(0f, 0.8f);
        randomVerticalDistance = isInverted ? -randomVerticalDistance : randomVerticalDistance;
        Vector2 targetPosition = new Vector2(player.position.x, player.position.y + randomVerticalDistance);
        Vector2 direction = (targetPosition - (Vector2)gunPoint.position).normalized;

        Rigidbody2D bulletRb = fireball.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = direction * fireballSpeed;
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
    }

    public void playDeathAnimation()
    {
        animator.Play("Wizard Death");
        StartCoroutine(waitForAnimation());
    }

    IEnumerator waitForAnimation()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(this.gameObject);
    }
}
