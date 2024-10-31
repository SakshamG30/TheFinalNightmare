using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienScript : MonoBehaviour
{
    public float moveSpeed;
    public float addSpeed;
    public LogicScript logic;
    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();

        StartCoroutine(IncreaseSpeedOverTime());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3.right * moveSpeed) * Time.deltaTime;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            logic.GameOver();
        }
    }

    private IEnumerator IncreaseSpeedOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            moveSpeed += addSpeed;
        }
    }

    public void StopHorde()
    {
        moveSpeed = 0f;
        StopAllCoroutines();
    }

    public void StartHorde()
    {
        moveSpeed = 1f;
        StartCoroutine(IncreaseSpeedOverTime());
    }
}
