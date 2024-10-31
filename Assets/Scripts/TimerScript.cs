using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{

    public Text timerText;
    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<Text>();
        float GameTime = PlayerPrefs.GetFloat("TotalGameTime", 0f);
        timerText.text = "Time of Completion: " + GameTime.ToString("F2") + " seconds";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
