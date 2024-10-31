using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeOpenScript : MonoBehaviour
{
    public GameObject OpenPipe;
    public GameObject ClosedPipe;
    private GameObject currentPipe;
    public float moveSpeed = 1.2f;
    private float deadZone = -32;
    // Start is called before the first frame update
    void Start()
    {
        ClosedPipe.SetActive(true);
        OpenPipe.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3.left * moveSpeed) * Time.deltaTime;
        if (transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
    }
    public void PipeOpen()
    {
        ClosedPipe.SetActive(false);
        OpenPipe.transform.position = ClosedPipe.transform.position;
        OpenPipe.SetActive(true);
    }
}
