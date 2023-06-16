using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Payload : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    float weight;

    [SerializeField]
    Request request;

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    public string GetId()
    {
        return id;
    }

    public float GetWeight()
    {
        return weight;
    }

    public Request GetRequest()
    {
        return request;
    }

    public SerializablePayload ToSerializablePayload()
    {
        return new SerializablePayload() { id = this.id, weight = this.weight, };
    }
}
