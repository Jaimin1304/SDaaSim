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
    float bodyWeight;

    [SerializeField]
    float maxLiftSpd;

    [SerializeField]
    float maxDescentSpd;

    [SerializeField]
    float maxHorizontalSpd;

    [SerializeField]
    float maxPayloadWeight;

    [SerializeField]
    float payloadWeight;

    [SerializeField]
    List<Payload> payloads = new();

    [SerializeField]
    DroneView droneView;

    [SerializeField]
    float batteryStatus;

    [SerializeField]
    float currBatteryJ;

    [SerializeField]
    float batteryCapacityJ;

    [SerializeField]
    float epm;

    [SerializeField]
    float eps;

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
        set { id = value; }
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

    public float BodyWeight
    {
        get { return bodyWeight; }
        set { bodyWeight = value; }
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

    public float MaxPayloadWeight
    {
        get { return maxPayloadWeight; }
        set { maxPayloadWeight = value; }
    }

    public float PayloadWeight
    {
        get { return payloadWeight; }
        set { payloadWeight = value; }
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

    public float CurrBatteryJ
    {
        get { return currBatteryJ; }
        set { currBatteryJ = value; }
    }

    public float BatteryCapacityJ
    {
        get { return batteryCapacityJ; }
        set { batteryCapacityJ = value; }
    }

    public float Epm
    {
        get { return epm; }
        set { epm = value; }
    }

    public float Eps
    {
        get { return eps; }
        set { eps = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        Init();
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
        //LogState();
    }

    public void Init()
    {
        droneView.initVisual(this);
        droneView.UpdateVisual(this);
        batteryStatus = 1;
        batteryCapacityJ = Globals.DroneBatCap;
        currBatteryJ = batteryCapacityJ;
        maxLiftSpd = Globals.MaxLiftSpd;
        maxDescentSpd = Globals.MaxDescnetSpd;
        maxHorizontalSpd = Globals.MaxHorizontalSpd;
        FetchPayloadWeight();
    }

    void FetchPayloadWeight()
    {
        payloadWeight = payloads.Sum(payload => payload.Weight);
    }

    public void Recharge(float amount)
    {
        currBatteryJ += amount * Time.deltaTime * Globals.PlaySpeed;
        if (currBatteryJ >= batteryCapacityJ)
        {
            currBatteryJ = batteryCapacityJ;
        }
        SyncBatStatus();
    }

    public void SyncBatStatus()
    {
        batteryStatus = Mathf.Round(currBatteryJ / batteryCapacityJ * 100f) / 100f;
    }

    void CollectData()
    {
        DroneData data = new DroneData
        {
            timestring = Simulator.instance.GetTimeString(),
            position = transform.position,
            va = subSwarm.AirSpd.magnitude,
            absSpd = (subSwarm.AirSpd + Globals.WindSpd).magnitude,
            batteryStatus = batteryStatus,
            epm = epm,
            eps = eps,
            g = subSwarm.G,
            airDensity = subSwarm.AirDensity,
            currBatteryJ = currBatteryJ,
            node = (subSwarm.Node != null) ? subSwarm.Node.name : "-",
            edge = (subSwarm.Edge != null) ? subSwarm.Edge.name : "-"
        };
        dataCollection.Add(data);
    }

    void LogState()
    {
        Debug.Log("BatteryStatus: " + batteryStatus);
        Debug.Log("currBatteryJ: " + currBatteryJ);
    }

    public SerializableDrone ToSerializableDrone()
    {
        return new SerializableDrone()
        {
            id = id,
            name = gameObject.name,
            subswarm = subSwarm.Id,
            bodyWeight = bodyWeight,
            payloadWeight = payloadWeight,
            maxPayloadWeight = maxPayloadWeight,
            batteryStatus = batteryStatus,
            epm = epm,
            currBatteryJ = currBatteryJ,
            batteryCapacityJ = batteryCapacityJ,
            payloads = payloads.Select(payload => payload.Id).ToList(),
        };
    }
}

public class DroneData
{
    public string timestring;
    public Vector3 position;
    public float va;
    public float absSpd;
    public float batteryStatus;
    public float epm;
    public float eps;
    public float g;
    public float currBatteryJ;
    public float airDensity;
    public string node;
    public string edge;

    public string ToCSV()
    {
        return $"{timestring},{position.x},{position.y},{position.z},{va},{absSpd},{batteryStatus},{epm},{eps},{g},{airDensity},{currBatteryJ},{node},{edge}";
    }
}
