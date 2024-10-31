using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroScript1 : MonoBehaviour
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
    private bool isGrounded = false;
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

    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        sword = transform.Find("Sword").gameObject;
        //gauge = transform.Find("JetPackGuage").GetComponent<JetpackGuage>();
        originalRotation = transform.rotation;
        invertedRotation = Quaternion.Euler(0, 0, 180);
        gauge.fillGauge(gaugeValue);
        tutorialManager = FindAnyObjectByType<TutorialManager>();
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0) // Check if the tutorial is not complete
        {
            StartCoroutine(tutorialManager.RunTutorials());
        }
    }

    // Update is called once per frame
    void Update()
    {
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
                sword.SetActive(true);
                isJumping = false;
                animator.SetTrigger("HBoost");
                gauge.DepleteGauge(consumeGauge);
                
                StartCoroutine(Boost());
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            InvertGravity();
            isGrounded = false;
        }
        
        if (transform.position.y > 11 ||  transform.position.y < -8.6)
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

    private void InvertGravity()
    {
        isGravityInverted = !isGravityInverted;
        
        astroPhysics.gravityScale = isGravityInverted ? -gravity : gravity;
        
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

            //birdIsAlive = false;
            //logic.gameOver();
            if (this.isActiveAndEnabled)
            {
                //StartCoroutine(BalanceToOriginalPosition());
            }
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
    }

    private void Flip()
    {
        facingRight = !facingRight; 
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
