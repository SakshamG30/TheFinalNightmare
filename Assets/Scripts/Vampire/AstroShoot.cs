using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class AstroShoot : MonoBehaviour
{
    public GameObject fireballPrefab;
    public GameObject ultFireballPrefab;

    public float ultFireballSpeed = 5f; 
    public int ultFireballCount = 5; 
    public float ultFireballSpreadAngle = 45f;
    public float ultFireballSpawnHeight = 3f;


    private Transform gunPoint;
    public Transform arm;
    public float fireballSpeed;
    public LineRenderer trajectoryLine;
    public LayerMask collisionMask;
    public float maxRaycastDistance;
    private float originalArmAngle;
    private bool trajectoryVisible = false;
    private AstroScript astro;
    private bool facingRight, isGravityInverted;
    public int bulletCount;
    public TextMesh playerText;
    public Transform shield;
    public bool isShieldActive = false;
    int shieldHealth = 10;
    public Text shieldText;
    private bool isRecharging = false;
    public bool isArrow;
    public int bulletLayer = 8;
    public int swordLayer = 11;
    public BulletManager bulletManager;
    public Animator animator;
    public bool unlockedHellfire;

    
    public AudioClip attackSound;

    private List<GameObject> spawnedFireballs = new List<GameObject>(); // Track spawned fireballs
    private bool isFireballSpawned = false; // Track whether fireballs are spawned

    public bool cutSceneEnabled;

    // Start is called before the first frame update
    void Start()
    {
        arm = transform.Find("ArmPivot");
        gunPoint = arm.transform.Find("GunPoint");
        shield = transform.Find("Shield");
        GameObject shieldTextObj = GameObject.Find("ShieldText");
        if (shieldTextObj != null)
        {
            shieldText = shieldTextObj.GetComponent<Text>();
        }
        trajectoryLine.enabled = trajectoryVisible;
        originalArmAngle = arm.eulerAngles.z;
        astro = GetComponent<AstroScript>();
        facingRight = astro.facingRight;
        isGravityInverted = astro.isGravityInverted;
        shield.gameObject.SetActive(false);
        bulletManager = GameObject.Find("Bullets").GetComponent<BulletManager>();
        bulletManager.UpdateBulletCount(bulletCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (shieldHealth <= 0 && !isRecharging)
        {
            ActivateShield(false);
            StartCoroutine(ShowTextForSecond("Shield Broken!"));
            StartCoroutine(ShieldRecharge());
        }
        astro = GetComponent<AstroScript>();
        cutSceneEnabled = astro.cutSceneEnabled;
        if (cutSceneEnabled)
        {
            return;
        }
        facingRight = astro.facingRight;
        isGravityInverted = astro.isGravityInverted;
        arm = transform.Find("ArmPivot");
        gunPoint = arm.transform.Find("GunPoint");
        AimAtMouse();
        ShowTrajectory();

        if (Input.GetMouseButton(1))
        {
            if (shieldHealth <= 0)
            {
                StartCoroutine(ShowTextForSecond("Shield is broken! Wait for it to recharge!"));
            }
            else
               ActivateShield(true);
        }
        else
        {
            ActivateShield(false);
        }

        if (unlockedHellfire)
        {
            if (Input.GetKeyUp(KeyCode.Z))
            {
                if (!isFireballSpawned)
                {
                    if (astro.isGrounded)
                        StartCoroutine(SpawnUltFireballsAbove());
                    else
                        StartCoroutine(ShowTextForSecond("Can't Deploy Ultimate mid air"));
                }
                else
                {
                    StartCoroutine(LaunchUltFireballs());
                }
            }
        }
    }

    private IEnumerator SpawnUltFireballsAbove()
    {
        // Play the main character's summoning animation
        animator.Play("HellFireSpawn");

        // Set the spawn height based on the inversion state
        Vector3 spawnOffset = astro.isGravityInverted ? Vector3.down * ultFireballSpawnHeight : Vector3.up * ultFireballSpawnHeight;

        // Spawn fireballs above or below the player with a slight delay
        for (int i = 0; i < ultFireballCount; i++)
        {
            // Position each fireball slightly offset based on inversion
            Vector3 spawnPosition = transform.position + spawnOffset + Vector3.right * (i - ultFireballCount / 2);
            GameObject ultFireball = Instantiate(ultFireballPrefab, spawnPosition, Quaternion.identity);

            // Add to list and play individual spawn animation with a delay
            spawnedFireballs.Add(ultFireball);
            UltFireball script = ultFireball.GetComponent<UltFireball>();
            if(ultFireball != null)
                StartCoroutine(script.PlayAnimation());

            yield return new WaitForSeconds(0.1f); // Short delay to prevent animation conflicts
        }

        isFireballSpawned = true; // Indicate that fireballs are spawned
    }

    private IEnumerator LaunchUltFireballs()
    {
        animator.Play("HellFire");

        yield return new WaitForSeconds(0.45f); // Optional delay for animation

        if (spawnedFireballs.Count > 0)
        {
            // Set the base direction to aim upwards if inverted, otherwise aim downwards
            Vector2 baseDirection = astro.isGravityInverted ? Vector2.up : Vector2.down;

            // Calculate the angle increment for each fireball in the spread
            float angleStep = 20f;
            float startingAngle = -angleStep * (spawnedFireballs.Count - 1) / 2;

            for (int i = 0; i < spawnedFireballs.Count; i++)
            {
                GameObject ultFireball = spawnedFireballs[i];

                if(ultFireball != null)
                {
                    ultFireball.SetActive(true); // Show fireball

                    // Calculate the angle for this fireball
                    float currentAngle = startingAngle + (angleStep * i);

                    currentAngle = isGravityInverted ? -currentAngle : currentAngle;

                    // Calculate the final direction for each fireball based on baseDirection
                    Vector2 launchDirection = Quaternion.Euler(0, 0, currentAngle) * baseDirection;

                    // Apply velocity to fireball to create the bombardment effect
                    Rigidbody2D rb = ultFireball.GetComponent<Rigidbody2D>();
                    rb.linearVelocity = launchDirection * ultFireballSpeed;
                }
            }

            spawnedFireballs.Clear(); // Clear the list after launching
            isFireballSpawned = false; // Reset spawn status
        }
    }




    private void AimAtMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;

        //To get the angle of the player towards the mouse
        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + 90f;

        /*if (!facingRight)
        {
            angle += 180f; // Adjust the angle by 180 degrees when flipped
        }

        float clampedAngle = Mathf.Clamp(angle, originalArmAngle - 88f, originalArmAngle + 200f);*/

        gunPoint.rotation = Quaternion.Euler(new Vector3(0, 0 , angle));

        arm.rotation = Quaternion.Euler(new Vector3(0, 0 , angle));

        if (Input.GetKeyDown(KeyCode.V))
        {
            trajectoryVisible = !trajectoryVisible;
        }
        trajectoryLine.enabled = trajectoryVisible;

        if (Input.GetMouseButtonDown(0))
        {
            if (isShieldActive)
            {
                StartCoroutine(ShowTextForSecond("Can't Shoot while shield is active!"));
            }
            else {
                if (bulletCount > 0)
                {
                    RemoveBullet();
                    Shoot(direction);
                }
                else
                {
                    if (this.isActiveAndEnabled)
                    {
                        StartCoroutine(ShowTextForSecond("Bullets are Empty!"));
                    }
                }
            }
        }
        
        
    }

    private IEnumerator ShieldRecharge()
    {
        isRecharging = true;
        float rechargeTime = 10f; // Total recharge time in seconds

        for (float countdown = rechargeTime; countdown > 0; countdown -= 1f)
        {
            // Update shield text to show countdown
            shieldText.text = "Shield Fully Recharging in " + countdown + " seconds";
            yield return new WaitForSeconds(1f); // Wait for 1 second between updates
        }

        // Set shield health to max once recharge is complete
        shieldHealth = 10;
        shieldText.text = ""; // Update text after recharge

        isRecharging = false; // Reset recharge flag
    }

    private void ActivateShield(bool activate)
    {
        isShieldActive = activate;
        shield.gameObject.SetActive(activate);
    }

    private void ShowTrajectory()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - gunPoint.position).normalized;

        RaycastHit2D hitInfo = Physics2D.Raycast(gunPoint.position, direction, maxRaycastDistance, collisionMask);

        /*if (hitInfo.collider)
        {
            trajectoryLine.SetPosition(0, gunPoint.position);
            trajectoryLine.SetPosition(1, hitInfo.point);
        }
        else
        {
            trajectoryLine.SetPosition(0, gunPoint.position);
            trajectoryLine.SetPosition(1, gunPoint.position + (Vector3)direction * maxRaycastDistance);
        }*/
    }

    private void Shoot(Vector2 direction)
    {
        PlayerAudioController.instance.PlayAttackSound(attackSound);
        GameObject fireball = Instantiate(fireballPrefab, gunPoint.position, gunPoint.rotation);
        FireBall fireballScript = fireball.GetComponent<FireBall>();
        fireballScript.shooter = this.gameObject;
        Rigidbody2D bulletRb = fireball.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = direction * fireballSpeed;
        animator.SetTrigger("Magic");
        StartCoroutine(ResetMagicTrigger());
    }

    private IEnumerator ResetMagicTrigger()
    {
        yield return new WaitForSeconds(0.2f);
        animator.ResetTrigger("Magic");
    }
    public IEnumerator ShowTextForSecond(string text, int period = 1)
    {
        playerText.text = text;
        yield return new WaitForSeconds(period);
        playerText.text = "";
    }

    public void AddBullet()
    {
        bulletCount++;
        bulletManager.UpdateBulletCount(bulletCount);
    }

    public void RemoveBullet()
    {
        bulletCount--;
        bulletManager.UpdateBulletCount(bulletCount);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isShieldActive)
        {
            int layer = other.gameObject.layer;

            // Check if the collider's layer is the bullet layer
            if (layer == bulletLayer)
            {
                if (this.isActiveAndEnabled)
                {
                    if(other.gameObject.tag != "Arrow")
                    {
                        if(other.gameObject.tag == "EnergyBolt")
                        {
                            if (isArrow)
                            {
                                shieldHealth -= 4;
                                StartCoroutine(ShowTextForSecond("Shield took damage!"));
                            }
                            else
                            {
                                AddBullet();
                                shieldHealth -= 2;
                                StartCoroutine(ShowTextForSecond("Absorbed but shield took some damage!"));
                            }
                        }
                        else
                        {
                            AddBullet();
                            StartCoroutine(ShowTextForSecond("Magic Absorbed!"));
                        }
                    }  
                    else
                        StartCoroutine(ShowTextForSecond("Arrow Blocked!"));
                }
            }
            if (layer == swordLayer)
            {
                StartCoroutine(ShowTextForSecond("Nice Block!"));
                AddBullet();
            }
            if (layer == 15)
            {
                StartCoroutine(ShowTextForSecond("Nice Block!"));
                AddBullet();
            }
        }
    }
}
