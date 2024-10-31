using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public LogicScript logic;
    public GameObject shooter;
    //public Enemy enemy;
    public TextMesh bulletText;
    public Rigidbody2D rb;
    public AstroShoot astro;
    public SpriteRenderer spriteRenderer;
    private bool deathAnimation;
    BossMovement boss;
    Werewolf werewolf;
    public int damageToBoss = 10;
    GameObject checkpointObj;
    public CheckpointManager checkpointManager;
    // Start is called before the first frame update
    void Start()
    {
        astro = GameObject.FindGameObjectWithTag("Player").GetComponent<AstroShoot>();
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();

        GameObject bossObject = GameObject.FindGameObjectWithTag("Boss");
        GameObject werewolfObj = GameObject.FindGameObjectWithTag("Werewolf");
        if (bossObject != null)
        {
            boss = bossObject.GetComponent<BossMovement>();
        }
        if (werewolfObj != null)
        {
            werewolf = werewolfObj.GetComponent<Werewolf>();
        }



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
            checkpointObj = GameObject.Find("CheckPoint");
            if (checkpointObj != null)
            {
                checkpointManager = checkpointObj.GetComponent<CheckpointManager>();
                checkpointManager.OnEnemyKilled();
            }
            GameObject parentObject = collision.gameObject.transform.root.gameObject;
            if (parentObject.tag == "Wizard")
            {
                Debug.Log("Wizard hit!");
                Wizard wizard = parentObject.GetComponent<Wizard>();
                deathAnimation = true;
                wizard.playDeathAnimation();
            }
            if (parentObject.tag == "Knight")
            {
                Debug.Log("Knight hit!");
                ZombieKnight knight = parentObject.GetComponent<ZombieKnight>();
                deathAnimation = true;
                knight.playDeathAnimation();
            }
            if (parentObject.tag == "Archer")
            {
                Debug.Log("Archer hit!");
                Archer archer = parentObject.GetComponent<Archer>();
                deathAnimation = true;
                archer.playDeathAnimation();
            }
            if (parentObject.tag == "Werewolf")
            {
                Debug.Log("Werewolf hit!");
                Werewolf werewolf = parentObject.GetComponent<Werewolf>();
                deathAnimation = true;
                werewolf.playDeathAnimation();
            }
            if (parentObject.tag == "Boss")
            {
                Debug.Log("boss hit!");
                BossMovement boss = parentObject.GetComponent<BossMovement>();
                deathAnimation = true;
                boss.playDeathAnimation();
            }
            string gameObjectName = collision.gameObject.name.ToLower(); 
            if (gameObjectName.Contains("head"))
            {
                Debug.Log("Head was hit!");
                
                astro.AddBullet();
                StartCoroutine(ShowTextForSecond("Headshot!"));
            }
            else if (gameObjectName.Contains("body") || (gameObjectName.Contains("leg")))
            {
                Debug.Log("Body was hit!");
             
                StartCoroutine(ShowTextForSecond("Targed Eradicated!"));
            }
            else
            {
                Debug.Log("Enemy was hit!");
                
                StartCoroutine(ShowTextForSecond("Targed Eradicated!"));
            }

            // Destroy the entire parent object
            if (!deathAnimation)
            {
                Destroy(parentObject);
            }
        }
        else if (collision.gameObject.layer == 15)
        {
            StartCoroutine(ShowTextForSecond("Targed Burned!"));
            if (collision.gameObject.tag == "Boss")
                boss.isHurt(damageToBoss);
            else
                boss.isHurt(damageToBoss / 3);
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
    IEnumerator waitForAnimation(GameObject gameObject)
    {
        yield return new WaitForSeconds(3);
        Debug.Log("Hi there!");
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
        Renderer textRenderer = textMesh.GetComponent<Renderer>();
        textRenderer.sortingLayerName = "UI"; // or whichever layer should be on top
        textRenderer.sortingOrder = 10;
    }
}
