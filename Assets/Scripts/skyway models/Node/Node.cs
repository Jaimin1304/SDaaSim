using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.XR;

public class Node : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    List<Drone> drones;

    [SerializeField]
    List<Edge> edges;

    [SerializeField]
    List<Pad> pads;

    [SerializeField]
    Utils utils;

    [SerializeField]
    NodeView nodeView;

    [SerializeField]
    GameObject padPrefab;

    List<Pad> rechargePads;
    List<Pad> nonRechargePads;

    public string Id
    {
        get { return id; }
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

    public List<Pad> Pads
    {
        get { return pads; }
        set { pads = value; }
    }

    public List<Pad> RechargePads
    {
        get { return rechargePads; }
        set { rechargePads = value; }
    }

    public List<Pad> NonRechargePads
    {
        get { return nonRechargePads; }
        set { nonRechargePads = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        nodeView.initVisual(this);
        nodeView.UpdateVisual(this);
        GenerateDefaultPads();
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
        if (drones.Count <= 0)
        {
            return;
        }
        foreach (Drone drone in drones)
        {
            drone.Recharge(0.01f);
        }
    }

    void SyncPadGroups()
    {
        rechargePads = pads.Where(pad => pad.Rechargeable).ToList();
        nonRechargePads = pads.Where(pad => !pad.Rechargeable).ToList();
    }

    public void GenerateDefaultPads()
    {
        CreatePads(Globals.RechargePadNum, true);
        CreatePads(Globals.NonRechargePadNum, false);
        SyncPadGroups();
    }

    private void CreatePads(int padCount, bool isRechargeable)
    {
        for (int i = 0; i < padCount; i++)
        {
            GameObject padGO = Instantiate(padPrefab, transform);
            padGO.transform.localPosition = Vector3.zero;
            Pad newPad = padGO.GetComponent<Pad>();
            newPad.ChangeRechargeableState(isRechargeable);
            newPad.Node = this;
            pads.Add(newPad);
        }
    }

    public bool AddPad(bool rechargeable)
    {
        // Add a new pad
        GameObject padGO = Instantiate(padPrefab, transform);
        padGO.transform.localPosition = Vector3.zero;
        Pad newPad = padGO.GetComponent<Pad>();
        // Set rechargeable
        newPad.ChangeRechargeableState(rechargeable);
        newPad.Node = this;
        // Add it to pad list according to rechargeable or not
        pads.Add(newPad);
        if (rechargeable)
        {
            rechargePads.Add(newPad);
        }
        else
        {
            nonRechargePads.Add(newPad);
        }
        SyncPadGroups();
        nodeView.ArrangePads(this);
        return true;
    }

    public bool RemovePad(bool rechargeable)
    {
        List<Pad> targetList = rechargeable ? rechargePads : nonRechargePads;
        if (targetList.Count <= 0)
            return false;
        // Remove pad from scene and list
        Destroy(targetList[0].gameObject);
        pads.Remove(targetList[0]);
        targetList.RemoveAt(0);
        SyncPadGroups();
        nodeView.ArrangePads(this);
        return true;
    }

    public List<Pad> FreeRechargePads()
    {
        return rechargePads.Where(pad => pad.Drone == null).ToList();
    }

    public List<Pad> FreeNonRechargePads()
    {
        return nonRechargePads.Where(pad => pad.Drone == null).ToList();
    }

    public SerializableNode ToSerializableNode()
    {
        return new SerializableNode()
        {
            id = id,
            position = transform.position,
            drones = drones.Select(drone => drone.Id).ToList(),
            edges = edges.Select(edge => edge.Id).ToList(),
            pads = pads.Select(pad => pad.Id).ToList(),
        };
    }
}
