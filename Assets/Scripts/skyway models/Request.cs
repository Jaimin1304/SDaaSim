using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Request : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    Node startNode;

    [SerializeField]
    Node destNode;

    [SerializeField]
    Swarm swarm;

    [SerializeField]
    List<Payload> payloads = new();

    public string Id
    {
        get { return id; }
        set { id = value; }
    }

    public Node StartNode
    {
        get { return startNode; }
        set { startNode = value; }
    }

    public Node DestNode
    {
        get { return destNode; }
        set { destNode = value; }
    }

    public Swarm Swarm
    {
        get { return swarm; }
        set { swarm = value; }
    }

    public List<Payload> Payloads
    {
        get { return payloads; }
        set { payloads = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    public SerializableRequest ToSerializableRequest()
    {
        return new SerializableRequest()
        {
            id = id,
            name = gameObject.name,
            startNode = startNode.Id,
            destNode = destNode.Id,
            swarm = swarm.Id,
            payloads = payloads.Select(payload => payload.Id).ToList(),
        };
    }
}
