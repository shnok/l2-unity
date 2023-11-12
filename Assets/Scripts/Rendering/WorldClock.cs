using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WorldTimer {
    public float dayStartTime; //0.25f
    public float dayEndTime; //0.75f
    public float sunriseStartTime; //-0.10f
    public float sunriseEndTime; //0.15f
    public float sunsetStartTime; //0.85f
    public float sunsetEndTime; //0.99f
}

[System.Serializable]
public struct Clock {
    public float dayRatio;
    public float nightRatio;
    public float totalRatio;
    public float dawnRatio;
    public float brightRatio;
    public float duskRatio;
    public float darkRatio;
}

[ExecuteInEditMode]
public class WorldClock : MonoBehaviour
{
    public float timeOfTheday = 0;
    public float dayDurationInSec = 30;
    public string timeHour;

    public WorldTimer worldTimer;
    public Clock worldClock;

    //public float dayStartRatio = 0.25f;
    // public float worldTimer.dayEndTime = 0.75f;
    // public float dayRatio;
    // public float nightRatio;
    // Define time when dawn end and dusk start
    /*float sunriseStartTime = -0.10f; // night
    float sunriseEndTime = 0.15f; // day
    float sunsetStartTime = 0.85f; // day
    float sunsetEndTime = 0.99f; // day*/


    void Update()
    {
        UpdateClock();
        CalculateDayNightRatio();
        CalculateSunPhaseRatio();
    }

    private void UpdateClock() {
        timeOfTheday += Time.deltaTime;
        if(timeOfTheday >= dayDurationInSec) {
            timeOfTheday = 0;
        }


        worldClock.totalRatio = timeOfTheday / dayDurationInSec;

        // Calculate the number of seconds based on the percentage
        int seconds = (int)(worldClock.totalRatio * 86400);

        // Convert seconds to hours, minutes, and seconds
        int hours = seconds / 3600;
        int minutes = (seconds % 3600) / 60;
        int remainingSeconds = seconds % 60;

        // Create a TimeSpan object with the calculated hours, minutes, and seconds
        TimeSpan time = new TimeSpan(hours, minutes, remainingSeconds);

        // Format the TimeSpan object as a string in the desired format (HH:mm:ss)
        timeHour = time.ToString(@"hh\:mm\:ss");
    }

    private void CalculateDayNightRatio() {
        if(worldClock.totalRatio >= worldTimer.dayStartTime && worldClock.totalRatio < worldTimer.dayEndTime) {
            worldClock.nightRatio = 0;
            worldClock.dayRatio = (worldClock.totalRatio - worldTimer.dayStartTime) / (worldTimer.dayEndTime - worldTimer.dayStartTime);
        } else {
            worldClock.dayRatio = 0;
            if(worldClock.totalRatio >= worldTimer.dayEndTime) {
                worldClock.nightRatio = (worldClock.totalRatio - worldTimer.dayEndTime) / (1.0f - worldTimer.dayEndTime + worldTimer.dayStartTime);
            } else {
                worldClock.nightRatio = (worldClock.totalRatio + (1.0f - worldTimer.dayEndTime)) / (1.0f - worldTimer.dayEndTime + worldTimer.dayStartTime);
            }
        }
    }

    private void CalculateSunPhaseRatio() {
        worldClock.dawnRatio = CalculatePeriodRatio(worldTimer.sunriseStartTime, worldTimer.sunriseEndTime);
        worldClock.brightRatio = CalculatePeriodRatio(worldTimer.sunriseEndTime, worldTimer.sunsetStartTime);
        worldClock.duskRatio = CalculatePeriodRatio(worldTimer.sunsetStartTime, worldTimer.sunsetEndTime);
        worldClock.darkRatio = CalculatePeriodRatio(-.99f, worldTimer.sunriseStartTime);
    }

    private float CalculatePeriodRatio(float startRatio, float endRatio) {
        float periodDuration = (startRatio < 0) ? (Mathf.Abs(startRatio) + endRatio) : (endRatio - startRatio);

        float ratio = 0;
        if(worldClock.dayRatio >= 0 && worldClock.nightRatio == 0) {
            // Day
            if(worldClock.dayRatio <= endRatio) {
                // In Range        
                if(startRatio < 0) {
                    ratio += Mathf.Abs(startRatio);
                    ratio += worldClock.dayRatio;
                } else if(worldClock.dayRatio >= startRatio) {
                    ratio -= startRatio;
                    ratio += worldClock.dayRatio;
                } else {
                    ratio = 0;
                }

                ratio = Mathf.Clamp(ratio / periodDuration, 0, 1);
            } else {
                ratio = 1;
            }
        } else {
            // Night
            if(startRatio < 0 && worldClock.nightRatio > (1 + startRatio)) {
                ratio += Mathf.Clamp((worldClock.nightRatio - (1 + startRatio)) / periodDuration, 0, 1);
            } else {
                ratio = 0;
            }
        }

        return ratio;
    }
}
