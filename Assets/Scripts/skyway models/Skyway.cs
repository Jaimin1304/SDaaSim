using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml.Serialization;

[Serializable]
public class Skyway : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    List<Node> nodes = new();

    [SerializeField]
    List<Edge> edges = new();

    [SerializeField]
    List<WayPoint> wayPoints = new();

    [SerializeField]
    List<Pad> pads = new();

    [SerializeField]
    List<Request> requests = new();

    [SerializeField]
    List<Payload> payloads = new();

    [SerializeField]
    List<Swarm> swarms = new();

    [SerializeField]
    List<SubSwarm> subSwarms = new();

    [SerializeField]
    List<Drone> drones = new();

    Dictionary<string, Edge> edgeDict = new();
    Dictionary<string, Pad> padDict = new();
    Dictionary<string, Node> nodeDict = new();
    Dictionary<string, SubSwarm> subSwarmDict = new();
    Dictionary<string, Drone> droneDict = new();
    Dictionary<string, Payload> payloadDict = new();

    public string Id
    {
        get { return id; }
        set { id = value; }
    }

    public List<Node> Nodes
    {
        get { return nodes; }
        set { nodes = value; }
    }

    public List<Edge> Edges
    {
        get { return edges; }
        set { edges = value; }
    }

    public List<WayPoint> WayPoints
    {
        get { return wayPoints; }
        set { wayPoints = value; }
    }

    public List<Pad> Pads
    {
        get { return pads; }
        set { pads = value; }
    }

    public List<Request> Requests
    {
        get { return requests; }
        set { requests = value; }
    }

    public List<Payload> Payloads
    {
        get { return payloads; }
        set { payloads = value; }
    }

    public List<Swarm> Swarms
    {
        get { return swarms; }
        set { swarms = value; }
    }

    public List<SubSwarm> SubSwarms
    {
        get { return subSwarms; }
        set { subSwarms = value; }
    }

    public List<Drone> Drones
    {
        get { return drones; }
        set { drones = value; }
    }

    public Dictionary<string, SubSwarm> SubSwarmDict
    {
        get { return subSwarmDict; }
        set { subSwarmDict = value; }
    }

    public Dictionary<string, Pad> PadDict
    {
        get { return padDict; }
        set { padDict = value; }
    }

    public Dictionary<string, Drone> DroneDict
    {
        get { return droneDict; }
        set { droneDict = value; }
    }

    public Dictionary<string, Payload> PayloadDict
    {
        get { return payloadDict; }
        set { payloadDict = value; }
    }

    public Dictionary<string, Edge> EdgeDict
    {
        get { return edgeDict; }
        set { edgeDict = value; }
    }

    public Dictionary<string, Node> NodeDict
    {
        get { return nodeDict; }
        set { nodeDict = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    public void InitSkyway()
    {
        // init edgeDict
        foreach (Edge edge in edges)
        {
            edgeDict.Add(edge.Id, edge);
        }
        // init nodeDict
        foreach (Node node in nodes)
        {
            nodeDict.Add(node.Id, node);
        }
        // init subSwarmDict
        foreach (Swarm swarm in swarms)
        {
            foreach (SubSwarm subSwarm in swarm.SubSwarms)
            {
                subSwarmDict.Add(subSwarm.Id, subSwarm);
            }
        }
        //// init pads
        //foreach (Node node in nodes)
        //{
        //    foreach (Pad pad in node.Pads)
        //    {
        //        pads.Add(pad.Id, pad);
        //    }
        //}
        // init droneDict
        foreach (SubSwarm subSwarm in subSwarmDict.Values)
        {
            foreach (Drone drone in subSwarm.Drones)
            {
                droneDict.Add(drone.Id, drone);
            }
        }
        // init payloadDict
        foreach (Request request in Requests)
        {
            foreach (Payload payload in request.Payloads)
            {
                payloadDict.Add(payload.Id, payload);
            }
        }
    }

    public bool AddNode(Node node)
    {
        node.gameObject.name = String.Format("Node ({0})", nodes.Count.ToString());
        nodes.Add(node);
        nodeDict.Add(node.Id, node);
        for (int i = 0; i < Globals.RechargePadNum; i++)
        {
            node.AddPad(true);
        }
        for (int i = 0; i < Globals.NonRechargePadNum; i++)
        {
            node.AddPad(false);
        }
        return true;
    }

    public bool AddEdge(Edge edge)
    {
        edge.gameObject.name = String.Format("Edge ({0})", edges.Count.ToString());
        edges.Add(edge);
        edgeDict.Add(edge.Id, edge);
        return true;
    }

    public bool RemoveNode(Node node)
    {
        // Remove the node from the nodes list
        if (!nodes.Remove(node))
        {
            Debug.LogWarning("Node not found in the nodes list");
            return false;
        }
        // Remove the node from the nodeDict dictionary
        if (!nodeDict.Remove(node.Id))
        {
            Debug.LogWarning("Node not found in the nodeDict dictionary");
            return false;
        }
        // Remove any edges connected to this node
        for (int i = node.Edges.Count - 1; i >= 0; i--)
        {
            RemoveEdge(node.Edges[i]);
        }
        // Remove all pads in the node
        node.ClearPads();
        Destroy(node.gameObject);
        return true;
    }

    public bool AddWayPoint(Edge edge, WayPoint newWayPoint, WayPoint baseWayPoint)
    {
        // Determine position for the new waypoint
        if (baseWayPoint)
        {
            newWayPoint.transform.position = baseWayPoint.transform.position + new Vector3(5, 0, 5);
            int index = edge.WayPoints.IndexOf(baseWayPoint);
            if (index != -1)
                edge.WayPoints.Insert(index + 1, newWayPoint);
        }
        else
        {
            newWayPoint.transform.position =
                (edge.LeftNode.transform.position + edge.RightNode.transform.position) / 2;
            edge.WayPoints.Add(newWayPoint);
        }
        // Set newWayPoint as a child of edge
        newWayPoint.transform.SetParent(edge.transform);
        // Synchronize the edge with the new waypoint
        edge.SyncEdge();
        wayPoints.Add(newWayPoint);
        return true;
    }

    public bool RemoveWayPoint(WayPoint wayPoint)
    {
        wayPoints.Remove(wayPoint);
        Edge edge = wayPoint.Edge;
        bool result = edge.RemoveWayPoint(wayPoint);
        return result;
    }

    public bool RemoveEdge(Edge edge)
    {
        // Remove the edge from the edges list
        if (!edges.Remove(edge))
        {
            Debug.LogWarning("Edge not found in the edges list");
            return false;
        }
        // Remove the edge from the edgeDict dictionary
        if (!edgeDict.Remove(edge.Id))
        {
            Debug.LogWarning("Edge not found in the edgeDict dictionary");
            return false;
        }
        // Remove this edge from the nodes it is connected to
        edge.LeftNode.Edges.Remove(edge);
        edge.RightNode.Edges.Remove(edge);
        // Remove waypoints in edge
        for (int i = edge.WayPoints.Count - 1; i >= 0; i--)
        {
            RemoveWayPoint(edge.WayPoints[i]);
        }
        Destroy(edge.gameObject);
        return true;
    }

    public bool AddRequest(Request request)
    {
        request.transform.SetParent(transform);
        requests.Add(request);
        // init request and swarm
        Swarm newSwarm = Instantiate(Simulator.instance.SwarmPrefab);
        SubSwarm subSwarm = Instantiate(Simulator.instance.SubSwarmPrefab);
        request.Swarm = newSwarm;
        newSwarm.SubSwarms.Add(subSwarm);
        newSwarm.Request = request;
        return true;
    }

    public bool RemoveRequest(Request request)
    {
        // remove drones and payloads
        for (int i = request.Payloads.Count - 1; i >= 0; i--)
        {
            RemovePayload(request.Payloads[i], request);
        }
        // remove subswarms and swarms
        for (int i = request.Swarm.SubSwarms.Count - 1; i >= 0; i--)
        {
            Destroy(request.Swarm.SubSwarms[i].gameObject);
        }
        Destroy(request.Swarm.gameObject);
        Destroy(request.gameObject);
        return true;
    }

    public bool AddPayload(Payload payload, Request request)
    {
        // skyway references
        payloads.Add(payload);
        payloadDict.Add(payload.Id, payload);
        request.Payloads.Add(payload);
        payload.Request = request;
        AddDroneForPayload(payload, request);
        return true;
    }

    public bool RemovePayload(Payload payload, Request request)
    {
        request.Payloads.Remove(payload);
        // remove payload from skyway
        payloads.Remove(payload);
        payloadDict.Remove(payload.Id);
        RemoveDroneWithPayload(payload, request);
        payload.Request = null;
        Destroy(payload.gameObject);
        return true;
    }

    void AddDroneForPayload(Payload payload, Request request)
    {
        Drone newDrone = Instantiate(
            Simulator.instance.DronePrefab,
            Vector3.zero,
            Quaternion.identity
        );
        // skyway references
        drones.Add(newDrone);
        droneDict.Add(newDrone.Id, newDrone);
        // payload references
        newDrone.Payloads.Add(payload);
        payload.Drone = newDrone;
        // request references
        SubSwarm subSwarm = request.Swarm.SubSwarms[0];
        newDrone.SubSwarm = subSwarm;
        subSwarm.Drones.Add(newDrone);
        subSwarm.Init();
        newDrone.transform.SetParent(subSwarm.transform);
        payload.transform.SetParent(newDrone.transform);
    }

    void RemoveDroneWithPayload(Payload payload, Request request)
    {
        Drone drone = payload.Drone;
        payload.Drone = null;
        drone.Payloads.Remove(payload);
        // remove skyway references
        drones.Remove(drone);
        droneDict.Remove(drone.Id);
        // remove drone from subswarm
        request.Swarm.SubSwarms[0].Drones.Remove(drone);
        // destroy drone
        drone.SubSwarm = null;
        drone.Payloads = null;
        Destroy(drone.gameObject);
    }

    public void ClearSkyway()
    {
        // destroy all skyway components
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            RemoveNode(nodes[i]);
        }
        // destroy all requests and corresponding swarms, subswarms and drones
        for (int i = requests.Count - 1; i >= 0; i--)
        {
            RemoveRequest(requests[i]);
        }
        // destroy self
        Destroy(gameObject);
    }

    public SerializableSkyway ToSerializableSkyway()
    {
        return new SerializableSkyway()
        {
            id = id,
            name = gameObject.name,
            nodes = nodes.Select(node => node.ToSerializableNode()).ToList(),
            edges = edges.Select(edge => edge.ToSerializableEdge()).ToList(),
            wayPoints = wayPoints.Select(wayPoint => wayPoint.ToSerializableWayPoint()).ToList(),
            pads = pads.Select(pad => pad.ToSerializablePad()).ToList(),
            requests = requests.Select(request => request.ToSerializableRequest()).ToList(),
            swarms = swarms.Select(swarm => swarm.ToSerializableSwarm()).ToList(),
            subSwarms = subSwarmDict
                .Select(subSwarm => subSwarm.Value.ToSerializableSubSwarm())
                .ToList(),
            drones = droneDict.Select(drone => drone.Value.ToSerializableDrone()).ToList(),
            payloads = payloadDict
                .Select(payload => payload.Value.ToSerializablePayload())
                .ToList(),
        };
    }
}
