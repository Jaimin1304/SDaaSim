using System;
using System.Collections;
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
    List<Payload> payloads = new List<Payload>();

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    public string GetId()
    {
        return id;
    }

    public Node GetStartNode()
    {
        return startNode;
    }

    public Node GetDestNode()
    {
        return destNode;
    }

    public List<Payload> GetPayloads()
    {
        return payloads;
    }

    public SerializableRequest ToSerializableRequest()
    {
        return new SerializableRequest()
        {
            id = id,
            startNode = startNode.GetId(),
            destNode = destNode.GetId(),
            payloads = payloads.Select(payload => payload.ToSerializablePayload()).ToList(),
        };
    }
}
