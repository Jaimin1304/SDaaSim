using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.Events;

[System.Serializable]
public class BoolEvent : UnityEvent<bool> { }

public class Simulator : MonoBehaviour
{
    public static Simulator instance;

    [SerializeField]
    Skyway skywayPrefab;

    [SerializeField]
    Node nodePrefab;

    [SerializeField]
    Pad padPrefab;

    [SerializeField]
    Edge edgePrefab;

    [SerializeField]
    WayPoint wayPointPrefab;

    [SerializeField]
    Request requestPrefab;

    [SerializeField]
    Swarm swarmPrefab;

    [SerializeField]
    SubSwarm subSwarmPrefab;

    [SerializeField]
    Drone dronePrefab;

    [SerializeField]
    Payload payloadPrefab;

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

    [SerializeField]
    RaycastHandler raycastHandler;

    float elapsedTime = 0f;

    public enum State
    {
        Play,
        Pause,
        Edit,
        Freeze,
        Finished
    }

    [SerializeField]
    State currentState;

    [SerializeField]
    State lastState;

    // Define UnityEvents for each state change
    public UnityEvent OnPlayEvent;
    public UnityEvent OnPauseEvent;
    public UnityEvent OnFreezeEvent;
    public UnityEvent OnEditEvent;
    public BoolEvent OnFinishEvent;

    public Skyway Skyway
    {
        get { return skyway; }
        set { skyway = value; }
    }

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

    public State LastState
    {
        get { return lastState; }
        set { lastState = value; }
    }

    public Drone DronePrefab
    {
        get { return dronePrefab; }
        set { dronePrefab = value; }
    }

    public Swarm SwarmPrefab
    {
        get { return swarmPrefab; }
        set { swarmPrefab = value; }
    }

    public SubSwarm SubSwarmPrefab
    {
        get { return subSwarmPrefab; }
        set { subSwarmPrefab = value; }
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
        pyRunner.ExecutePythonScript();

        // Initialize the UnityEvents
        OnPlayEvent = new UnityEvent();
        OnPauseEvent = new UnityEvent();
        OnFreezeEvent = new UnityEvent();
        OnEditEvent = new UnityEvent();
        OnFinishEvent = new BoolEvent();
    }

    void Start()
    {
        currentState = State.Edit;
        lastState = State.Edit;
        //skyway = FindObjectOfType<Skyway>();
        skyway.InitSkyway();
        //InitSimulation();
    }

    void Update()
    {
        if (currentState == State.Play) // simulation logic
        {
            foreach (SubSwarm subSwarm in skyway.SubSwarmDict.Values)
            {
                subSwarm.UpdateLogic();
            }
            elapsedTime += Time.deltaTime * Globals.PlaySpeed; // update timer
        }
    }

    public void InitSimulation()
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

    public void UpdateDrones(SubSwarm subSwarm)
    {
        string dronesJson = dataManager.DronesToJson(subSwarm);
        string response = api.SendRequest(Globals.updateDronesHeader, dronesJson);
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
            // pass if proceed
            if (responseHeader == Globals.proceed)
            {
                return;
            }
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
                    if (skyway.SubSwarmDict.ContainsKey(subSwarmId))
                    {
                        SubSwarm subSwarm = skyway.SubSwarmDict[subSwarmId];
                        // Check if the key exists in the EdgeDict dictionary
                        if (skyway.EdgeDict.ContainsKey(edgeId))
                        {
                            Edge edge = skyway.EdgeDict[edgeId];
                            subSwarm.ToFlying(edge);
                        }
                        else
                        {
                            Debug.LogError("Edge ID not found in skyway.EdgeDict: " + edgeId);
                        }
                    }
                    else
                    {
                        Debug.LogError(
                            "SubSwarm ID not found in skyway.SubSwarmDict: " + subSwarmId
                        );
                    }
                    break;

                case Globals.subswarmLand:
                    subSwarmId = responseBody["subswarm_id"];
                    string nodeId = responseBody["node_id"];
                    Debug.Log("SubSwarm ID: " + subSwarmId);
                    Debug.Log("Edge ID: " + nodeId);
                    // Check if the key exists in the SubSwarms dictionary
                    if (skyway.SubSwarmDict.ContainsKey(subSwarmId))
                    {
                        SubSwarm subSwarm = skyway.SubSwarmDict[subSwarmId];
                        // Check if the key exists in the NodeDict dictionary
                        if (skyway.NodeDict.ContainsKey(nodeId))
                        {
                            Node node = skyway.NodeDict[nodeId];
                            subSwarm.ToRecharging(node);
                        }
                        else
                        {
                            Debug.LogError("Node ID not found in skyway.NodeDict: " + nodeId);
                        }
                    }
                    else
                    {
                        Debug.LogError(
                            "SubSwarm ID not found in skyway.SubSwarmDict: " + subSwarmId
                        );
                    }
                    break;

                case Globals.splitSubswarm:
                    subSwarmId = responseBody["subswarm_id"];
                    string droneLst = responseBody["drone_lst"];
                    Debug.Log("SubSwarm ID: " + subSwarmId);
                    Debug.Log("Drone list: " + droneLst);
                    // Check if the key exists in the SubSwarms dictionary
                    if (skyway.SubSwarmDict.ContainsKey(subSwarmId))
                    {
                        SubSwarm subSwarm = skyway.SubSwarmDict[subSwarmId];
                        Debug.Log("split according to drone list");
                    }
                    else
                    {
                        Debug.LogError(
                            "SubSwarm ID not found in skyway.SubSwarmDict: " + subSwarmId
                        );
                    }
                    break;

                case Globals.mergeTwoSubswarms:
                    String subSwarmAId = responseBody["subswarmA_id"];
                    String subSwarmBId = responseBody["subswarmB_id"];
                    Debug.Log("SubSwarmA ID: " + subSwarmAId);
                    Debug.Log("SubSwarmB ID: " + subSwarmBId);
                    // Check if the key exists in the SubSwarms dictionary
                    if (
                        skyway.SubSwarmDict.ContainsKey(subSwarmAId)
                        && skyway.SubSwarmDict.ContainsKey(subSwarmBId)
                    )
                    {
                        Debug.Log("merge");
                    }
                    else
                    {
                        Debug.LogError(
                            string.Format(
                                "SubSwarm ID not found in skyway.SubSwarmDict, could be {0} or {1}",
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

    public void ToPlay()
    {
        if (currentState == State.Edit)
        {
            InitSimulation();
        }
        lastState = currentState;
        currentState = State.Play;
        OnPlayEvent.Invoke(); // Invoke the OnPlayEvent
    }

    public void ToPause()
    {
        lastState = currentState;
        currentState = State.Pause;
        OnPauseEvent.Invoke(); // Invoke the OnPauseEvent
    }

    public void ToFreeze()
    {
        lastState = currentState;
        currentState = State.Freeze;
        OnFreezeEvent.Invoke(); // Invoke the OnFreezeEvent
    }

    public void ToEdit()
    {
        lastState = currentState;
        currentState = State.Edit;
        OnEditEvent.Invoke(); // Invoke the OnEditEvent
    }

    public void ToFinish(bool saveCSV = true)
    {
        lastState = currentState;
        currentState = State.Finished;
        OnFinishEvent.Invoke(saveCSV); // Invoke the OnFinishEvent
        if (saveCSV)
        {
            dataManager.SaveAllDroneDataToCSV(skyway.DroneDict.Values.ToList());
        }
    }

    public void ToLastState()
    {
        switch (lastState)
        {
            case State.Play:
                ToPlay();
                break;
            case State.Edit:
                ToEdit();
                break;
            case State.Pause:
                ToPause();
                break;
            case State.Freeze:
                ToFreeze();
                break;
            default:
                Debug.LogError("Invalid simulator state");
                break;
        }
    }

    public void CheckSimulationComplete()
    {
        bool complete = true;
        foreach (Swarm swarm in skyway.Swarms)
        {
            foreach (SubSwarm subSwarm in swarm.SubSwarms)
            {
                if (subSwarm.CurrentState != SubSwarm.State.Arrived)
                {
                    complete = false;
                }
            }
        }
        if (complete)
        {
            ToFinish();
        }
    }

    Vector3 CamFrontPosition()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;
        // Calculate the position in the front of the camera
        return cameraPosition + cameraForward * Globals.objectCreationDistance;
    }

    public void CreateNode()
    {
        Debug.Log("CreateNode");
        Vector3 creationPosition = CamFrontPosition();
        // Instantiate the node at the calculated position
        Node newNode = Instantiate(nodePrefab, creationPosition, Quaternion.identity);
        skyway.AddNode(newNode);
    }

    public void DeleteNode(Node node)
    {
        Debug.Log("DeleteNode");
        raycastHandler.SelectedObject = null;
        skyway.RemoveNode(node);
    }

    public void CreateEdge(Node a, Node b)
    {
        if (Utils.HasEdgeBetween(a, b))
        {
            return;
        }
        Debug.Log("CreateEdge");
        Edge newEdge = Instantiate(edgePrefab, Vector3.zero, Quaternion.identity);
        newEdge.LeftNode = a;
        newEdge.RightNode = b;
        a.Edges.Add(newEdge);
        b.Edges.Add(newEdge);
        skyway.AddEdge(newEdge);
    }

    public void DeleteEdge(Edge edge)
    {
        Debug.Log("DeleteEdge");
        raycastHandler.SelectedObject = null;
        skyway.RemoveEdge(edge);
    }

    public void CreateWayPoint(Edge edge, WayPoint baseWayPoint)
    {
        Debug.Log("CreateWayPoint");
        // If baseWayPoint is null and edge already has waypoints, then return early
        if (!baseWayPoint && edge.WayPoints.Any())
        {
            Debug.Log("Edge already has waypoints");
            return;
        }
        // Instantiate waypoint object
        WayPoint newWayPoint = Instantiate(wayPointPrefab, Vector3.zero, Quaternion.identity);
        skyway.AddWayPoint(edge, newWayPoint, baseWayPoint);
    }

    public void DeleteWayPoint(WayPoint wayPoint)
    {
        Debug.Log("DeleteWayPoint");
        raycastHandler.SelectedObject = null;
        skyway.RemoveWayPoint(wayPoint);
    }

    public void EditPad(Node node, bool add, bool rechargeable)
    {
        if (add)
        {
            Debug.Log("AddPad");
            node.AddPad(rechargeable);
        }
        else
        {
            Debug.Log("RemovePad");
            node.RemovePad(rechargeable);
        }
    }

    public void CreateRequest()
    {
        Debug.Log("AddRequest");
        Request newRequest = Instantiate(requestPrefab, Vector3.zero, Quaternion.identity);
        skyway.AddRequest(newRequest);
    }

    public void DeleteRequest(Request request)
    {
        Debug.Log("DeleteRequest");
        skyway.RemoveRequest(request);
    }

    public void CreatePayload(Request request)
    {
        Debug.Log("CreatePayload");
        Payload newPayload = Instantiate(payloadPrefab, Vector3.zero, Quaternion.identity);
        skyway.AddPayload(newPayload, request);
    }

    public void DeletePayload(Payload payload, Request request)
    {
        Debug.Log("DeletePayload");
        skyway.RemovePayload(payload, request);
    }

    public void SaveSkyway()
    {
        dataManager.SaveSkywayToJson(skyway);
    }

    public void LoadSkyway()
    {
        String skywayJson = dataManager.FetchSkywayJsonFromFile();
        Debug.Log("skyway json get successful: " + skywayJson);
        if (skywayJson == "")
        {
            return;
        }
        // clear original skyway
        skyway.ClearSkyway();
        skyway = dataManager.LoadSkywayFromJson(skywayJson);
        skyway.InitSkyway();
    }

    public void ResetSkyway()
    {
        return;
    }

    public string GetTimeString()
    {
        int hours = Mathf.FloorToInt(elapsedTime / 3600f);
        int hourInSec = hours * 3600;
        int minutes = Mathf.FloorToInt((elapsedTime - hourInSec) / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime - hourInSec - minutes * 60);
        string timeString = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        return timeString;
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
