using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public GameObject bulletPrefab;
    public float bulletSpeed;
    public float shootingInterval;
    public Transform player;
    public Transform arm;
    private Transform gunPoint;
    public float maxRange;
    public TextMesh enemyText;
    public string textToShow;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        arm = transform.Find("ArmPivot");
        gunPoint = arm.transform.Find("GunPoint");
        if (bulletPrefab == null)
        {
            Debug.LogError("bulletPrefab is not assigned!");
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
    }

    private IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootingInterval);
            if (withinRange()) {
                enemyText.text = textToShow;
                Shoot();
            }
        }
    }
    private void AimAtPlayer()
    {
        if (player != null && gunPoint != null)
        {
            Vector2 direction = (player.position - gunPoint.position).normalized;
            bool stance = false ;
            if(transform.localScale.y < 0f)
            {
                stance = true ;
            }
            
            float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            angle += stance ? 270f : 90f;

            gunPoint.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            arm.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.shooter = this.gameObject;
        Vector2 direction = (player.position - gunPoint.position).normalized;

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = direction * bulletSpeed;
    }

    public IEnumerator ShowTextForSecond(string text)
    {
        enemyText.text = text;
        yield return new WaitForSeconds(1);
        enemyText.text = "";
    }
}
