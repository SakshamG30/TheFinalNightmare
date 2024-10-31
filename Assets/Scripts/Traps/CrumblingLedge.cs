using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingLedge : MonoBehaviour
{
    public float crumbleDelay = 2.0f; // Delay before the platform crumbles
    public float respawnTime = 5.0f; // Time before the platform reappears

    private bool isCrumbling = false;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCrumbling)
        {
            StartCoroutine(CrumblePlatform());
        }
    }

    private IEnumerator CrumblePlatform()
    {
        isCrumbling = true;
        yield return new WaitForSeconds(crumbleDelay);

        // Hide or disable the platform
        gameObject.SetActive(false);

        // Respawn the platform after a delay
        yield return new WaitForSeconds(respawnTime);
        gameObject.SetActive(true);
        transform.position = initialPosition;

        isCrumbling = false;
    }
}
