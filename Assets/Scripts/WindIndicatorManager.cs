using UnityEngine;
using UnityEngine.UI;

public class WindIndicatorManager : MonoBehaviour
{
    [Header("References")]
    public Image windArrow;
    public Sprite[] windSpeedSprites;
    Vector2 windDirection;
    float windSpeed;
    float maxWindSpeed;

    void Start()
    {
        windDirection = GetComponent<WeatherManager>().GetWindDirection();
        windSpeed = GetComponent<WeatherManager>().GetWindSpeed();
        maxWindSpeed = GetComponent<WeatherManager>().maxWindSpeed;
    }

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
        windDirection = GetComponent<WeatherManager>().GetWindDirection();
        windSpeed = GetComponent<WeatherManager>().GetWindSpeed();
        float angle = Mathf.Atan2(windDirection.y, windDirection.x);
        Vector2 vWindDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        UpdateWindIndicator(vWindDirection, windSpeed);
    }
}