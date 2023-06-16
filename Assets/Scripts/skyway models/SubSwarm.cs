using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SubSwarm : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    Swarm parentSwarm;

    [SerializeField]
    List<Drone> drones = new List<Drone>();

    [SerializeField]
    Node targetNode;

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    public string GetId()
    {
        return id;
    }

    public Swarm GetSwarm()
    {
        return parentSwarm;
    }

    public List<Drone> GetDrones()
    {
        return drones;
    }

    public Node GetTarget()
    {
        return targetNode;
    }

    public SerializableSubSwarm ToSerializableSubSwarm()
    {
        return new SerializableSubSwarm()
        {
            id = id,
            position = transform.position,
            drones = drones.Select(drone => drone.ToSerializableDrone()).ToList(),
            targetNode = targetNode.GetId(),
        };
    }
}
