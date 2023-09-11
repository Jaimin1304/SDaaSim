using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    List<Drone> landedDrones;

    [SerializeField]
    List<Edge> edges;

    [SerializeField]
    Utils utils;

    [SerializeField]
    NodeView nodeView;

    [SerializeField]
    int capacity;

    public string Id
    {
        get { return id; }
    }

    public List<Drone> LandedDrones
    {
        get { return landedDrones; }
        set { landedDrones = value; }
    }

    public List<Edge> Edges
    {
        get { return edges; }
        set { edges = value; }
    }

    public int Capacity
    {
        get { return capacity; }
        set { capacity = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        nodeView.initVisual(this);
        nodeView.UpdateVisual(this);
    }

    void Update()
    {
        nodeView.UpdateVisual(this);
        if (Simulator.instance.CurrentState == Simulator.State.Play)
        {
            RechargeDrones();
        }
    }

    public bool HasEdgeTo(Node otherNode)
    {
        return edges.Any(edge => edge.LeftNode == otherNode || edge.RightNode == otherNode);
    }

    void RechargeDrones()
    {
        if (landedDrones.Count <= 0)
        {
            return;
        }
        foreach (Drone drone in landedDrones)
        {
            drone.Recharge(0.01f);
        }
    }

    public SerializableNode ToSerializableNode()
    {
        return new SerializableNode()
        {
            id = id,
            position = transform.position,
            landedDrones = landedDrones.Select(drone => drone.Id).ToList(),
            edges = edges.Select(edge => edge.Id).ToList(),
            capacity = capacity
        };
    }
}
