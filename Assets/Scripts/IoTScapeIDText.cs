using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Sets a TMP_Text to the ID of an IoTScapeObject
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class IoTScapeIDText : MonoBehaviour
{
    /// <summary>
    /// Prefix for displayed text
    /// </summary>
    public string Prefix = "ID: ";

    /// <summary>
    /// Object to get ID from
    /// </summary>
    public IoTScapeObject Object;

    /// <summary>
    /// Has the text been set to the ID yet
    /// </summary>
    private bool textSet = false;

    private TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If an ID has been assigned and has not been displayed, show it
        if (!textSet)
        {
            if (!string.IsNullOrEmpty(Object.Definition.id))
            {
                text.text = Prefix + Object.Definition.id;
                text.enabled = true;
                textSet = true;
            }
        }
    }
}
