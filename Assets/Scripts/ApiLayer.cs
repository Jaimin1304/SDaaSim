using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApiLayer : MonoBehaviour
{
    void HandleUserCommand()
    {
        string msg = "hello from client";
        string command = ClientSocket.communicate("127.0.0.1", 1235, msg);
        Debug.Log(command);
    }

    void Update()
    {
        HandleUserCommand();
    }
}
