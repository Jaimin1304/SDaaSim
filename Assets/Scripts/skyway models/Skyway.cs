using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class Skyway : MonoBehaviour
{
    [SerializeField]
    List<Node> nodes = new();

    [SerializeField]
    List<Edge> edges = new();

    [SerializeField]
    List<Request> requests = new();

    [SerializeField]
    List<Swarm> swarms = new();

    Dictionary<string, Edge> edgeDict = new();
    Dictionary<string, SubSwarm> subSwarms = new();
    Dictionary<string, Pad> pads = new();
    Dictionary<string, Drone> drones = new();
    Dictionary<string, Payload> payloads = new();

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

    public List<Request> Requests
    {
        get { return requests; }
        set { requests = value; }
    }

    public List<Swarm> Swarms
    {
        get { return swarms; }
        set { swarms = value; }
    }

    public Dictionary<string, SubSwarm> SubSwarms
    {
        get { return subSwarms; }
        set { subSwarms = value; }
    }

    public Dictionary<string, Pad> Pads
    {
        get { return pads; }
        set { pads = value; }
    }

    public Dictionary<string, Drone> Drones
    {
        get { return drones; }
        set { drones = value; }
    }

    public Dictionary<string, Payload> Payloads
    {
        get { return payloads; }
        set { payloads = value; }
    }

    public Dictionary<string, Edge> EdgeDict
    {
        get { return edgeDict; }
        set { edgeDict = value; }
    }

    public void InitSkyway()
    {
        // init edgeDict
        foreach (Edge edge in edges)
        {
            edgeDict.Add(edge.Id, edge);
        }
        // init subswarms
        foreach (Swarm swarm in swarms)
        {
            foreach (SubSwarm subSwarm in swarm.SubSwarms)
            {
                subSwarms.Add(subSwarm.Id, subSwarm);
            }
        }
        // init pads
        foreach (Node node in nodes)
        {
            foreach (Pad pad in node.Pads)
            {
                pads.Add(pad.Id, pad);
            }
        }
        // init drones
        foreach (SubSwarm subSwarm in subSwarms.Values)
        {
            foreach (Drone drone in subSwarm.Drones)
            {
                drones.Add(drone.Id, drone);
            }
        }
        // init payloads
        foreach (Request request in Requests)
        {
            foreach (Payload payload in request.Payloads)
            {
                payloads.Add(payload.Id, payload);
            }
        }
    }

    public SerializableSkyway ToSerializableSkyway()
    {
        return new SerializableSkyway()
        {
            nodes = nodes.Select(node => node.ToSerializableNode()).ToList(),
            requests = requests.Select(request => request.ToSerializableRequest()).ToList(),
            edges = edges.Select(edge => edge.ToSerializableEdge()).ToList(),
            swarms = swarms.Select(swarm => swarm.ToSerializableSwarm()).ToList(),
            subSwarms = subSwarms
                .Select(subSwarm => subSwarm.Value.ToSerializableSubSwarm())
                .ToList(),
            pads = pads.Select(pad => pad.Value.ToSerializablePad()).ToList(),
            drones = drones.Select(drone => drone.Value.ToSerializableDrone()).ToList(),
            payloads = payloads.Select(payload => payload.Value.ToSerializablePayload()).ToList(),
        };
    }
}
