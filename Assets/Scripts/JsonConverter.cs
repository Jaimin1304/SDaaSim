using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;

[Serializable]
public class SerializableSkyway
{
    public List<SerializableNode> nodes;
    public List<SerializableEdge> edges;
    public List<SerializableWayPoint> wayPoints;
    public List<SerializablePad> pads;
    public List<SerializableRequest> requests;
    public List<SerializableSwarm> swarms;
    public List<SerializableSubSwarm> subSwarms;
    public List<SerializableDrone> drones;
    public List<SerializablePayload> payloads;
}

[Serializable]
public class SerializableNode
{
    public string id;
    public Vector3 position;
    public List<string> drones;
    public List<string> edges;
    public List<string> pads;
}

[Serializable]
public class SerializableEdge
{
    public string id;
    public Vector3 position;
    public string leftNode;
    public string rightNode;
    public float totalLength;
    public List<string> wayPoints;
}

[Serializable]
public class SerializableWayPoint
{
    public string id;
    public Vector3 position;
    public string edge;
}

[Serializable]
public class SerializablePad
{
    public string id;
    public string node;
    public bool rechargeable;
}

[Serializable]
public class SerializableRequest
{
    public string id;
    public string startNode;
    public string destNode;
    public List<String> payloads;
}

[Serializable]
public class SerializablePayload
{
    public string id;
    public float weight;
    public string request;
    public string drone;
}

[Serializable]
public class SerializableSwarm
{
    public string id;
    public string request;
    public List<String> subSwarms;
}

[Serializable]
public class SerializableSubSwarm
{
    public string id;
    public string parentSwarm;
    public Vector3 position;
    public List<String> drones;
    public string node;
    public string edge;
    public int wayPointIndex;
    public String currentState;
}

[Serializable]
public class SerializableDrone
{
    public string id;
    public string subswarm;
    public float selfWeight;
    public float speed;
    public float maxPayloadWeight;
    public float batteryStatus;
    public List<string> payloads;
}

[Serializable]
public class SerializableDrones
{
    public List<SerializableDrone> drones;

    public SerializableDrones(List<SerializableDrone> drones)
    {
        this.drones = drones;
    }
}

public class JsonConverter : MonoBehaviour
{
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

    public string RecordCurrentStateToJson(Skyway skyway)
    {
        SerializableSkyway currState = skyway.ToSerializableSkyway();
        string skywayJson = JsonUtility.ToJson(currState);
        return skywayJson;
    }

    public string SwarmToJson(Swarm swarm)
    {
        SerializableSwarm currSwarm = swarm.ToSerializableSwarm();
        string swarmJson = JsonUtility.ToJson(currSwarm);
        return swarmJson;
    }

    public string SubSwarmToJson(SubSwarm subSwarm)
    {
        SerializableSubSwarm currSubSwarm = subSwarm.ToSerializableSubSwarm();
        string subSwarmJson = JsonUtility.ToJson(currSubSwarm);
        return subSwarmJson;
    }

    public string DronesToJson(SubSwarm subSwarm)
    {
        List<SerializableDrone> serializableDrones = subSwarm.Drones
            .Select(drone => drone.ToSerializableDrone())
            .ToList();
        SerializableDrones drones = new(serializableDrones);
        string dronesJson = JsonUtility.ToJson(drones);
        Debug.Log(dronesJson);
        return dronesJson;
    }

    public Skyway JsonToSkyway(String skywayJson)
    {
        SerializableSkyway serializableSkyway = JsonUtility.FromJson<SerializableSkyway>(
            skywayJson
        );
        Skyway skyway = Instantiate(skywayPrefab, Vector3.zero, Quaternion.identity);
        InitSkywayObjects(skyway, serializableSkyway);
        EstablishReferences(skyway, serializableSkyway);
        return skyway;
    }

    public void InitSkywayObjects(Skyway skyway, SerializableSkyway serializableSkyway)
    {
        foreach (SerializableNode i in serializableSkyway.nodes)
        {
            Node node = Instantiate(nodePrefab, i.position, Quaternion.identity);
            node.Id = i.id;
            skyway.Nodes.Add(node);
            node.transform.SetParent(skyway.transform);
        }

        foreach (SerializableEdge i in serializableSkyway.edges)
        {
            Edge edge = Instantiate(edgePrefab, i.position, Quaternion.identity);
            edge.Id = i.id;
            skyway.Edges.Add(edge);
            edge.transform.SetParent(skyway.transform);
        }

        foreach (SerializableWayPoint i in serializableSkyway.wayPoints)
        {
            WayPoint wayPoint = Instantiate(wayPointPrefab, i.position, Quaternion.identity);
            wayPoint.Id = i.id;
            skyway.WayPoints.Add(wayPoint);
            wayPoint.transform.SetParent(skyway.transform);
        }

        foreach (SerializablePad i in serializableSkyway.pads)
        {
            Pad pad = Instantiate(padPrefab, Vector3.zero, Quaternion.identity);
            pad.Id = i.id;
            pad.Rechargeable = i.rechargeable;
            skyway.Pads.Add(pad);
            pad.transform.SetParent(skyway.transform);
        }

        foreach (SerializableRequest i in serializableSkyway.requests)
        {
            Request request = Instantiate(requestPrefab, Vector3.zero, Quaternion.identity);
            request.Id = i.id;
            skyway.Requests.Add(request);
            request.transform.SetParent(skyway.transform);
        }

        foreach (SerializablePayload i in serializableSkyway.payloads)
        {
            Payload payload = Instantiate(payloadPrefab, Vector3.zero, Quaternion.identity);
            payload.Id = i.id;
            payload.Weight = i.weight;
            skyway.Payloads.Add(payload);
            payload.transform.SetParent(skyway.transform);
        }

        foreach (SerializableSwarm i in serializableSkyway.swarms)
        {
            Swarm swarm = Instantiate(swarmPrefab, Vector3.zero, Quaternion.identity);
            swarm.Id = i.id;
            skyway.Swarms.Add(swarm);
            swarm.transform.SetParent(skyway.transform);
        }

        foreach (SerializableSubSwarm i in serializableSkyway.subSwarms)
        {
            SubSwarm subSwarm = Instantiate(subSwarmPrefab, Vector3.zero, Quaternion.identity);
            subSwarm.Id = i.id;
            skyway.SubSwarms.Add(subSwarm);
            subSwarm.transform.SetParent(skyway.transform);
        }

        foreach (SerializableDrone i in serializableSkyway.drones)
        {
            Drone drone = Instantiate(dronePrefab, Vector3.zero, Quaternion.identity);
            drone.Id = i.id;
            skyway.Drones.Add(drone);
            drone.transform.SetParent(skyway.transform);
        }
    }

    void EstablishReferences(Skyway skyway, SerializableSkyway serializableSkyway)
    {
        // Establish references for waypoints
        foreach (WayPoint wayPoint in skyway.WayPoints)
        {
            SerializableWayPoint w = serializableSkyway.wayPoints.Find(w => w.id == wayPoint.Id);
            Edge edge = skyway.Edges.Find(edge => edge.Id == w.edge);
            wayPoint.Edge = edge;
            // set as child
            wayPoint.transform.SetParent(edge.transform);
        }

        // Establish references for edges
        foreach (Edge edge in skyway.Edges)
        {
            SerializableEdge e = serializableSkyway.edges.Find(e => e.id == edge.Id);
            // left node and right node
            edge.LeftNode = skyway.Nodes.Find(n => n.Id == e.leftNode);
            edge.RightNode = skyway.Nodes.Find(n => n.Id == e.rightNode);
            // wayPoints
            foreach (String wayPointID in e.wayPoints)
            {
                WayPoint wayPoint = skyway.WayPoints.Find(w => w.Id == wayPointID);
                edge.WayPoints.Add(wayPoint);
            }
            edge.Init();
        }

        // Establish references for pads
        foreach (Pad pad in skyway.Pads)
        {
            SerializablePad p = serializableSkyway.pads.Find(p => p.id == pad.Id);
            Node node = skyway.Nodes.Find(node => node.Id == p.node);
            pad.Node = node;
            // Establish references for pad list in nodes
            node.Pads.Add(pad);
            // set as child
            pad.transform.SetParent(node.transform);
        }

        // Establish references for payloads
        foreach (Payload payload in skyway.Payloads)
        {
            SerializablePayload p = serializableSkyway.payloads.Find(i => i.id == payload.Id);
            Request request = skyway.Requests.Find(request => request.Id == p.request);
            payload.Request = request;
            Drone drone = skyway.Drones.Find(drone => drone.Id == p.drone);
            payload.Drone = drone;
            // Establish references for payload list in requests
            request.Payloads.Add(payload);
            // set as child
            payload.transform.SetParent(drone.transform);
        }

        // Establish references for requests
        foreach (Request request in skyway.Requests)
        {
            SerializableRequest r = serializableSkyway.requests.Find(i => i.id == request.Id);
            request.StartNode = skyway.Nodes.Find(node => node.Id == r.startNode);
            request.DestNode = skyway.Nodes.Find(node => node.Id == r.destNode);
        }

        // Establish references for drones
        foreach (Drone drone in skyway.Drones)
        {
            SerializableDrone d = serializableSkyway.drones.Find(i => i.id == drone.Id);
            // subswarm
            drone.SubSwarm = skyway.SubSwarms.Find(subSwarm => subSwarm.Id == d.subswarm);
            // payloads
            foreach (String payloadID in d.payloads)
            {
                Payload payload = skyway.Payloads.Find(payload => payload.Id == payloadID);
                drone.Payloads.Add(payload);
            }
            // set as child
            drone.transform.SetParent(drone.SubSwarm.transform);
        }

        // Establish references for nodes
        foreach (Node node in skyway.Nodes)
        {
            SerializableNode n = serializableSkyway.nodes.Find(i => i.id == node.Id);
            // edges
            foreach (String edgeID in n.edges)
            {
                Edge edge = skyway.Edges.Find(i => i.Id == edgeID);
                node.Edges.Add(edge);
            }
            node.Init();
        }

        // Establish references for swarms
        foreach (Swarm swarm in skyway.Swarms)
        {
            SerializableSwarm s = serializableSkyway.swarms.Find(i => i.id == swarm.Id);
            // request
            swarm.Request = skyway.Requests.Find(request => request.Id == s.request);
        }

        // Establish references for subSwarms
        foreach (SubSwarm subSwarm in skyway.SubSwarms)
        {
            SerializableSubSwarm s = serializableSkyway.subSwarms.Find(i => i.id == subSwarm.Id);
            // parent swarm
            subSwarm.ParentSwarm = skyway.Swarms.Find(i => i.Id == s.parentSwarm);
            // Establish references for subSwarm list in swarms
            subSwarm.ParentSwarm.SubSwarms.Add(subSwarm);
            // drones
            foreach (String droneID in s.drones)
            {
                Drone drone = skyway.Drones.Find(drone => drone.Id == droneID);
                subSwarm.Drones.Add(drone);
            }
            // node
            subSwarm.Node = subSwarm.ParentSwarm.Request.StartNode;
            subSwarm.Init();
        }
    }
}
