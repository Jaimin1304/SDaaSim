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
    Pad pad;

    [SerializeField]
    float selfWeight;

    [SerializeField]
    float speed;

    [SerializeField]
    float maxLiftSpd;

    [SerializeField]
    float maxDescentSpd;

    [SerializeField]
    float maxHorizontalSpd;

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

    public Pad Pad
    {
        get { return pad; }
        set { pad = value; }
    }

    public float SelfWeight
    {
        get { return selfWeight; }
        set { selfWeight = value; }
    }

    public float MaxLiftSpd
    {
        get { return maxLiftSpd; }
        set { maxLiftSpd = value; }
    }

    public float MaxDescentSpd
    {
        get { return maxDescentSpd; }
        set { maxDescentSpd = value; }
    }

    public float MaxHorizontalSpd
    {
        get { return maxHorizontalSpd; }
        set { maxHorizontalSpd = value; }
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
        currBatteryWh = batteryCapacityWh;
        maxLiftSpd = Globals.MaxLiftSpd;
        maxDescentSpd = Globals.MaxDescnetSpd;
        maxHorizontalSpd = Globals.MaxHorizontalSpd;
    }

    void Update()
    {
        droneView.UpdateVisual(this);
        if (Simulator.instance.CurrentState != Simulator.State.Play)
        {
            return;
        }
        // Update energy status
        switch (subSwarm.CurrentState)
        {
            case SubSwarm.State.Hovering:
                break;
            case SubSwarm.State.Flying:

                batteryStatus -= 0.03f * Time.deltaTime * Globals.PlaySpeed;
                if (batteryStatus < 0)
                {
                    Debug.LogError(string.Format("{0} is out of battery!", this.name));
                }
                break;
            case SubSwarm.State.Landed:
                break;
            case SubSwarm.State.Recharging:
                break;
        }
        // Check if the time elapsed since the last data collection is greater than the interval
        if (Simulator.instance.ElapsedTime - lastDataCollectionTime >= dataCollectionInterval)
        {
            CollectData();
            lastDataCollectionTime = Simulator.instance.ElapsedTime; // Update the last collection time
        }
        LogState();
    }

    void EnergyDrop()
    {
        // Use energy comsumption model to calculate energy loss
    }

    public void Recharge(float amount)
    {
        currBatteryWh += amount * Time.deltaTime * Globals.PlaySpeed;
        if (currBatteryWh >= batteryCapacityWh)
        {
            currBatteryWh = batteryCapacityWh;
        }
        SyncBatStatus();
    }

    void SyncBatStatus()
    {
        batteryStatus = Mathf.Round(currBatteryWh / batteryCapacityWh * 100f) / 100f;
    }

    void CollectData()
    {
        DroneData data = new DroneData
        {
            timestring = Simulator.instance.GetTimeString(),
            position = transform.position,
            speed = speed,
            batteryStatus = batteryStatus,
            node = (subSwarm.Node != null) ? subSwarm.Node.name : "-",
            edge = (subSwarm.Edge != null) ? subSwarm.Edge.name : "-"
        };
        dataCollection.Add(data);
    }

    void LogState()
    {
        Debug.Log("BatteryStatus: " + batteryStatus);
        Debug.Log("currBatteryWh: " + currBatteryWh);
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
    public string node;
    public string edge;

    public string ToCSV()
    {
        return $"{timestring},{position.x},{position.y},{position.z},{speed},{batteryStatus},{node},{edge}";
    }
}
