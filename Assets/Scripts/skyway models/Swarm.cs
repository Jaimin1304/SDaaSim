using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Swarm : MonoBehaviour
{
    [SerializeField]
    private string id;

    [SerializeField]
    private Request request;

    [SerializeField]
    private List<SubSwarm> subSwarms = new List<SubSwarm>();

    public string Id
    {
        get { return id; }
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

    public SerializableSwarm ToSerializableSwarm()
    {
        return new SerializableSwarm()
        {
            id = id,
            request = request.Id,
            subSwarms = subSwarms.Select(subSwarm => subSwarm.Id).ToList(),
        };
    }
}
