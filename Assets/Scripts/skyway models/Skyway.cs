using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skyway : MonoBehaviour
{
    [SerializeField] private List<Node> nodes = new List<Node>();
    [SerializeField] private List<Edge> edges = new List<Edge>();
    [SerializeField] private List<Request> requests = new List<Request>();
    [SerializeField] private List<Swarm> swarms = new List<Swarm>();

    public List<Node> getNodes() {
        return this.nodes;
    }

    public List<Edge> getEdges() {
        return this.edges;
    }

    public List<Request> getRequests() {
        return this.requests;
    }

    public List<Swarm> getSwarms() {
        return this.swarms;
    }
}
