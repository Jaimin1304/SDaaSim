using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class SkywaySimulator : MonoBehaviour
{
    public static SkywaySimulator instance;

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
        if (instance != null) // Singleton
        {
            Debug.LogError("More than one GameManager in scene!");
            return;
        }
        instance = this;
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

    public void UpdateSwarm(Swarm swarm) {
        string swarmJson = dataManager.SwarmToJson(swarm);
        string response = api.SendRequest(Globals.updateSwarmHeader, swarmJson);
        Debug.Log(response);
    }

    public void UpdateSubSwarm(SubSwarm subSwarm) {
        string subSwarmJson = dataManager.SubSwarmToJson(subSwarm);
        string response = api.SendRequest(Globals.updateSubSwarmHeader, subSwarmJson);
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
