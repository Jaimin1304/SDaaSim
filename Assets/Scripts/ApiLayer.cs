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
        JObject dataPackage = new JObject { { "header", header }, { "body", bodyJson } };
        string JsonMsg = dataPackage.ToString();
        Debug.Log(JsonMsg);
        return cs.communicate(Globals.ip, Globals.port, JsonMsg);
    }
}
