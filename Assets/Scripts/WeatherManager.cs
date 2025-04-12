using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class WeatherManager : MonoBehaviour
{
    [Header("Settings")]
    public float minTime = 1f;        // Minimum time in seconds
    public float maxTime = 240f;      // Maximum time in seconds
    public bool autoRestart = true;   // Automatically start new timer after completion

    [Header("Events")]
    public UnityEvent OnTimerCompleted;

    private float currentTime;
    private float targetTime;
    private Coroutine timerCoroutine;

    [Header("Inputs")]
    public GameObject boat;
    public GameObject rainEffect;


    void Start()
    {
        StartNewTimer();
    }

    public void StartNewTimer()
    {
        targetTime = Random.Range(minTime, maxTime);
        currentTime = 0f;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        while (currentTime < targetTime)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        //to do -- implement random generating direction/speed
        //boat.GetComponent<BoatController>().windDirection = 180;
        //boat.GetComponent<BoatController>().windSpeed = 2;

        //todo -- turn rain on and off

        OnTimerCompleted.Invoke();

        if (autoRestart)
        {
            StartNewTimer();
        }
    }

    // Optional: Add this if you want to display time remaining
    public float GetTimeRemaining()
    {
        return targetTime - currentTime;
    }
}
