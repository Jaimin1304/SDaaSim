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
    List<Pad> pads;

    [SerializeField]
    Utils utils;

    [SerializeField]
    NodeView nodeView;

    [SerializeField]
    private GameObject padPrefab;

    int totalCapacity;
    int rechargeableCapacity;
    List<Pad> rechargeablePads;
    List<Pad> nonRechargeablePads;

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

    public List<Pad> Pads
    {
        get { return pads; }
        set { pads = value; }
    }

    public List<Pad> RechargeablePads
    {
        get { return rechargeablePads; }
        set { rechargeablePads = value; }
    }

    public List<Pad> NonRechargeablePads
    {
        get { return nonRechargeablePads; }
        set { nonRechargeablePads = value; }
    }

    public int TotalCapacity
    {
        get { return totalCapacity; }
        set { totalCapacity = value; }
    }

    public int RechargeableCapacity
    {
        get { return rechargeableCapacity; }
        set { rechargeableCapacity = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        nodeView.initVisual(this);
        nodeView.UpdateVisual(this);
        GenerateRandomPads();
        SyncPadGroups();
        nodeView.ArrangePads(this);
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

    void SyncCapacities()
    {
        totalCapacity = pads.Count;
        rechargeableCapacity = pads.Count(pad => pad.Rechargeable);
    }

    void SyncPadGroups()
    {
        rechargeablePads = pads.Where(pad => pad.Rechargeable).ToList();
        nonRechargeablePads = pads.Where(pad => !pad.Rechargeable).ToList();
    }

    public void GenerateRandomPads()
    {
        // Generate a random number for the count of pads.
        int numberOfPads = UnityEngine.Random.Range(1, 9);
        for (int i = 0; i < numberOfPads; i++)
        {
            // Instantiate a new pad
            GameObject padGO = Instantiate(padPrefab, transform);
            // Reset local position to ensure it's placed at the position of the Node
            padGO.transform.localPosition = Vector3.zero;
            Pad newPad = padGO.GetComponent<Pad>();
            bool randomRechargeableState = UnityEngine.Random.value > 0.5f;
            newPad.ChangeRechargeableState(randomRechargeableState);
            newPad.Node = this;
            // Add the new pad to the pads list
            pads.Add(newPad);
        }
        SyncCapacities();
    }

    public SerializableNode ToSerializableNode()
    {
        return new SerializableNode()
        {
            id = id,
            position = transform.position,
            landedDrones = landedDrones.Select(drone => drone.Id).ToList(),
            edges = edges.Select(edge => edge.Id).ToList(),
            pads = pads.Select(pad => pad.Id).ToList(),
        };
    }
}
