using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ApiLayer : MonoBehaviour
{
    public Skyway skyway;  // Assuming you have a reference to your Skyway object

    void ProcessResponse()
    {
        string skywayJson = JsonUtility.ToJson(skyway);
        string command = ClientSocket.communicate("127.0.0.1", 1235, "what is changed in system since last time");
        Debug.Log(command);

        // Process the command from the server
    }
}
