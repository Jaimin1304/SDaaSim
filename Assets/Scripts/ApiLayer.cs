using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System;

public class ApiLayer : MonoBehaviour
{
    [SerializeField]
    ClientSocket cs;

    public string SendRequest(string header, string body) // Changed parameter type from string to object
    {
        JObject bodyJson = JObject.Parse(body);
        JObject dataPackage = new() { { "header", header }, { "body", bodyJson } };
        string jsonMsg = dataPackage.ToString();
        Debug.Log("Msg sent: " + jsonMsg);
        string jsonResponse = cs.Communicate(Globals.ip, Globals.port, jsonMsg);
        return jsonResponse;
    }
}
