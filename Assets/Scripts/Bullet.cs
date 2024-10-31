using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LogicScript logic;
    public GameObject shooter;
    //public Enemy enemy;
    public TextMesh bulletText;
    public Rigidbody2D rb;
    public AstroShoot astro;
    public SpriteRenderer spriteRenderer;
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
        if (collision.gameObject.layer == 7)
        {
            GameObject parentObject = collision.gameObject.transform.root.gameObject;
            if (collision.gameObject.name == "Head")
            {
                Debug.Log("Head was hit!");
                if (spriteRenderer != null)
                {
                    Material material = spriteRenderer.material;

                    material.shader = Shader.Find("Sprites/Default");

                    Color color = material.color;
                    color.a = 0; 
                    material.color = color;
                }
                rb.linearVelocity = Vector3.zero;
                rb.isKinematic = true;
                astro.AddBullet();
                StartCoroutine(ShowTextForSecond("Headshot!"));
            }
            else if(collision.gameObject.name == "Body")
            {
                Debug.Log("Body was hit!");
                if (spriteRenderer != null)
                {
                    Material material = spriteRenderer.material;

                    material.shader = Shader.Find("Sprites/Default");

                    Color color = material.color;
                    color.a = 0;
                    material.color = color;
                }
                rb.linearVelocity = Vector3.zero;
                rb.isKinematic = true;
                StartCoroutine(ShowTextForSecond("Kill Confirmed!"));
            }

            // Destroy the entire parent object
            
            Destroy(parentObject);
        }
        else if (collision.gameObject.layer == 3)
        {
            //GameObject parentObject = collision.gameObject.transform.root.gameObject;
            logic.GameOver();
        }
        else
        {
            OnBecameInvisible();
        }
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
