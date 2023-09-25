using System;
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
    float currBatteryJoule;

    [SerializeField]
    float batteryCapacityJoule;

    [SerializeField]
    List<Payload> payloads = new();

    [SerializeField]
    DroneView droneView;

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

    public float CurrBatteryJoule
    {
        get { return currBatteryJoule; }
        set { currBatteryJoule = value; }
    }

    public float BatteryCapacityJoule
    {
        get { return batteryCapacityJoule; }
        set { batteryCapacityJoule = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        droneView.initVisual(this);
        droneView.UpdateVisual(this);
        batteryStatus = 1;
        batteryCapacityJoule = Globals.droneBatteryJoule;
        Debug.Log(batteryCapacityJoule);
    }

    void Update()
    {
        droneView.UpdateVisual(this);
        if (
            Simulator.instance.CurrentState == Simulator.State.Play
            && subSwarm.CurrentState != SubSwarm.State.Landed
        )
        {
            batteryStatus -= 0.01f * Time.deltaTime * Globals.PlaySpeed;
            if (batteryStatus < 0)
            {
                Debug.LogError(string.Format("{0} is out of battery!", this.name));
            }
        }
    }

    public void Recharge(float amount)
    {
        if (batteryStatus >= 1)
        {
            batteryStatus = 1;
            return;
        }
        batteryStatus += amount * Time.deltaTime * Globals.PlaySpeed;
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
            payloads = payloads.Select(payload => payload.Id).ToList(),
        };
    }
}
