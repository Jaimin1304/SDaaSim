using System;
using System.Collections;
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

    public List<Node> getNodes()
    {
        return this.nodes;
    }

    public List<Edge> getEdges()
    {
        return this.edges;
    }

    public List<Request> getRequests()
    {
        return this.requests;
    }

    public List<Swarm> getSwarms()
    {
        return this.swarms;
    }

    public Boolean setNodes(List<Node> nodes)
    {
        this.nodes = nodes;
        return true;
    }

    public Boolean setEdges(List<Edge> edges)
    {
        this.edges = edges;
        return true;
    }

    public Boolean setRequests(List<Request> requests)
    {
        this.requests = requests;
        return true;
    }

    public Boolean setSwarms(List<Swarm> swarms)
    {
        this.swarms = swarms;
        return true;
    }

    public SerializableSkyway ToSerializableSkyway()
    {
        return new SerializableSkyway()
        {
            nodes = nodes.Select(node => node.ToSerializableNode()).ToList(),
            requests = requests.Select(request => request.ToSerializableRequest()).ToList(),
            edges = edges.Select(edge => edge.ToSerializableEdge()).ToList(),
            swarms = swarms.Select(swarm => swarm.ToSerializableSwarm()).ToList(),
        };
    }
}
