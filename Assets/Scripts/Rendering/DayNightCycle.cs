using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[ExecuteInEditMode]
public class DayNightCycle : MonoBehaviour
{
    public float timeOfTheday = 0;
    public float dayDurationInSec = 30;
    public string timeHour;
    public float dayPercent;

    public float dayStartRatio = 0.25f;
    public float dayEndRatio = 0.75f;
    public float dayRatio;
    public float nightRatio;

    private int totalSecondsInDay = 86400;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateClock();
        UpdateDayNightRatio();
    }

    private void UpdateClock() {
        timeOfTheday += Time.deltaTime;
        if(timeOfTheday >= dayDurationInSec) {
            timeOfTheday = 0;
        }

        dayPercent = timeOfTheday / dayDurationInSec;

        // Calculate the number of seconds based on the percentage
        int seconds = (int)(dayPercent * totalSecondsInDay);

        // Convert seconds to hours, minutes, and seconds
        int hours = seconds / 3600;
        int minutes = (seconds % 3600) / 60;
        int remainingSeconds = seconds % 60;

        // Create a TimeSpan object with the calculated hours, minutes, and seconds
        TimeSpan time = new TimeSpan(hours, minutes, remainingSeconds);

        // Format the TimeSpan object as a string in the desired format (HH:mm:ss)
        timeHour = time.ToString(@"hh\:mm\:ss");
    }

    private void UpdateDayNightRatio() {
        if(dayPercent >= dayStartRatio && dayPercent < dayEndRatio) {
            nightRatio = 0;
            dayRatio = (dayPercent - dayStartRatio) / (dayEndRatio - dayStartRatio);
        } else {
            dayRatio = 0;
            if(dayPercent >= dayEndRatio) {
                nightRatio = (dayPercent - dayEndRatio) / (1.0f - dayEndRatio + dayStartRatio);
            } else {
                nightRatio = (dayPercent + (1.0f - dayEndRatio)) / (1.0f - dayEndRatio + dayStartRatio);
            }
        }
    }
}
