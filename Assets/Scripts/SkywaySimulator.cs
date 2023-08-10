using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

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

    public enum State
    {
        Play,
        Pause,
        Edit
    }

    [SerializeField]
    State currentState;

    public State CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    void Awake()
    {
        if (instance != null) // Singleton
        {
            Debug.LogError("More than one GameManager in scene!");
            return;
        }
        instance = this;
        // start the server
        //pyRunner.ExecutePythonScript();
    }

    void Start()
    {
        currentState = State.Edit;
        skyway.InitSkyway();
        InitSimulation();
    }

    void Update()
    {
        if (currentState == State.Play)
        {
            foreach (SubSwarm subSwarm in skyway.SubSwarms.Values)
            {
                subSwarm.UpdateLogic();
            }
        }
    }

    void InitSimulation()
    {
        // sent entire skyway to server
        string skywayJson = dataManager.RecordCurrentStateToJson(skyway);
        string response = api.SendRequest(Globals.initSkywayHeader, skywayJson);
    }

    public void UpdateSwarm(Swarm swarm)
    {
        string swarmJson = dataManager.SwarmToJson(swarm);
        string response = api.SendRequest(Globals.updateSwarmHeader, swarmJson);
    }

    public void UpdateSubSwarm(SubSwarm subSwarm)
    {
        string subSwarmJson = dataManager.SubSwarmToJson(subSwarm);
        string response = api.SendRequest(Globals.updateSubSwarmHeader, subSwarmJson);
        ProcessResponse(response);
    }

    void ProcessResponse(string response)
    {
        Debug.Log("Response: " + response); // print the entire response

        // Parse the response into a list of dictionary objects
        List<Dictionary<string, Dictionary<string, string>>> operations =
            JsonConvert.DeserializeObject<List<Dictionary<string, Dictionary<string, string>>>>(
                response
            );

        // Print the deserialized operations
        Debug.Log(JsonConvert.SerializeObject(operations, Formatting.Indented));

        // Iterate over the operations and execute each one
        foreach (var operation in operations)
        {
            string responseHeader = operation.Keys.First();
            Dictionary<string, string> responseBody = operation[responseHeader];
            switch (responseHeader)
            {
                case Globals.setSubswarmEdge:
                    Debug.Log("Operation: " + responseHeader);
                    string subSwarmId = responseBody["subswarm_id"];
                    string edgeId = responseBody["edge_id"];
                    Debug.Log("SubSwarm ID: " + subSwarmId);
                    Debug.Log("Edge ID: " + edgeId);
                    // Check if the key exists in the SubSwarms dictionary
                    if (skyway.SubSwarms.ContainsKey(subSwarmId))
                    {
                        SubSwarm subSwarm = skyway.SubSwarms[subSwarmId];
                        // Check if the key exists in the EdgeDict dictionary
                        if (skyway.EdgeDict.ContainsKey(edgeId))
                        {
                            Edge edge = skyway.EdgeDict[edgeId];
                            subSwarm.ToOperating(edge);
                        }
                        else
                        {
                            foreach (KeyValuePair<string, Edge> entry in skyway.EdgeDict)
                            {
                                Debug.Log("Key: " + entry.Key + ", Edge: " + entry.Value.Id);
                            }
                            Debug.LogError("Edge ID not found in skyway.EdgeDict: " + edgeId);
                        }
                    }
                    else
                    {
                        Debug.LogError("SubSwarm ID not found in skyway.SubSwarms: " + subSwarmId);
                    }
                    break;
                // Add more cases here if needed
                default:
                    Debug.Log("Response header not found: " + responseHeader);
                    break;
            }
        }
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Good Bye!");
    }
}
