using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletManager : MonoBehaviour
{
    private GameObject[] bulletIcons;
    public Text bulletCountText;

    private int bulletCount = 10;

    void Start()
    {
        // Initialize the bullet icons by fetching all child GameObjects
        bulletIcons = new GameObject[transform.childCount];
        bulletCountText = GameObject.Find("BulletCountText").GetComponent<Text>();

        for (int i = 0; i < transform.childCount; i++)
        {
            bulletIcons[i] = transform.GetChild(i).gameObject;
        }

        // Ensure all bullet icons are enabled at the start
        UpdateBulletIcons();
    }

    // Call this method whenever a bullet is fired or reloaded
    public void UpdateBulletCount(int newBulletCount)
    {
        bulletCount = newBulletCount;
        UpdateBulletIcons();
    }

    private void UpdateBulletIcons()
    {
        // Loop through all bullet icons and disable the ones that are above the current bullet count
        bulletCountText.text = $"Fireball Count: {bulletCount}";
        if(bulletCount <= 0)
        {
            bulletCountText.text = "No Magic Left";
        }
        for (int i = 0; i < bulletIcons.Length; i++)
        {
            if (i < bulletCount)
            {
                bulletIcons[i].SetActive(true); // Enable icons for available bullets
            }
            else
            {
                bulletIcons[i].SetActive(false); // Disable icons for bullets that have been used
            }
        }
    }
}
