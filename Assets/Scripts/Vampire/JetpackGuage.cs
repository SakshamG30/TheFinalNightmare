using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JetpackGuage : MonoBehaviour
{
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {

    }
    public void fillGauge(float time)
    {
        Mathf.Lerp(slider.minValue, slider.maxValue, Time.deltaTime * 1f);
        slider.value += time;
    }

    public void DepleteGauge(float value)
    {
        slider.value -= value;

        if (slider.value < slider.minValue)
        {
            slider.value = slider.minValue;
        }
    }
}
