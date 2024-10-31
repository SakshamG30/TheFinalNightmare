using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{

    public GameObject boltPrefab;
    public GameObject werewolfPrefab;
    public float boltSpeed;
    public float shootingInterval;
    public float animationInterval;
    public Transform player;
    private Transform shootingPoint;
    public float maxRange;
    public Animator animator;
    Animator boltAnimator;
    public BossMovement bossMove;
    AstroShoot astro;
    bool facingLeft;
    bool cutSceneEnabled;

    private float timer = 0f;
    private bool transformPossible = false;
    private float transformThreshold = 10f;


    public AudioClip fireSound;
    public AudioClip boltSound;
    public AudioClip transformSound;
    public float rangeMult;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        astro = player.GetComponent<AstroShoot>();
        bossMove = GetComponent<BossMovement>();
        if(maxRange == 0 )
            maxRange = bossMove.maxRange;
        animator = GetComponent<Animator>();
        shootingPoint = transform.Find("ShootingPoint");
        if (boltPrefab == null)
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
    private bool withinRange(float range)
    {
        return Vector2.Distance(transform.position, player.position) <= range;
    }
    // Update is called once per frame
    void Update()
    {
        bossMove = GetComponent<BossMovement>();
        cutSceneEnabled = bossMove.cutSceneEnabled;
        if (cutSceneEnabled)
        {
            return;
        }
        facingLeft = bossMove.facingLeft;
        shootingPoint = transform.Find("ShootingPoint");
        UpdateTransformTimer();
    }

    private IEnumerator ShootAtPlayer()
    {
        bossMove = GetComponent<BossMovement>();
        while (true)
        {
            yield return new WaitForSeconds(shootingInterval);
            if(withinRange(maxRange*rangeMult) && !cutSceneEnabled)
            {
                if (withinRange(maxRange))
                {
                    if (withinRange(maxRange / 2.5f) && transformPossible)
                    {
                        ResetTransform();
                    }
                    else
                    {
                        animator.SetTrigger("Attack1");
                        astro.isArrow = false;
                        yield return new WaitForSeconds(animationInterval);
                        
                        if (bossMove.textToShow != "")
                        {
                            StartCoroutine(bossMove.ShowTextForSecond(bossMove.textToShow));
                        }
                        Shoot();
                    }
                }
                else
                {
                    animator.SetTrigger("Attack2");
                    astro.isArrow = true;
                    yield return new WaitForSeconds(animationInterval);
                    
                    if (bossMove.textToShow != "")
                    {
                        StartCoroutine(bossMove.ShowTextForSecond(bossMove.textToShow));
                    }
                    Shoot();
                }
            }
            
        }
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
        GameObject bolt = Instantiate(boltPrefab, shootingPoint.position, Quaternion.identity);
        EnergyBolt energyScript = bolt.GetComponent<EnergyBolt>();
        if (!facingLeft)
        {
            SpriteRenderer spriteRenderer = bolt.GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = true;
        }
        energyScript.shooter = this.gameObject;
        float randomVerticalDistance = Random.Range(0f, 0.8f);
        Vector2 targetPosition = new Vector2(player.position.x, player.position.y + randomVerticalDistance);
        Vector2 direction = (targetPosition - (Vector2)shootingPoint.position).normalized;

        Rigidbody2D bulletRb = bolt.GetComponent<Rigidbody2D>();
        boltAnimator = bolt.GetComponent<Animator>();
        if (astro.isArrow)
        {
            EnemyAudioController.instance1.PlayFireSound(boltSound);
            StartCoroutine(ConvertToArrowAnimation(bulletRb, direction));
        }
        else
        {
            EnemyAudioController.instance1.PlayFireSound(fireSound);
            bulletRb.linearVelocity = direction * boltSpeed;
        }    
    }

    public IEnumerator ConvertToArrowAnimation(Rigidbody2D bulletRb, Vector2 direction)
    {
        boltAnimator.SetTrigger("Convert");
        yield return new WaitForSeconds(0.6f);
        
        bulletRb.linearVelocity = direction * boltSpeed * 2;
        yield return new WaitForSeconds(0.6f);
        
    }

    private void UpdateTransformTimer()
    {
        if (!transformPossible)
        {
            timer += Time.deltaTime;
            if (timer >= transformThreshold)
            {
                transformPossible = true;
                timer = transformThreshold; // Cap the timer at the threshold for clarity
                Debug.Log("Transformation is now possible!");
            }
        }
    }

    // Function to reset the timer and transformation possibility
    public void ResetTransform()
    {
        bossMove.cutSceneEnabled = true;
        timer = 0f;
        EnemyAudioController.instance1.PlayFireSound(transformSound);
        StartCoroutine(TransformToWerewolf());
    }

    private IEnumerator TransformToWerewolf()
    {
        
        // Instantiate the werewolf at the current position and rotation
        animator.SetTrigger("Transform");
        yield return new WaitForSeconds(0.5f);
        
        GameObject werewolfObject = Instantiate(werewolfPrefab, transform.position, transform.rotation);
        Werewolf werewolf = werewolfObject.GetComponent<Werewolf>();
        werewolf.player = player;
        if (werewolf != null)
        {
            Debug.Log("Inquisitor transforming into Werewolf");

            // Disable only the sprite renderer to hide the boss visually
            GetComponent<SpriteRenderer>().enabled = false;

            // Activate the werewolf behaviors
            werewolf.Activate();
        }
        else
        {
            Debug.LogError("Werewolf component not found on instantiated werewolf prefab!");
            yield break; // Exit if instantiation fails
        }
        Animator wereanim = werewolfObject.GetComponent<Animator>();
        // Wait for 4 seconds in werewolf form
        yield return new WaitForSeconds(1.8f);

        wereanim.SetTrigger("Transform");
        yield return new WaitForSeconds(0.5f);
        bossMove = GetComponent<BossMovement>();
        bossMove.cutSceneEnabled = false;
        // Destroy the werewolf object and revert to Inquisitor

        transform.position = werewolfObject.transform.position;

        Destroy(werewolfObject);
        Debug.Log("Werewolf destroyed, reverting to Inquisitor");

        EnemyAudioController.instance1.audioSource = GetComponent<AudioSource>();
        // Re-enable the sprite renderer to make the boss visible again


        Debug.Log("Inquisitor reactivated");
        EnemyAudioController.instance1.PlayFireSound(transformSound);
        GetComponent<SpriteRenderer>().enabled = true;
        
        yield return new WaitForSeconds(1f); // Add a delay for smooth reactivation, if needed
        transformPossible = false;

        


    }


}
