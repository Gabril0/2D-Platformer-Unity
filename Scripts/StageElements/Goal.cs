using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private float currentTime, endingTime;
    private bool isBeaten = false;
    private TextMeshProUGUI timerText;
    void Awake()
    {
        currentTime = 0;
        timerText = GameObject.Find("Timer").GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBeaten)
        {
            currentTime += Time.deltaTime;
            timerText.text = currentTime.ToString();
        }
        else {
            timerText.text = endingTime.ToString();
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            endingTime = currentTime;
            isBeaten = true;
        }
    }
}
