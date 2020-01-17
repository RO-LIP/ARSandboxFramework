using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public Light directionaLight;
    public Light[] ambientLights;

    public Color32 directionalLightColorDay;
    public Color32 directionalLightColorNight;

    public Color32 ambientLightColorDay;
    public Color32 ambientLightColorNight;

    private bool isDay = true;

    private void Start()
    {
        SetDirectionalLightColor(directionalLightColorDay);
        SetAmbientLightColor(ambientLightColorDay);
    }

    public void ToggleNightDay()
    {
        isDay = !isDay;
        if (isDay)
        {
            SetDirectionalLightColor(directionalLightColorDay);
            SetAmbientLightColor(ambientLightColorDay);
        }
        else
        {
            SetDirectionalLightColor(directionalLightColorNight);
            SetAmbientLightColor(ambientLightColorNight);
        }
    }

    private void SetDirectionalLightColor(Color color)
    {
        directionaLight.color = color;
    }

    private void SetAmbientLightColor(Color color)
    {
        foreach (Light ambientLight in ambientLights)
        {
            ambientLight.color = color;
            if (ColorIsBlack(color))
            {
                ambientLight.gameObject.SetActive(false);
            } else
            {
                ambientLight.gameObject.SetActive(true);
            }
        }
    }

    private bool ColorIsBlack(Color color)
    {
        float channelSum = color.r + color.g + color.b;
        return channelSum == 0;
    }
}
