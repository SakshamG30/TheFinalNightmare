using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public LogicScript logic;
    public GameObject shooter;
    //public Enemy enemy;
    public TextMesh bulletText;
    public Rigidbody2D rb;
    public AstroShoot astro;
    public SpriteRenderer spriteRenderer;
    private bool deathAnimation;
    // Start is called before the first frame update
    void Start()
    {
        astro = GameObject.FindGameObjectWithTag("Player").GetComponent<AstroShoot>();
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        rb = GetComponent<Rigidbody2D>();
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
            OnBecameInvisible();
            logic.GameOver();
        }
        else
        {
            OnBecameInvisible();
        }
    }
    IEnumerator waitForAnimation(GameObject gameObject)
    {
        yield return new WaitForSeconds(3);
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
}
