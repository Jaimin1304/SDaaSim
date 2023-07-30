using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Drone : MonoBehaviour
{
    [SerializeField]
    private string id;

    [SerializeField]
    private SubSwarm subSwarm;

    [SerializeField]
    private float selfWeight;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float maxPayloadWeight;

    [SerializeField]
    private float batteryStatus;

    [SerializeField]
    private List<Payload> payloads = new List<Payload>();

    public string Id
    {
        get { return id; }
    }

    public SubSwarm SubSwarm
    {
        get { return subSwarm; }
        set { subSwarm = value; }
    }

    public float SelfWeight
    {
        get { return selfWeight; }
        set { selfWeight = value; }
    }

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public float MaxPayloadWeight
    {
        get { return maxPayloadWeight; }
        set { maxPayloadWeight = value; }
    }

    public float BatteryStatus
    {
        get { return batteryStatus; }
        set { batteryStatus = value; }
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

    void Start() { }

    void Update() { }

    public SerializableDrone ToSerializableDrone()
    {
        return new SerializableDrone()
        {
            id = id,
            selfWeight = selfWeight,
            speed = speed,
            maxPayloadWeight = maxPayloadWeight,
            batteryStatus = batteryStatus,
            payloads = payloads.Select(payload => payload.Id).ToList(),
        };
    }
}
