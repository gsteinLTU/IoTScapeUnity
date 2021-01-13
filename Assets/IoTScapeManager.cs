using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Newtonsoft.Json;

public class IoTScapeManager : MonoBehaviour
{
    public static IoTScapeManager Manager;
    public string Host = "localhost";
    public ushort Port = 1975;

    private Socket _socket;

    private List<IoTScapeObject> objects = new List<IoTScapeObject>();

    // Start is called before the first frame update
    void Start()
    {
        Manager = this;

        _socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
        _socket.Bind(new IPEndPoint(IPAddress.Any, 0));
    }

    /// <summary>
    /// Announces all services to server
    /// </summary>
    /// <param name="o">IoTScapeObject to announce</param>
    void announce(IoTScapeObject o)
    {

    }

    /// <summary>
    /// Announce all object-services to server
    /// </summary>
    void announceAll()
    {
        objects.ForEach(announce);
    }

    /// <summary>
    /// Register an IoTScapeObject
    /// </summary>
    /// <param name="o">IoTScapeObject to register</param>
    public void Register(IoTScapeObject o)
    {
        objects.Add(o);
        announce(o);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
class IoTScapeServiceDefinition
{
    public string id;
    public IoTScapeServiceDescription service;
    public Dictionary<string,IoTScapeMethodDescription> methods;
}

[Serializable]
public class IoTScapeServiceDescription
{
    public string description;
    public string externalDocumentation;
    public string termsOfService;
    public string contact;
    public string license;
    public string version;
}

[Serializable]
public class IoTScapeMethodDescription
{
    public string Documentation;
    public List<string> ParamsList;
    public IoTScapeMethodReturns Returns;
}

[Serializable]
public class IoTScapeMethodReturns
{
    public string Documentation;
    public List<string> TypesList;
}