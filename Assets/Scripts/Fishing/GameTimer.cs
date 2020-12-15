using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public int currentTime = 10;
    private int failTimer = 3;
    public int startingTime = 10;
    [SerializeField] Text countdownText;

    public void SetFailTimer(int failTime)
    {
        failTimer = failTime;
    }
    public void SetTimer(int time)
    {
        this.startingTime = time;
    }

    public int GetTimer()
    {
        return currentTime;
    }

    public void Decrement()
    {
        currentTime--;
        if (currentTime <= 0)
        {
            currentTime = 0;
        }
        countdownText.text = currentTime.ToString("0");
        if (currentTime <= failTimer)
        {
            countdownText.color = Color.red;
            
        }
    }

    public void ResetTimer()
    {
        currentTime = startingTime;
        countdownText.enabled = false;
    }
    
    void Start()
    {
        currentTime = startingTime;
        countdownText.enabled = true;
    }

    void Update()
    {

    }
}
