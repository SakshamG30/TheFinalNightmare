using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    public GameObject pipe;
    public PipeMoveScript pipeMoveScript;
    // Start is called before the first frame update
    void Start()
    {
        pipeMoveScript = pipe.GetComponent<PipeMoveScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            pipeMoveScript.OpenPipe();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            pipeMoveScript.OpenPipe();
        }
    }
}
