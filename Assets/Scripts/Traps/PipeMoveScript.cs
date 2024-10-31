using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMoveScript : MonoBehaviour
{
    public float moveSpeed;
    public float moveAmount;
    private float deadZone = -32;
    private Transform bottomPipeTransform;
    private bool openPipe;
    public LogicScript logic;

    private Vector3 bottomPipeOriginalPosition;
    private Vector3 bottomPipeTargetPosition;
    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        bottomPipeTransform = transform.Find("Bottom Pipe");

        if (bottomPipeTransform == null)
        {
            Debug.LogError("One or more child pipes are missing!");
        }

        bottomPipeOriginalPosition = bottomPipeTransform.position;

        bottomPipeTransform.position = bottomPipeOriginalPosition + Vector3.up * moveAmount;

        bottomPipeTargetPosition = bottomPipeOriginalPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
        if (openPipe) {
            if (bottomPipeTransform != null)
            {
                bottomPipeTransform.position = Vector3.MoveTowards(bottomPipeTransform.position, new Vector3(transform.position.x, bottomPipeTargetPosition.y, transform.position.z), moveAmount * Time.deltaTime);
            }
        }
    }

    public void OpenPipe()
    {
        openPipe = true;
    }
}
