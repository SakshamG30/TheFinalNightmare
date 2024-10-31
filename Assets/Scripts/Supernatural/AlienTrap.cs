using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienTrap : MonoBehaviour
{
    public float distance;
    public float speed;
    public bool goingDown = true;
    private float originalYPos;
    public LogicScript logic;
    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        originalYPos = 10.48003f;
    }

    // Update is called once per frame
    void Update()
    {
        MoveAlien();
    }

    void MoveAlien()
    {
        if (goingDown)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
            if (transform.position.y <= originalYPos - distance)
            {
                goingDown = false;
            }
        }
        else
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
            if (transform.position.y >= originalYPos)
            {
                goingDown = true;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            logic.GameOver();
        }
    }
}
