using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Swarm : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    Request request;

    [SerializeField]
    List<SubSwarm> subSwarms = new();

    public string Id
    {
        get { return id; }
        set { id = value; }
    }

    public Request Request
    {
        get { return request; }
        set { request = value; }
    }

    public List<SubSwarm> SubSwarms
    {
        get { return subSwarms; }
        set { subSwarms = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    public void TransferDrone(SubSwarm originalSubSwarm, SubSwarm targetSubSwarm, Drone drone)
    {
        originalSubSwarm.RemoveDrone(drone);
        targetSubSwarm.AddDrone(drone);
    }

    // Split the given SubSwarm into two new SubSwarms
    public void SplitSubSwarm(SubSwarm originalSubSwarm, List<Drone> dronesToSplit, Edge edgeToGo)
    {
        if (originalSubSwarm == null || dronesToSplit == null || edgeToGo == null)
        {
            Debug.LogError("Invalid input for SplitSubSwarm");
            return;
        }
        // create a new SubSwarm
        SubSwarm newSubSwarm = Instantiate(Simulator.instance.SubSwarmPrefab);
        newSubSwarm.Edge = edgeToGo;
        foreach (Drone drone in dronesToSplit)
        {
            // remove drone from original SubSwarm and add it to new SubSwarm
            TransferDrone(originalSubSwarm, newSubSwarm, drone);
        }
        // add the new SubSwarm into Swarm
        SubSwarms.Add(newSubSwarm);
    }

    // Merge all SubSwarms at the same node into a new SubSwarm
    public void MergeAllSubSwarmsAtNode(Node node, Edge edgeToGo)
    {
        if (node == null || edgeToGo == null)
        {
            Debug.LogError("Invalid input for MergeAllSubSwarmsAtNode");
            return;
        }
        SubSwarm mergedSubSwarm = Instantiate(Simulator.instance.SubSwarmPrefab);
        mergedSubSwarm.Edge = edgeToGo;
        foreach (SubSwarm subSwarm in SubSwarms)
        {
            if (subSwarm.Node == node && subSwarm.Edge == null)
            {
                foreach (Drone drone in subSwarm.Drones)
                {
                    TransferDrone(subSwarm, mergedSubSwarm, drone);
                }
                // could destory original subSwarm
                Destroy(subSwarm.gameObject);
            }
        }
        // add new merged subSwarm
        SubSwarms.Add(mergedSubSwarm);
    }

    // Merge one subswaem into another
    public void MergeTwoSubSwarms(SubSwarm subSwarmA, SubSwarm subSwarmB, Edge edgeToGo)
    {
        if (subSwarmA.Node != subSwarmB.Node)
        {
            Debug.LogError("Two subswarms not at same position");
            return;
        }
        subSwarmA.Edge = edgeToGo;
        foreach (Drone drone in subSwarmB.Drones)
        {
            TransferDrone(subSwarmA, subSwarmB, drone);
        }
        Destroy(subSwarmB.gameObject);
    }

    public SerializableSwarm ToSerializableSwarm()
    {
        return new SerializableSwarm()
        {
            id = id,
            name = gameObject.name,
            request = request.Id,
            subSwarms = subSwarms.Select(subSwarm => subSwarm.Id).ToList(),
        };
    }
}
