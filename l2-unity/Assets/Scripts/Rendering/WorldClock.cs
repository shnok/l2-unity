using System;
using UnityEngine;

[System.Serializable]
public struct WorldTimer
{
    public float dayStartTime; //0.25f
    public float dayEndTime; //0.75f
    public float sunriseStartTime; //0f
    public float sunriseEndTime; //0.15f
    public float sunsetStartTime; //0.85f
    public float sunsetEndTime; //0.99f
}

[System.Serializable]
public struct Clock
{
    public float totalRatio;
    [Header("Day/Night cycle")]
    public float dayRatio;
    public float nightRatio;
    [Header("Full day cycles")]
    public float dawnRatio;
    public float brightRatio;
    public float duskRatio;
    public float darkRatio;
}

[ExecuteInEditMode]
public class WorldClock : MonoBehaviour
{
    [SerializeField] private float _dayDurationMinutes = 240;
    [SerializeField] private string _timeHour;
    [SerializeField] private float _timeElapsed = 0;
    [SerializeField] private bool _startClock;
    [SerializeField] private WorldTimer _worldTimer;
    [SerializeField] private Clock _clock;

    public Clock Clock { get { return _clock; } }

    private static WorldClock _instance;
    public static WorldClock Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void Update()
    {
        if (_startClock)
        {
            UpdateClock();
        }

        CalculateDayNightRatio();
        CalculateSunPhaseRatio();
    }

    private void UpdateClock()
    {
        _timeElapsed += Time.deltaTime;

        if (_timeElapsed >= (_dayDurationMinutes * 60f))
        {
            _timeElapsed = 0;
        }

        _clock.totalRatio = _timeElapsed / (_dayDurationMinutes * 60f);

        // Calculate the number of seconds based on the percentage
        int seconds = (int)(_clock.totalRatio * 86400);

        // Convert seconds to hours, minutes, and seconds
        int hours = seconds / 3600;
        int minutes = (seconds % 3600) / 60;
        int remainingSeconds = seconds % 60;

        // Create a TimeSpan object with the calculated hours, minutes, and seconds
        TimeSpan time = new TimeSpan(hours, minutes, remainingSeconds);

        // Format the TimeSpan object as a string in the desired format (HH:mm:ss)
        _timeHour = time.ToString(@"hh\:mm\:ss");
    }

    public void SynchronizeClock(int currentGameTime)
    {
        //Gametime goes from 0 to 1439
        float serverDayRatio = currentGameTime / 1439f;
        _timeElapsed = serverDayRatio * _dayDurationMinutes * 60f;
    }

    public bool IsNightTime()
    {
        return _clock.nightRatio > 0 && _clock.nightRatio <= 1 || _clock.dayRatio < 0.25f;
    }

    private void CalculateDayNightRatio()
    {
        if (_clock.totalRatio >= _worldTimer.dayStartTime && _clock.totalRatio < _worldTimer.dayEndTime)
        {
            _clock.nightRatio = 0;
            _clock.dayRatio = (_clock.totalRatio - _worldTimer.dayStartTime) / (_worldTimer.dayEndTime - _worldTimer.dayStartTime);
        }
        else
        {
            _clock.dayRatio = 0;
            if (_clock.totalRatio >= _worldTimer.dayEndTime)
            {
                _clock.nightRatio = (_clock.totalRatio - _worldTimer.dayEndTime) / (1.0f - _worldTimer.dayEndTime + _worldTimer.dayStartTime);
            }
            else
            {
                _clock.nightRatio = (_clock.totalRatio + (1.0f - _worldTimer.dayEndTime)) / (1.0f - _worldTimer.dayEndTime + _worldTimer.dayStartTime);
            }
        }
    }

    private void CalculateSunPhaseRatio()
    {
        _clock.dawnRatio = CalculatePeriodRatio(_worldTimer.sunriseStartTime, _worldTimer.sunriseEndTime);
        _clock.brightRatio = CalculatePeriodRatio(_worldTimer.sunriseEndTime, _worldTimer.sunsetStartTime);
        _clock.duskRatio = CalculatePeriodRatio(_worldTimer.sunsetStartTime, _worldTimer.sunsetEndTime);
        _clock.darkRatio = CalculatePeriodRatio(-.99f, _worldTimer.sunriseStartTime);
    }

    private float CalculatePeriodRatio(float startRatio, float endRatio)
    {
        float periodDuration = (startRatio < 0) ? (Mathf.Abs(startRatio) + endRatio) : (endRatio - startRatio);

        float ratio = 0;
        if (_clock.dayRatio >= 0 && _clock.nightRatio == 0)
        {
            // Day
            if (_clock.dayRatio <= endRatio)
            {
                // In Range        
                if (startRatio < 0)
                {
                    ratio += Mathf.Abs(startRatio);
                    ratio += _clock.dayRatio;
                }
                else if (_clock.dayRatio >= startRatio)
                {
                    ratio -= startRatio;
                    ratio += _clock.dayRatio;
                }
                else
                {
                    ratio = 0;
                }

                ratio = Mathf.Clamp(ratio / periodDuration, 0, 1);
            }
            else
            {
                ratio = 1;
            }
        }
        else
        {
            // Night
            if (startRatio < 0 && _clock.nightRatio > (1 + startRatio))
            {
                ratio += Mathf.Clamp((_clock.nightRatio - (1 + startRatio)) / periodDuration, 0, 1);
            }
            else
            {
                ratio = 0;
            }
        }

        return ratio;
    }
}
