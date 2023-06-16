using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Drone : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    SubSwarm subSwarm;

    [SerializeField]
    float selfWeight;

    [SerializeField]
    float speed;

    [SerializeField]
    float maxPayloadWeight;

    [SerializeField]
    float batteryStatus;

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

    public SubSwarm GetSubSwarm()
    {
        return subSwarm;
    }

    public float GetSelfWeight()
    {
        return selfWeight;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetMaxPayloadWeight()
    {
        return maxPayloadWeight;
    }

    public float GetBatteryStatus()
    {
        return batteryStatus;
    }

    public List<Payload> GetPayloads()
    {
        return payloads;
    }

    public SerializableDrone ToSerializableDrone()
    {
        return new SerializableDrone()
        {
            id = id,
            selfWeight = selfWeight,
            speed = speed,
            maxPayloadWeight = maxPayloadWeight,
            batteryStatus = batteryStatus,
            payloads = payloads.Select(payload => payload.GetId()).ToList(),
        };
    }
}
