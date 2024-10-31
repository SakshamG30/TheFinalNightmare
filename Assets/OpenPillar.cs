using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenPillar : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject healthBar;
    public AstroShoot astro;
    AudioSource sound;
    public Text shieldText;

    private void Start()
    {
        astro = GameObject.FindGameObjectWithTag("Player").GetComponent<AstroShoot>();
        sound  = GetComponent<AudioSource>();
    }
    public void Open()
    {
        sound.Play();
        healthBar.SetActive(false);
        transform.position = new Vector3(transform.position.x, transform.position.y - 10f, transform.position.z);
        StartCoroutine(astro.ShowTextForSecond("A Pillar has opened up ahead", 5));
        shieldText.text = "A pillar has opened ahead. Now reclaim what's yours.";
    }
}
