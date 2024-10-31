using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{

    public Slider slider;
    public Gradient gradient;
    public Image fill;
    // Start is called before the first frame update

    private void Start()
    {
        slider = GetComponent<Slider>();
    }
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(20f);
    }

    public void SetHealth(int health)
    {
        if (health < 0)
            health = 0;
        slider.value = health;


        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
