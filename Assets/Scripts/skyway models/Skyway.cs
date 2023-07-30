using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class Skyway : MonoBehaviour
{
    [SerializeField]
    List<Node> nodes = new List<Node>();

    [SerializeField]
    List<Edge> edges = new List<Edge>();

    [SerializeField]
    List<Request> requests = new List<Request>();

    [SerializeField]
    List<Swarm> swarms = new List<Swarm>();

    List<SubSwarm> subSwarms = new List<SubSwarm>();
    List<Pad> pads = new List<Pad>();
    List<Drone> drones = new List<Drone>();
    List<Payload> payloads = new List<Payload>();

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

    void Awake()
    {
        // init subswarms
        foreach (Swarm swarm in swarms)
        {
            foreach (SubSwarm subSwarm in swarm.SubSwarms)
            {
                subSwarms.Add(subSwarm);
            }
        }
        // init pads
        foreach (Node node in nodes)
        {
            foreach (Pad pad in node.Pads)
            {
                pads.Add(pad);
            }
        }
        // init drones
        foreach (SubSwarm subSwarm in subSwarms)
        {
            foreach (Drone drone in subSwarm.Drones)
            {
                drones.Add(drone);
            }
        }
        // init payloads
        foreach (Request request in Requests)
        {
            foreach (Payload payload in request.Payloads)
            {
                payloads.Add(payload);
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

            subSwarms = subSwarms.Select(subSwarm => subSwarm.ToSerializableSubSwarm()).ToList(),
            pads = pads.Select(pad => pad.ToSerializablePad()).ToList(),
            drones = drones.Select(drone => drone.ToSerializableDrone()).ToList(),
            payloads = payloads.Select(payload => payload.ToSerializablePayload()).ToList(),
        };
    }
}
