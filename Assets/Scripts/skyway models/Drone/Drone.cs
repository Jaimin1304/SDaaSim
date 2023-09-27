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
    List<Payload> payloads = new();

    [SerializeField]
    DroneView droneView;

    [SerializeField]
    float batteryStatus;

    [SerializeField]
    float currBatteryWh;

    [SerializeField]
    float batteryCapacityWh;

    List<DroneData> dataCollection = new List<DroneData>();

    float lastDataCollectionTime = 0f; // To store the time of the last data collection
    const float dataCollectionInterval = 5f; // The interval in seconds between data collections

    public List<DroneData> DataCollection
    {
        get { return dataCollection; }
    }

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

    public float CurrBatteryWh
    {
        get { return currBatteryWh; }
        set { currBatteryWh = value; }
    }

    public float BatteryCapacityWh
    {
        get { return batteryCapacityWh; }
        set { batteryCapacityWh = value; }
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
        batteryCapacityWh = Globals.DroneBatCap;
        Debug.Log(batteryCapacityWh);
    }

    void Update()
    {
        if (Simulator.instance.CurrentState != Simulator.State.Play)
        {
            return;
        }
        droneView.UpdateVisual(this);

        // Check if the time elapsed since the last data collection is greater than the interval
        if (Simulator.instance.ElapsedTime - lastDataCollectionTime >= dataCollectionInterval)
        {
            CollectData();
            lastDataCollectionTime = Simulator.instance.ElapsedTime; // Update the last collection time
        }

        if (subSwarm.CurrentState != SubSwarm.State.Recharging)
        {
            batteryStatus -= 0.03f * Time.deltaTime * Globals.PlaySpeed;
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

    void CollectData()
    {
        DroneData data = new DroneData
        {
            timestring = Simulator.instance.GetTimeString(),
            position = transform.position,
            speed = speed,
            batteryStatus = batteryStatus
        };
        dataCollection.Add(data);
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

public class DroneData
{
    public string timestring;
    public Vector3 position;
    public float speed;
    public float batteryStatus;

    public string ToCSV()
    {
        return $"{timestring},{position.x},{position.y},{position.z},{speed},{batteryStatus}";
    }
}
