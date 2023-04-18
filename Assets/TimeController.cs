using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    private float timeMultiplier;

    [SerializeField]
    private float startHour;

    [SerializeField]
    private Light sunlight;

    [SerializeField]
    private float sunriseHour;

    [SerializeField]
    private float sunsetHour;

    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;

    private DateTime currentTime;

    [SerializeField]
    private Color dayAmbientLight;

    [SerializeField]
    private Color nightAmbientLight;

    [SerializeField]
    private AnimationCurve lightChangeCurve;

    [SerializeField]
    private float maxSunlightIntensity;

    [SerializeField]
    private Light moonlight;

    [SerializeField]
    private float maxMoonlightIntensity;
    
    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
    }

    // Update is called once per frame
    void Update()
    {
        updateTimeOfDay();
        rotateSunlightDirection();
        updateLightSettings();
    }

    private void updateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
    }

    private void rotateSunlightDirection()
    {
        float sunlightAngle;
        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime){
            TimeSpan sunriseToSunsetTimeSpan = calculateTimeDiff(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunrise = calculateTimeDiff(currentTime.TimeOfDay, sunriseTime);
            double ratio = timeSinceSunrise.TotalMinutes / sunriseToSunsetTimeSpan.TotalMinutes;
            sunlightAngle = Mathf.Lerp(0, 180, (float)ratio);
        }
        else{
            TimeSpan sunsetToSunriseTimeSpan = calculateTimeDiff(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunset = calculateTimeDiff(currentTime.TimeOfDay, sunsetTime);
            double ratio = timeSinceSunset.TotalMinutes / sunsetToSunriseTimeSpan.TotalMinutes;
            sunlightAngle = Mathf.Lerp(180, 360, (float)ratio);
        }
        sunlight.transform.rotation = Quaternion.AngleAxis(sunlightAngle, Vector3.right);
    }

    private TimeSpan calculateTimeDiff(TimeSpan t2, TimeSpan t1)
    {
        TimeSpan diff = t2 - t1;
        if (diff.TotalSeconds < 0){
            diff += TimeSpan.FromHours(24);
        }
        return diff;
    }

    private void updateLightSettings()
    {
        float dotProduct = Vector3.Dot(sunlight.transform.forward, Vector3.down);
        sunlight.intensity = Mathf.Lerp(0, maxSunlightIntensity, lightChangeCurve.Evaluate(dotProduct));
        moonlight.intensity = Mathf.Lerp(maxMoonlightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));
    }
}
