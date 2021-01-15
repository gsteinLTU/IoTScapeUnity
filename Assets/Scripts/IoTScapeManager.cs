﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using JetBrains.Annotations;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;

public class IoTScapeManager : MonoBehaviour
{
    public static IoTScapeManager Manager;
    public IPAddress Host = IPAddress.Parse("52.73.65.98");
    public ushort Port = 1975;

    private Socket _socket;

    private int idprefix;
    private int lastid = 0;

    private Dictionary<string, IoTScapeObject> objects = new Dictionary<string, IoTScapeObject>();
    private EndPoint hostEndPoint;

    // Start is called before the first frame update
    void Start()
    {
        hostEndPoint = new IPEndPoint(Host, Port);
        Debug.Log(hostEndPoint);
        idprefix = Random.Range(0, 0x10000);
        Manager = this;

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.Bind(new IPEndPoint(IPAddress.Any, 0));

        // Reannounce services occasionally
        Timer reAnnounceTimer = new Timer(60 * 1000);
        reAnnounceTimer.Elapsed += (sender, args) => announceAll();
        reAnnounceTimer.Start();
    }

    /// <summary>
    /// Announces all services to server
    /// </summary>
    /// <param name="o">IoTScapeObject to announce</param>
    void announce(IoTScapeObject o)
    {
        string serviceJson = JsonConvert.SerializeObject(new Dictionary<string, IoTScapeServiceDefinition>(){{o.ServiceName, o.Definition}});
        Debug.Log($"Announcing service {o.ServiceName}");
        Debug.Log(serviceJson);
        _socket.SendTo(serviceJson.Select(c => (byte) c).ToArray(), SocketFlags.None, hostEndPoint);
        //_socket.SendToAsync(new ArraySegment<byte>(writer.ToString().Select(c => (byte) c).ToArray()), SocketFlags.None, hostEndPoint);
    }

    /// <summary>
    /// Announce all object-services to server
    /// </summary>
    void announceAll()
    {
        objects.Values.ToList().ForEach(announce);
    }

    /// <summary>
    /// Register an IoTScapeObject
    /// </summary>
    /// <param name="o">IoTScapeObject to register</param>
    public void Register(IoTScapeObject o)
    {
        o.Definition.id = idprefix.ToString("x4") + (lastid++).ToString("x4");
        objects.Add(o.Definition.id, o);
        announce(o);
    }

    // Update is called once per frame
    void Update()
    {
        // Parse incoming messages
        if (_socket.Available > 0)
        {
            byte[] incoming = new byte[2048];
            int len = _socket.Receive(incoming);

            string incomingString = Encoding.UTF8.GetString(incoming, 0, len);

            var json = JsonSerializer.Create();
            IoTScapeRequest request = json.Deserialize<IoTScapeRequest>(new JsonTextReader(new StringReader(incomingString)));
            Debug.Log(request);

            // Verify device exists
            if (objects.ContainsKey(request.device))
            {
                var device = objects[request.device];

                // Call function if valid
                if (device.RegisteredMethods.ContainsKey(request.function))
                {
                    string[] result = device.RegisteredMethods[request.function](request.ParamsList.ToArray());

                    IoTScapeResponse response = new IoTScapeResponse
                    {
                        id = request.device,
                        request = request.id,
                        service = request.service,
                        response = (result ?? new string[]{}).ToList()
                    };

                    // Send response
                    string responseJson = JsonConvert.SerializeObject(response,
                        new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});

                    _socket.SendTo(responseJson.Select(c => (byte)c).ToArray(), SocketFlags.None, hostEndPoint);
                }
            }
        }
    }
}

[Serializable]
public class IoTScapeServiceDefinition
{
    public string id;
    public IoTScapeServiceDescription service;
    public Dictionary<string,IoTScapeMethodDescription> methods = new Dictionary<string, IoTScapeMethodDescription>();
    public Dictionary<string, IoTScapeEventDescription> events = new Dictionary<string, IoTScapeEventDescription>();
}

public class IoTScapeEventDescription
{
    [JsonProperty(PropertyName = "params")]
    public List<string> paramsList = new List<string>();
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
    public string documentation;

    [JsonProperty(PropertyName = "params")]
    public List<IoTScapeMethodParams> paramsList = new List<IoTScapeMethodParams>();
    public IoTScapeMethodReturns returns;
}

[Serializable]
public class IoTScapeMethodParams
{
    public string name;
    public string documentation;
    public string type;
    public bool optional;
}

[Serializable]
public class IoTScapeMethodReturns
{
    public string documentation;
    public List<string> type = new List<string>();
}

[Serializable]
public class IoTScapeRequest
{
    public string id;
    public string service;
    public string device;
    public string function;

    [JsonProperty(PropertyName = "params")]
    public List<String> ParamsList = new List<string>();

    public override string ToString()
    {
        return $"IoTScape Request #{id}: call {service}/{function} on {device} with params [{string.Join(", ", ParamsList)}]";
    }
}

[Serializable]
public class IoTScapeResponse
{
    public string id;
    public string request;
    public string service;
    [CanBeNull] public List<string> response;

    [JsonProperty(PropertyName = "event")]
    [CanBeNull] public IoTScapeEventResponse EventResponse;

    [CanBeNull] public string error;

}

[Serializable]
public class IoTScapeEventResponse
{
    public string type;
    public string args;
}