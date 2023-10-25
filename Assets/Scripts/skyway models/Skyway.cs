using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
