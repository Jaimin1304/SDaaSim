using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class SkywaySimulator : MonoBehaviour
{
    [SerializeField]
    Skyway skyway;

    [SerializeField]
    ApiLayer apiLayer;

    [SerializeField]
    DataManager dataManager;

    [SerializeField]
    ClientSocket clientSocket;

    [SerializeField]
    PythonRunner pythonRunner;

    void Start()
    {
        // start the server
        pythonRunner.ExecutePythonScript();
        // sent entire skyway to server
        string skywayJson = dataManager.RecordCurrentStateToJson(skyway);
        Debug.Log(skywayJson);
        string res = ClientSocket.communicate(
            "127.0.0.1",
            1235,
            "what is changed in system since last time"
        );
    }

    void Update() { }

    void quitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Good Bye!");
    }
}
