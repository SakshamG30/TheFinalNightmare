using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Ghoul : MonoBehaviour
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
    public Animator animator;
    public float deathAnimationDuration;
    public bool isInverted = false;
    public bool facingLeft = true;
    private bool detected;
    public float distPerFixedAmt;

    public AudioClip fireSound;
    // Start is called before the first frame update
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

    private void FixedUpdate()
    {
        animator.SetBool("WithinRange", detected);
        if (detected)
        {
            Advance(distPerFixedAmt);
        }
    }
    private bool WithinRange()
    {
        return Vector2.Distance(transform.position, player.position) <= maxRange;
    }
    // Update is called once per frame
    void Update()
    {
        arm = transform.Find("ArmPivot");
        gunPoint = arm.transform.Find("GunPoint");
    }

    private IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootingInterval);
            if (WithinRange())
            {
                detected = true;
                //animator.SetTrigger("Fire");
                yield return new WaitForSeconds(animationInterval);
                if (textToShow != "")
                {
                    StartCoroutine(ShowTextForSecond(textToShow));
                }
                Shoot();
            }
            else
            {
                detected = false;
            }
        }
    }

    public void Advance(float dist)
    {
        if (facingLeft)
        {
            Vector2 pos = transform.position;
            pos.x -= dist;
            transform.position = pos;
        }
        else
        {
            Vector2 pos = transform.position;
            pos.x += dist;
            transform.position = pos;
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
        EnemyAudioController.instance1.PlayFireSound(fireSound);
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
    public IEnumerator ShowTextForSecond(string text)
    {
        enemyText.text = text;
        yield return new WaitForSeconds(1);
        enemyText.text = "";
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
