using System;
using System.Collections.Generic;
using UnityEngine;

public class Payload : MonoBehaviour
{
    [SerializeField]
    private string id;

    [SerializeField]
    private float weight;

    [SerializeField]
    private Request request;

    public string Id
    {
        get { return id; }
    }

    public float Weight
    {
        get { return weight; }
        set { weight = value; }
    }

    public Request Request
    {
        get { return request; }
        set { request = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    public SerializablePayload ToSerializablePayload()
    {
        return new SerializablePayload() { id = this.id, weight = this.weight, };
    }
}
