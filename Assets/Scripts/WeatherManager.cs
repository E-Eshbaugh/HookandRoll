using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;

public class WeatherManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float minTime = 1f;        // Minimum time in seconds
    public float maxTime = 240f;      // Maximum time in seconds
    public bool autoRestart = true;   // Automatically start new timer after completion

    [Header("Wind Settings")]
    public float minWindSpeed = 0f;
    public float maxWindSpeed = 2f;
    [Range(0, 360)] public float maxWindDirectionChange = 45f;
    private float currentWindSpeed;
    private Vector2 windDirection;
    [Header("Wind GUI Ref")]
    public Image windArrow;
    public Sprite[] windSpeedSprites;

    [Header("Rain Settings")]
    [Range(0, 1)] public float rainChangeProbability = 0.3f;
    public float rainFadeDuration = 5f;
    private bool isRaining;

    [Header("References")]
    public ParticleSystem rainEffect;
    public GameObject boat;  // For potential boat-specific weather effects

    [Header("Events")]
    public UnityEvent OnTimerCompleted;

    private float currentTime;
    private float targetTime;
    private Coroutine timerCoroutine;

    [Header("Debug")]
    public float speed;
    public Vector2 direction;

    void Start()
    {
        InitializeWeather();
        StartNewTimer();
    }

    void InitializeWeather()
    {
        // Set initial random wind direction and speed
        windDirection = Random.insideUnitCircle.normalized;
        currentWindSpeed = Random.Range(minWindSpeed, maxWindSpeed);
        
        // Initialize rain state
        if (rainEffect != null)
        {
            rainEffect.gameObject.SetActive(false);
        }
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

        // Update weather conditions
        GenerateNewWindConditions();
        ToggleRainState();

        OnTimerCompleted.Invoke();

        if (autoRestart)
        {
            StartNewTimer();
        }
    }

    private void GenerateNewWindConditions()
    {
        // Calculate new wind direction with limited change from previous
        float currentAngle = Vector2.SignedAngle(Vector2.right, windDirection);
        float angleChange = Random.Range(-maxWindDirectionChange, maxWindDirectionChange);
        windDirection = Quaternion.Euler(0, 0, currentAngle + angleChange) * Vector2.right;

        // Randomize speed within configured range
        currentWindSpeed = Random.Range(minWindSpeed, maxWindSpeed);

        direction = windDirection.normalized;
        speed = currentWindSpeed;
    }

    private void ToggleRainState()
    {
        if (rainEffect == null) return;

        // Random chance to change rain state
        if (Random.value <= rainChangeProbability)
        {
            if (isRaining)
            {
                StartCoroutine(FadeRain());
            }
            else
            {
                StartRain();
            }
            isRaining = !isRaining;
        }
    }

    private void StartRain()
    {
        rainEffect.gameObject.SetActive(true);
        rainEffect.Play();
    }

    private IEnumerator FadeRain()
    {
        var emission = rainEffect.emission;
        float startRate = emission.rateOverTime.constant;
        
        // Smoothly fade out rain
        float timer = 0f;
        while (timer < rainFadeDuration)
        {
            emission.rateOverTime = Mathf.Lerp(startRate, 0, timer / rainFadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        
        rainEffect.Stop();
        rainEffect.gameObject.SetActive(false);
    }

    public float GetTimeRemaining()
    {
        return targetTime - currentTime;
    }

    // Public accessors for other systems (e.g., sailboat physics)
    public Vector2 GetWindDirection() => windDirection.normalized;
    public float GetWindSpeed() => currentWindSpeed;

    //wind GUI stuff
    public void UpdateWindIndicator(Vector2 windDirection, float windSpeed)
    {
        RotateArrow(windDirection);
        UpdateSpeedVisual(windSpeed);
    }

    private void RotateArrow(Vector2 windDirection)
    {
        float angle = Mathf.Atan2(windDirection.y, windDirection.x) * Mathf.Rad2Deg;
        windArrow.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void UpdateSpeedVisual(float windSpeed)
    {
        if (windSpeedSprites.Length > 0)
        {
            int spriteIndex = Mathf.FloorToInt(windSpeed / maxWindSpeed * windSpeedSprites.Length);
            if (spriteIndex == 0){
                spriteIndex = 1;
            }

            if (windSpeed == 0)
            {
                spriteIndex = 0;
            }
            windArrow.sprite = windSpeedSprites[Mathf.Clamp(spriteIndex, 0, windSpeedSprites.Length - 1)];
        }
    }

    void Update()
    {
        UpdateWindIndicator(windDirection, currentWindSpeed);
    }
}