using System;
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

    [SerializeField]
    Drone drone;

    public string Id
    {
        get { return id; }
        set { id = value; }
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

    public Drone Drone
    {
        get { return drone; }
        set { drone = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    public SerializablePayload ToSerializablePayload()
    {
        return new SerializablePayload()
        {
            id = id,
            weight = weight,
            request = request.Id,
            drone = drone.Id
        };
    }
}
