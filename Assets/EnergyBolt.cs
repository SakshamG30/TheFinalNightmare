using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBolt : MonoBehaviour
{
    public LogicScript logic;
    public GameObject shooter;
    public TextMesh bulletText;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;   
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
        if (shooter != null)
        {
            Collider2D bulletCollider = GetComponent<Collider2D>();

            // Get all colliders attached to the shooter and its children
            Collider2D[] shooterColliders = shooter.GetComponentsInChildren<Collider2D>();

            foreach (Collider2D shooterCollider in shooterColliders)
            {
                if (shooterCollider != null)
                {
                    Physics2D.IgnoreCollision(bulletCollider, shooterCollider);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer == 3)
        {
            //GameObject parentObject = collision.gameObject.transform.root.gameObject;
            rb.linearVelocity = Vector3.zero;

            animator.SetTrigger("Impact");
            StartCoroutine(WaitForAnimation(0.5f));
            logic.GameOver();
        }
        else if (collision.gameObject.layer == 6)
        {
            rb.linearVelocity = Vector3.zero;

            animator.SetTrigger("Impact");
            StartCoroutine(WaitForAnimation(0.5f));
        }
        else
        {
            OnBecameInvisible();
        }
    }
    IEnumerator WaitForAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        Destroy(gameObject);
    }

    public IEnumerator ShowTextForSecond(string text)
    {
        AlignTextToCamera(bulletText);
        bulletText.text = text;
        yield return new WaitForSeconds(1);
        OnBecameInvisible();
        bulletText.text = "";
    }

    private void AlignTextToCamera(TextMesh textMesh)
    {
        textMesh.transform.LookAt(Camera.main.transform);

        textMesh.transform.Rotate(0, 180, 0);
    }

    public IEnumerator ConvertToArrowAnimation()
    {
        animator.SetTrigger("Convert");
        yield return new WaitForSeconds(0.1f);
        animator.SetTrigger("Arrow");
    }
}
