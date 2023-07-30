using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Request : MonoBehaviour
{
    [SerializeField]
    private string id;

    [SerializeField]
    private Node startNode;

    [SerializeField]
    private Node destNode;

    [SerializeField]
    private List<Payload> payloads = new List<Payload>();

    public string Id
    {
        get { return id; }
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
            startNode = startNode.Id,
            destNode = destNode.Id,
            payloads = payloads.Select(payload => payload.Id).ToList(),
        };
    }
}
