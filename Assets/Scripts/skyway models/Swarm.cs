using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Swarm : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    Request request;

    //[SerializeField] List<Drone> drones = new List<Drone>();
    [SerializeField]
    List<SubSwarm> subSwarms = new List<SubSwarm>();

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    public string GetId() {
        return id;
    }

    public Request GetRequest() {
        return request;
    }

    public List<SubSwarm> GetSubSwarms() {
        return subSwarms;
    }

    public SerializableSwarm ToSerializableSwarm()
    {
        return new SerializableSwarm()
        {
            id = id,
            request = request.GetId(),
            subSwarms = subSwarms.Select(subSwarm => subSwarm.ToSerializableSubSwarm()).ToList(),
        };
    }
}
