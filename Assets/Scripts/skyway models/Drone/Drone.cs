using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.PlayerLoop;

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
    float physicalBatteryStatus;

    [SerializeField]
    float currBatteryJ;

    [SerializeField]
    float batteryCapacityJ;

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

    public float PhysicalBatteryStatus
    {
        get { return physicalBatteryStatus; }
        set { physicalBatteryStatus = value; }
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

                // traditional
                batteryStatus -= 0.0018f * Time.deltaTime * Globals.PlaySpeed;
                if (batteryStatus < 0)
                {
                    Debug.LogError(string.Format("{0} is out of battery!", this.name));
                }
                // physical
                float energyUsedPerSecond = subSwarm.CurrEngineSpd.magnitude * subSwarm.Epm; // 根据速度和epm计算每秒的能量消耗
                CurrBatteryJ -= energyUsedPerSecond * Time.deltaTime; // 更新电池电量
                if (CurrBatteryJ < 0)
                    CurrBatteryJ = 0;
                SyncBatStatus(); // 同步电池状态
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
        physicalBatteryStatus = 1;
        batteryCapacityJ = Globals.DroneBatCap;
        currBatteryJ = batteryCapacityJ;
        maxLiftSpd = Globals.MaxLiftSpd;
        maxDescentSpd = Globals.MaxDescnetSpd;
        maxHorizontalSpd = Globals.MaxHorizontalSpd;
    }

    void EnergyDrop()
    {
        // Use energy comsumption model to calculate energy loss
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

    void SyncBatStatus()
    {
        physicalBatteryStatus = Mathf.Round(currBatteryJ / batteryCapacityJ * 100f) / 100f;
    }

    void CollectData()
    {
        DroneData data = new DroneData
        {
            timestring = Simulator.instance.GetTimeString(),
            position = transform.position,
            speed = speed,
            batteryStatus = batteryStatus,
            physicalBatteryStatus = physicalBatteryStatus,
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
    public float physicalBatteryStatus;
    public string node;
    public string edge;

    public string ToCSV()
    {
        return $"{timestring},{position.x},{position.y},{position.z},{speed},{batteryStatus},{physicalBatteryStatus},{node},{edge}";
    }
}
