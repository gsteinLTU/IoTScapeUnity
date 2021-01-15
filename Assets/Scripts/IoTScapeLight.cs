using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(IoTScapeObject)), RequireComponent(typeof(Light))]
public class IoTScapeLight : MonoBehaviour
{
    private Light light;
    private IoTScapeObject iotscapeobject;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        iotscapeobject = GetComponent<IoTScapeObject>();

        iotscapeobject.RegisterMethod("setEnabled", SetEnabled, new IoTScapeMethodDescription()
            {
                documentation = "Turn light on or off",
                paramsList = new List<IoTScapeMethodParams>
                {
                    new IoTScapeMethodParams
                    {
                        documentation = "on/off",
                        type = "string",
                        name = "status",
                        optional = false
                    }
                },
                returns = new IoTScapeMethodReturns()
                {
                    documentation = "",
                    type = new List<string>() { "void" }
                }
            });

        iotscapeobject.RegisterMethod("setIntensity", SetIntensity, new IoTScapeMethodDescription()
        {
            documentation = "Set light brightness",
            paramsList = new List<IoTScapeMethodParams>()
            {
                new IoTScapeMethodParams
                {
                    documentation = "Intensity value",
                    type = "float",
                    name = "intensity",
                    optional = false
                }
            },
            returns = new IoTScapeMethodReturns
            {
                documentation = "",
                type = new List<string> { "void" }
            }
        });

        iotscapeobject.RegisterMethod("setColor", SetColor, new IoTScapeMethodDescription()
        {
            documentation = "Set light color",
            paramsList = new List<IoTScapeMethodParams>()
            {
                new IoTScapeMethodParams
                {
                    documentation = "Red value",
                    type = "float",
                    name = "red",
                    optional = false
                },
                new IoTScapeMethodParams
                {
                    documentation = "Green value",
                    type = "float",
                    name = "green",
                    optional = false
                },
                new IoTScapeMethodParams
                {
                    documentation = "Blue value",
                    type = "float",
                    name = "blue",
                    optional = false
                },
            },
            returns = new IoTScapeMethodReturns
            {
                documentation = "",
                type = new List<string> { "void" }
            }
        });


        iotscapeobject.RegisterMethod("getIntensity", GetIntensity, new IoTScapeMethodDescription()
        {
            documentation = "Get current light color",
            paramsList = new List<IoTScapeMethodParams>()
            {

            },
            returns = new IoTScapeMethodReturns
            {
                documentation = "Colors of this light",
                type = new List<string> { "number" }
            }
        });

        iotscapeobject.RegisterMethod("getColor", GetColor, new IoTScapeMethodDescription()
        {
            documentation = "Get current light color",
            paramsList = new List<IoTScapeMethodParams>(),
            returns = new IoTScapeMethodReturns
            {
                documentation = "Intensity of this light",
                type = new List<string> { "number", "number", "number" }
            }
        });

        UpdateMaterial();
    }

    public string[] SetEnabled(params string[] input)
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

    public string[] SetIntensity(params string[] input)
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

    public string[] SetColor(params string[] input)
    {
        float r, g, b;

        if (input.Length == 3 && float.TryParse(input[0], out r) && float.TryParse(input[1], out g) && float.TryParse(input[2], out b))
        {
            light.color = new Color(r, g, b);
        }

        UpdateMaterial();

        return null;
    }

    public string[] GetColor(params string[] input)
    {
        return new[] { light.color.r.ToString(), light.color.b.ToString(), light.color.g.ToString() };
    }

    public string[] GetIntensity(params string[] input)
    {
        return new[] { light.intensity.ToString() };
    }

}
