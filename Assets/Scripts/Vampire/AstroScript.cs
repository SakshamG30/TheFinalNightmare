using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AstroScript : MonoBehaviour
{
    public Rigidbody2D astroPhysics;
    public float movementSpeed;
    public float verticalBoostStrength;
    public float horizontalBoostStrength;
    public float rotationSpeed;
    //public LogicScript logic;
    public GameObject logicObject;
    private bool IsAlive = true;
    public bool isGravityInverted = false;
    public bool isGrounded = false;
    bool isJumping = false;
    public float gravity = 0.4f;
    public float balanceSpeed;
    private Quaternion originalRotation, invertedRotation;
    public bool facingRight = true;
    public LogicScript logic;
    public JetpackGuage gauge;
    float gaugeValue = 10f;
    public float currentCauge;
    public float consumeGauge;
    public TutorialManager tutorialManager;
    public Animator animator;
    public GameObject sword;
    public TextMesh playerText;
    public GameObject heroBody, bats;
    public SpriteRenderer heroRenderer;
    private bool isTransformed;
    public bool cutSceneEnabled;
    public AstroShoot astroShoot;
    public AudioClip jumpSound;
    public AudioClip jumplanding;
    public AudioClip gravitySound;
    public float boundUpY = 25;
    public float boundDownY = -15;

    // Start is called before the first frame update
    void Start()
    {
        heroBody = transform.Find("HeroBody").gameObject;
        heroRenderer = heroBody.GetComponent<SpriteRenderer>();
        bats = transform.Find("Bats").gameObject;
        bats.SetActive(false);
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        sword = transform.Find("Sword").gameObject;
        sword.SetActive(false);
        gauge = GameObject.Find("JetPackGuage").GetComponent<JetpackGuage>();
        originalRotation = transform.rotation;
        invertedRotation = Quaternion.Euler(0, 0, 180);
        gauge.fillGauge(gaugeValue);
        tutorialManager = FindAnyObjectByType<TutorialManager>();
        if (tutorialManager != null)
        {
            if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0) // Check if the tutorial is not complete
            {
                StartCoroutine(tutorialManager.RunTutorials());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cutSceneEnabled)
        {
            return;
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector2 direction = new Vector2(horizontal, 0).normalized;
        if (isGrounded)
        {
            animator.SetFloat("Velocity", direction.magnitude);
        }
        else
        {
            animator.SetFloat("Velocity", 0);
        }
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsTransformed", isTransformed);

        if (horizontal != 0)
        {
            MoveAndFlipPlayer(horizontal);
        }
        if (isGrounded)
        {
            gauge.fillGauge(Time.deltaTime);
            if (Input.GetButtonDown("Jump"))
            {
                //animator.Play("VampireJump");
                if (!isGravityInverted)
                {
                    astroPhysics.linearVelocity = new Vector2(astroPhysics.linearVelocity.x, movementSpeed);
                }
                else
                {
                    astroPhysics.linearVelocity = new Vector2(astroPhysics.linearVelocity.x, -movementSpeed);
                }
                isGrounded = false;
                isJumping = true;
                PlayerAudioController.instance.PlayJumpSound(jumpSound);
            }
            if (direction.magnitude >= 0.1f)
            {
                transform.position += new Vector3(direction.x * movementSpeed * Time.deltaTime, 0, 0);
            }
        }
        else
        {
            if (direction.magnitude >= 0.1f)
            {
                transform.position += new Vector3(direction.x * movementSpeed * Time.deltaTime, 0, 0);
            }
            if (Input.GetButtonDown("Jump") && !isGrounded)
            {
                if (gauge.slider.value >= consumeGauge)
                {
                    PlayerAudioController.instance.PlayJumpSound(jumpSound);
                    isJumping = false;
                    animator.SetTrigger("VBoost");
                    gauge.DepleteGauge(consumeGauge);
                    if (isGravityInverted)
                    {
                        astroPhysics.linearVelocity = Vector2.down * verticalBoostStrength;
                        StartCoroutine(VBoost());
                    }
                    else
                    {
                        astroPhysics.linearVelocity = Vector2.up * verticalBoostStrength;
                        StartCoroutine(VBoost());
                    }
                }
            }
           
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && IsAlive)
        {
            if (gauge.slider.value > 0f)
            {
                astroShoot = GetComponent<AstroShoot>();
                if (astroShoot.isShieldActive)
                {
                    StartCoroutine(astroShoot.ShowTextForSecond("Cannot Lunge while shield is activated!"));
                }
                else
                {
                    sword.SetActive(true);
                    isJumping = false;
                    animator.SetTrigger("HBoost");
                    gauge.DepleteGauge(consumeGauge);

                    StartCoroutine(Boost());
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X) && !SceneManager.GetActiveScene().name.Equals("Intro_scene"))
        {
            InvertGravity();
            if (isGravityInverted)
            {
                isJumping = false;
                animator.Play("Transform");
                StartCoroutine(TransformIntoSmoke("WHOOOSH", 1));
            }
            else
            {
                bats.SetActive(false);
                heroRenderer.enabled = true;
            }
            isGrounded = false;
        }
        
        if (transform.position.y > boundUpY ||  transform.position.y < boundDownY)
        {
            logic.GameOver();
        }
    }

    private IEnumerator VBoost()
    {  
        yield return new WaitForSeconds(0.6f);
        isJumping = true;
        animator.ResetTrigger("VBoost");
    }

    private void FixedUpdate()
    {
        RotateAstro();
        if(isGrounded)
        {
            astroPhysics.freezeRotation = true;
        }
        else
        {
            astroPhysics.freezeRotation = false;
        }
    }

    public void InvertGravity()
    {
        isGravityInverted = !isGravityInverted;
        PlayerAudioController.instance.PlayGravityInvertSound(gravitySound);
        astroPhysics.gravityScale = isGravityInverted ? -gravity : gravity;
        
    }

    public IEnumerator TransformIntoSmoke(string text, int seconds = 1)
    {
        isTransformed = true;
        playerText.text = text;
        yield return new WaitForSeconds(0.3f);
        bats.SetActive(true);
        if (isGravityInverted)
        {
            heroRenderer.enabled = false;
        }
        yield return new WaitForSeconds(seconds - 0.3f);
        
        isTransformed = false;

        heroBody.layer = LayerMask.NameToLayer("Default");
        playerText.text = "";
    }
    private IEnumerator Boost()
    {
        // Temporarily disable gravity
        float temp = astroPhysics.gravityScale;
        astroPhysics.gravityScale = 0;

        // Apply boost based on gravity direction
        if (isGravityInverted)
        {
            if (!facingRight) {
                astroPhysics.linearVelocity = Vector2.right * horizontalBoostStrength;
            }
            else
            {
                astroPhysics.linearVelocity = Vector2.left * horizontalBoostStrength;
            }
        }
        else
        {
            if (!facingRight)
            {
                astroPhysics.linearVelocity = Vector2.left * horizontalBoostStrength;
            }
            else
            {
                astroPhysics.linearVelocity = Vector2.right * horizontalBoostStrength;
            }
        }

        // Wait for a short duration while the boost is applied
        yield return new WaitForSeconds(0.8f);
        if (isGravityInverted)
        {
            temp = -1 * Mathf.Abs(temp);
        }
        else
        {
            temp = Mathf.Abs(temp);
        }
        isJumping = true;
        animator.ResetTrigger("HBoost");
        // Re-enable gravity after the boost
        astroPhysics.gravityScale = temp;
        sword.SetActive(false);
        playerText.text = "";
    }
    void RotateAstro()
    {
        float targetRotationZ = isGravityInverted ? 180f : 0f;

        // Get the current rotation angle
        Vector3 currentRotation = transform.rotation.eulerAngles;
        float currentRotationZ = currentRotation.z;

        float newRotationZ = Mathf.LerpAngle(currentRotationZ, targetRotationZ, rotationSpeed * Time.deltaTime);

        // Apply the new rotation
        transform.rotation = Quaternion.Euler(0, 0, newRotationZ);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            isGrounded = true;
            bats.SetActive(false);
            heroRenderer.enabled = true;

            heroBody.layer = LayerMask.NameToLayer("Player");

            if (isGravityInverted)
            {
                animator.Play("Inverted");
            }
            //birdIsAlive = false;
            //logic.gameOver();
            if (this.isActiveAndEnabled)
            {
                //StartCoroutine(BalanceToOriginalPosition());
            }
            PlayerAudioController.instance.PlayJumpSound(jumplanding);
        }
    }
    private void MoveAndFlipPlayer(float horizontal)
    {
        if (!isGravityInverted)
        {
            if (horizontal > 0 && !facingRight)
            {
                Flip();
            }
            else if (horizontal < 0 && facingRight)
            {
                Flip();
            }
        }
        else
        {
            if (horizontal > 0 && facingRight)
            {
                Flip();
            }
            else if (horizontal < 0 && !facingRight)
            {
                Flip();
            }
        }
        AdjustText();
    }

    private void AdjustText()
    {
        Vector3 textScale = playerText.transform.localScale;

        if (!facingRight)
        {
            textScale.x = -Mathf.Abs(textScale.x);
        }
        else
        {
            textScale.x = Mathf.Abs(textScale.x);
        }

        // Flip the Y scale if gravity is inverted
        if (isGravityInverted)
        {
            textScale.y = -Mathf.Abs(textScale.y);
            if (!facingRight)
            {
                textScale.x = Mathf.Abs(textScale.x);
            }
            else
            {
                textScale.x = -Mathf.Abs(textScale.x);
            }
        }
        else
        {
            textScale.y = Mathf.Abs(textScale.y);
        }

        // Apply the adjusted scale to the text
        playerText.transform.localScale = textScale;
    }

    public void Flip()
    {
        facingRight = !facingRight;

        // Flip the player's scale
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    private IEnumerator BalanceToOriginalPosition()
    {
        Quaternion targetRotation = isGravityInverted ? invertedRotation : originalRotation;

        while (Mathf.Abs(Quaternion.Angle(transform.rotation, targetRotation)) > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, balanceSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = targetRotation;
    }
}
