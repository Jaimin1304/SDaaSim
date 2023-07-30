using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node : MonoBehaviour
{
    [SerializeField]
    private string id;

    [SerializeField]
    private List<Pad> pads;

    [SerializeField]
    private List<Drone> drones;

    [SerializeField]
    private List<Edge> edges;

    [SerializeField]
    private Outline outline;

    [SerializeField]
    private Utils utils;

    public string Id
    {
        get { return id; }
    }

    public List<Pad> Pads
    {
        get { return pads; }
        set { pads = value; }
    }

    public List<Drone> Drones
    {
        get { return drones; }
        set { drones = value; }
    }

    public List<Edge> Edges
    {
        get { return edges; }
        set { edges = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start() { }

    void Update()
    {
        utils.Outline(outline, this.gameObject);
    }

    public SerializableNode ToSerializableNode()
    {
        return new SerializableNode()
        {
            id = id,
            position = transform.position,
            pads = pads.Select(pad => pad.Id).ToList(),
            drones = drones.Select(drone => drone.Id).ToList(),
            edges = edges.Select(edge => edge.Id).ToList(),
        };
    }
}
