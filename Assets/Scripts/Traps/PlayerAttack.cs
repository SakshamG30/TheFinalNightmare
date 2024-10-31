using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 1;
    public float attackRange = 1f;
    public LayerMask enemyLayer; // Layer for enemies like bats

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("MouseButton1"))
        {
            Attack();
        }
    }

    void Attack()
    {
        // Detect enemies in range of attack
        RaycastHit2D[] hitEnemies = Physics2D.CircleCastAll(transform.position, attackRange, Vector2.zero, 0f, enemyLayer);

        // Damage each enemy hit
        foreach (RaycastHit2D enemy in hitEnemies)
        {
            EnemyBat bat = enemy.transform.GetComponent<EnemyBat>();
            if (bat != null)
            {
                bat.TakeDamage(attackDamage); // Deal damage to the bat
            }
        }
    }

    // Optional: Visualize the attack range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
