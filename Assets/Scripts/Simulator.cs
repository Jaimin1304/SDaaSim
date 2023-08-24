using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

public class Simulator : MonoBehaviour
{
    public static Simulator instance;

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

    float elapsedTime = 0f;

    public enum State
    {
        Play,
        Pause,
        Edit,
        Freeze
    }

    [SerializeField]
    State currentState;

    public float ElapsedTime
    {
        get { return elapsedTime; }
        set { elapsedTime = value; }
    }

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
        { // simulation logic
            foreach (SubSwarm subSwarm in skyway.SubSwarms.Values)
            {
                subSwarm.UpdateLogic();
            }
            elapsedTime += Time.deltaTime * Globals.PlaySpeed; // update timer
        }
    }

    void InitSimulation()
    {
        // sent entire skyway to server
        string skywayJson = dataManager.RecordCurrentStateToJson(skyway);
        string response = api.SendRequest(Globals.initSkywayHeader, skywayJson);
        ProcessResponse(response);
    }

    public void UpdateSwarm(Swarm swarm)
    {
        string swarmJson = dataManager.SwarmToJson(swarm);
        string response = api.SendRequest(Globals.updateSwarmHeader, swarmJson);
        ProcessResponse(response);
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
            Debug.Log("Operation: " + responseHeader);
            switch (responseHeader)
            {
                case Globals.setSubswarmEdge:
                    Debug.Log("Operation: ");
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
                            Debug.LogError("Edge ID not found in skyway.EdgeDict: " + edgeId);
                        }
                    }
                    else
                    {
                        Debug.LogError("SubSwarm ID not found in skyway.SubSwarms: " + subSwarmId);
                    }
                    break;

                case Globals.subswarmLand:
                    subSwarmId = responseBody["subswarm_id"];
                    string nodeId = responseBody["node_id"];
                    Debug.Log("SubSwarm ID: " + subSwarmId);
                    Debug.Log("Edge ID: " + nodeId);
                    // Check if the key exists in the SubSwarms dictionary
                    if (skyway.SubSwarms.ContainsKey(subSwarmId))
                    {
                        SubSwarm subSwarm = skyway.SubSwarms[subSwarmId];
                        // Check if the key exists in the NodeDict dictionary
                        if (skyway.NodeDict.ContainsKey(nodeId))
                        {
                            Node node = skyway.NodeDict[nodeId];
                            subSwarm.ToLanded(node);
                        }
                        else
                        {
                            Debug.LogError("Node ID not found in skyway.NodeDict: " + nodeId);
                        }
                    }
                    else
                    {
                        Debug.LogError("SubSwarm ID not found in skyway.SubSwarms: " + subSwarmId);
                    }
                    break;

                case Globals.splitSubswarm:
                    subSwarmId = responseBody["subswarm_id"];
                    string droneLst = responseBody["drone_lst"];
                    Debug.Log("SubSwarm ID: " + subSwarmId);
                    Debug.Log("Drone list: " + droneLst);
                    // Check if the key exists in the SubSwarms dictionary
                    if (skyway.SubSwarms.ContainsKey(subSwarmId))
                    {
                        SubSwarm subSwarm = skyway.SubSwarms[subSwarmId];
                        Debug.Log("split according to drone list");
                    }
                    else
                    {
                        Debug.LogError("SubSwarm ID not found in skyway.SubSwarms: " + subSwarmId);
                    }
                    break;

                case Globals.mergeTwoSubswarms:
                    String subSwarmAId = responseBody["subswarmA_id"];
                    String subSwarmBId = responseBody["subswarmB_id"];
                    Debug.Log("SubSwarmA ID: " + subSwarmAId);
                    Debug.Log("SubSwarmB ID: " + subSwarmBId);
                    // Check if the key exists in the SubSwarms dictionary
                    if (
                        skyway.SubSwarms.ContainsKey(subSwarmAId)
                        && skyway.SubSwarms.ContainsKey(subSwarmBId)
                    )
                    {
                        Debug.Log("merge");
                    }
                    else
                    {
                        Debug.LogError(
                            string.Format(
                                "SubSwarm ID not found in skyway.SubSwarms, could be {0} or {1}",
                                subSwarmAId,
                                subSwarmBId
                            )
                        );
                    }
                    break;

                // Add more cases here if needed
                default:
                    Debug.LogError("Response header not found: " + responseHeader);
                    break;
            }
        }
    }

    public void SaveSkyway()
    {
        dataManager.SaveSkywayToJson(skyway);
    }

    void CheckComplete() { }

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
