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
    ApiLayer api;

    [SerializeField]
    DataManager dataManager;

    [SerializeField]
    PythonRunner pyRunner;

    [SerializeField]
    ClientSocket cs;

    void Awake()
    {
        // start the server
        pyRunner.ExecutePythonScript();
    }

    void Start()
    {
        InitSimulation();
    }

    void Update()
    {
        //Debug.Log("update");
    }

    void InitSimulation()
    {
        // sent entire skyway to server
        string skywayJson = dataManager.RecordCurrentStateToJson(skyway);
        string response = api.SendRequest(Globals.initSkywayHeader, skywayJson);
        Debug.Log(response);
    }

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
