using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPlatform : MonoBehaviour
{

    private bool isCollapsing;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 && !isCollapsing)
        {
            StartCoroutine(CollapsePlatform());
        }
    }

    private IEnumerator CollapsePlatform()
    {
        isCollapsing = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 0.6f);
        }

        yield return new WaitForSeconds(1f);

        Destroy(gameObject); // Destroy the platform
    }
}

