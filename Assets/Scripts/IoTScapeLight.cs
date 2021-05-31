using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class IoTScapeLight : MonoBehaviour
{
    private Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        UpdateMaterial();
    }

    public String[] SetEnabled(params String[] input)
    {
        if (input.Length == 1)
        {
            string[] truthivalues = {"on", "yes", "1", "true"};

            if (truthivalues.Contains(input[0].ToLower())) { 
                light.enabled = true;
            }
            else
            {
                light.enabled = false;
            }
            UpdateMaterial();
        }

        return null;
    }

    public String[] SetIntensity(params String[] input)
    {
        if (input.Length == 1 && float.TryParse(input[0], out var intensity))
        {
            light.intensity = intensity;
            UpdateMaterial();
        }

        return null;
    }

    /// <summary>
    /// Update emissive color to match light settings
    /// </summary>
    public void UpdateMaterial()
    {
        GetComponent<Renderer>().material.SetColor("_EmissionColor", (light.enabled? light.color : Color.black) * light.intensity / 2.5f);
    }

    public String[] SetColor(params String[] input)
    {
        float r, g, b;

        if (input.Length == 3 && float.TryParse(input[0], out r) && float.TryParse(input[1], out g) && float.TryParse(input[2], out b))
        {
            light.color = new Color(r, g, b);
        }

        UpdateMaterial();

        return null;
    }

    public String[] GetColor(params String[] input)
    {
        return new[] { light.color.r.ToString(), light.color.b.ToString(), light.color.g.ToString() };
    }

    public String[] GetIntensity(params String[] input)
    {
        return new[] { light.intensity.ToString() };
    }

}
