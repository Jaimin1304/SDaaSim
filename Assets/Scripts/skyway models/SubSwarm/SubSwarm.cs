using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEditor.Search;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;

public class SubSwarm : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    SubSwarmView subSwarmView;

    [SerializeField]
    Swarm parentSwarm;

    [SerializeField]
    List<Drone> drones = new();

    [SerializeField]
    Node node;

    [SerializeField]
    Edge edge;

    public enum State
    {
        Hovering,
        Flying,
        Recharging, // Land at rechargeable pad
        Landed, // Land at non-rechargeable pad
        Arrived
    }

    [SerializeField]
    State currentState;

    int wayPointIndex;

    [SerializeField]
    Vector3 airSpd = new Vector3(0, 0, 0);

    [SerializeField]
    float flightAngle;

    [SerializeField]
    float g;

    [SerializeField]
    float airDensity;

    [SerializeField]
    bool usePhysicsEPM;

    public string Id
    {
        get { return id; }
        set { id = value; }
    }

    public State CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    public Swarm ParentSwarm
    {
        get { return parentSwarm; }
        set { parentSwarm = value; }
    }

    public List<Drone> Drones
    {
        get { return drones; }
        set { drones = value; }
    }

    public Node Node
    {
        get { return node; }
        set { node = value; }
    }

    public Edge Edge
    {
        get { return edge; }
        set { edge = value; }
    }

    public int WayPointIndex
    {
        get { return wayPointIndex; }
        set { wayPointIndex = value; }
    }

    public float FlightAngle
    {
        get { return flightAngle; }
        set { flightAngle = value; }
    }

    public float G
    {
        get { return g; }
        set { g = value; }
    }

    public float AirDensity
    {
        get { return airDensity; }
        set { airDensity = value; }
    }

    public bool UsePhysicsEPM
    {
        get { return usePhysicsEPM; }
        set { usePhysicsEPM = value; }
    }

    public Vector3 AirSpd
    {
        get { return airSpd; }
        set { airSpd = value; }
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
        subSwarmView.UpdateVisual(this); // update nametag
        subSwarmView.DrawEngineSpeed(this); // draw engine speed
        subSwarmView.DrawWindSpeed(this); // draw wind speed
        //LogState();
    }

    public void Init()
    {
        if (node == null)
        {
            return;
        }
        currentState = State.Flying;
        transform.position = node.transform.position;
        subSwarmView.SetFlyPosition(this);
        subSwarmView.InitVisual(gameObject.name);
        flightAngle = 0f;
        G = Globals.g0;
        usePhysicsEPM = false;
        flightAngle = CalFlightAngle();
        //ToFlying(edge);
    }

    public void UpdateLogic()
    {
        switch (currentState)
        {
            case State.Hovering:
                // stay still, waiting for command
                HoverLogic();
                break;

            case State.Flying:
                // move along path, once reach node, switch to Hovering
                FlyLogic(Node == Edge.LeftNode);
                break;

            case State.Recharging:
                // Land at a rechargeable pad and wait for command
                RechargeLogic();
                break;

            case State.Landed:
                // Land at a non-rechargeable pad and wait for command
                LandLogic();
                break;

            case State.Arrived:
                // Send arrived event to the simulator for once
                ArrivedLogic();
                break;
        }
        subSwarmView.Visual(this);
    }

    void HoverLogic()
    {
        return;
    }

    void FlyLogic(bool leftToRight)
    {
        Node targetNode;
        int indexIncrease;
        if (leftToRight)
        {
            targetNode = Edge.RightNode;
            indexIncrease = 1;
        }
        else
        {
            targetNode = Edge.LeftNode;
            indexIncrease = -1;
        }
        // check if reach milestone
        if (
            Vector3.Distance(transform.position, Edge.Path[wayPointIndex])
            <= Globals.nodeTouchDistance
        )
        {
            // update flight angle
            flightAngle = CalFlightAngle();
            // check if reach target node
            if (
                Vector3.Distance(transform.position, targetNode.transform.position)
                <= Globals.nodeTouchDistance
            )
            {
                // check if target node is destination
                if (targetNode == parentSwarm.Request.DestNode)
                {
                    ToArrived(targetNode);
                }
                else
                {
                    ToHovering(targetNode);
                    AskForCommand();
                }
                return;
            }
            // milestone is a waypoint, keep going
            transform.position = Edge.Path[wayPointIndex];
            wayPointIndex += indexIncrease;
        }
        MoveToTarget(Edge.Path[wayPointIndex]);
        // get altitude
        float altitude = transform.position.y;
        // update gravity
        g = GravityModel.instance.CalGravity(altitude);
        // update air density
        airDensity = AirDensityModel.instance.CalAirDensity(altitude, g);
        // calculate drone energy consumption, apply energy loss for each drone
        if (usePhysicsEPM)
        {
            ApplyPhysicalEPMForDrones();
        }
        else
        {
            ApplyTraditionalEPMForDrones();
        }
    }

    void ApplyTraditionalEPMForDrones()
    {
        Debug.Log("ApplyTraditionalEPMForDrones");
        foreach (Drone drone in drones)
        {
            drone.BatteryStatus -= 0.00005f * Time.deltaTime * Globals.PlaySpeed * airSpd.magnitude;
            drone.CurrBatteryJ = drone.BatteryCapacityJ * drone.BatteryStatus;
            if (drone.BatteryStatus < 0)
            {
                Debug.LogError(string.Format("{0} is out of battery!", drone.name));
                drone.CurrBatteryJ = 0;
                drone.BatteryStatus = 0;
            }
        }
    }

    void ApplyPhysicalEPMForDrones()
    {
        Debug.Log("ApplyPhysicalEPMForDrones");
        // update epm for each drone according to their payload weight
        foreach (Drone drone in drones)
        {
            float epm = KirchsteinECM.instance.CalEpm(
                airSpd.magnitude,
                flightAngle,
                g,
                airDensity,
                drone.PayloadWeight
            );
            drone.Epm = epm;
            Debug.Log("Epm: " + epm);
            // calculate energyUsedPerSecond by epm and va
            float energyUsedPerSecond = airSpd.magnitude * epm;
            Debug.Log("energyUsedPerSecond: " + energyUsedPerSecond);
            // update battery status
            drone.CurrBatteryJ -= energyUsedPerSecond * Time.deltaTime * Globals.PlaySpeed;
            if (drone.CurrBatteryJ < 0)
            {
                Debug.LogError(string.Format("{0} is out of battery!", drone.name));
                drone.CurrBatteryJ = 0;
                drone.BatteryStatus = 0;
            }
            drone.SyncBatStatus();
        }
    }

    float CalFlightAngle()
    {
        if (edge == null)
        {
            return 0;
        }
        Vector3 direction = (edge.Path[wayPointIndex] - transform.position).normalized;
        Vector3 horizontalProjection = new Vector3(direction.x, 0, direction.z);
        float theta = Vector3.Angle(horizontalProjection, direction);
        if (direction.y < 0)
        {
            theta = -theta;
        }
        return theta;
    }

    void LandLogic()
    {
        return;
    }

    void RechargeLogic()
    {
        drones.ForEach(drone => drone.Recharge(Globals.PadRechargeRate));
        if (drones.All(drone => drone.BatteryStatus >= 0.6f))
        {
            AskForCommand();
        }
    }

    void ArrivedLogic()
    {
        return;
    }

    public void MoveToTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        airSpd = CalculateEngineSpeedWithWind(direction);
        Vector3 absSpd = airSpd + Globals.WindSpd;
        transform.position += absSpd * Time.deltaTime * Globals.PlaySpeed;
    }

    Vector3 CalculateEngineSpeedWithWind(Vector3 dir)
    {
        Vector3 direction = new Vector3(dir.x, 0, dir.z);
        Vector3 windSpeed = Globals.WindSpd;
        float ob = Globals.MaxHorizontalSpd;
        // Find the anti-wind speed vector
        Vector3 antiWS = -windSpeed;
        // Find the length of the vector parallel to the direction
        float oa = antiWS.magnitude; // length of vector OA
        float cosTheta = Vector3.Dot(antiWS.normalized, direction);
        // Solve the quadratic equation with respect to ab
        // Get ab using cosine theorem
        float[] roots = Utils.SolveQuadratic(1, 2 * oa * cosTheta, oa * oa - ob * ob);
        float ab = roots[0] >= 0 ? roots[0] : roots[1];
        Vector3 abVector = direction.normalized * ab;
        // Adjust ab if the corresponding vertical vector exceeds the vertical speed limit
        float currVerticalSpd = abVector.x * dir.y / dir.x;
        float maxLift = Globals.MaxLiftSpd;
        float maxDescent = Globals.MaxDescnetSpd;
        if (currVerticalSpd > maxLift)
        {
            abVector *= maxLift / currVerticalSpd;
            currVerticalSpd = maxLift;
        }
        else if (currVerticalSpd < -1 * maxDescent)
        {
            abVector *= maxDescent / Mathf.Abs(currVerticalSpd);
            currVerticalSpd = -1 * maxDescent;
        }
        abVector.y = currVerticalSpd;
        return antiWS + abVector; // The resultant vector OA + AB
    }

    public void ToFlying(Edge edge)
    {
        Debug.Log("ToFlying");
        currentState = State.Flying;
        Edge = edge;
        if (node == edge.LeftNode)
        {
            wayPointIndex = 0;
        }
        else
        {
            wayPointIndex = edge.Path.Count - 2;
        }
        // restart drone flying animations
        subSwarmView.ToggleDroneAnimation(this, 1);
        subSwarmView.SetFlyPosition(this);
    }

    public void ToHovering(Node node)
    {
        Debug.Log("ToHovering");
        currentState = State.Hovering;
        Node = node;
        transform.position = node.transform.position;
        Edge = null;
        wayPointIndex = 0;
        // restart drone flying animations
        subSwarmView.ToggleDroneAnimation(this, 1);
        subSwarmView.SetFlyPosition(this);
    }

    public void ToArrived(Node node)
    {
        Debug.Log("ToArrived");
        currentState = State.Arrived;
        Node = node;
        transform.position = node.transform.position;
        Edge = null;
        wayPointIndex = 0;
        Simulator.instance.CheckSimulationComplete();
        // stop drone animations
        subSwarmView.ToggleDroneAnimation(this, 0);
    }

    public bool ToRecharging(Node node)
    {
        Debug.Log("ToRecharging");
        List<Pad> freeRechargePads = node.FreeRechargePads();
        // Check if still has available pads
        if (freeRechargePads.Count < drones.Count)
        {
            return false;
        }
        // Land each drone
        for (int i = 0; i < drones.Count; i++)
        {
            freeRechargePads[i].Drone = drones[i];
            Drones[i].Pad = freeRechargePads[i];
        }
        // Update drone visuals
        //subSwarmView.LandVisualUpdate(this);
        subSwarmView.SetLandPosition(this, freeRechargePads);
        // Update subswarm states
        currentState = State.Recharging;
        Node = node;
        transform.position = node.transform.position;
        Edge = null;
        wayPointIndex = 0;
        node.Drones.AddRange(drones);
        Debug.Log(node.name);
        Debug.Log(node.Drones.Count);
        return true;
    }

    public bool ToLanded(Node node)
    {
        Debug.Log("ToLanded");
        currentState = State.Recharging;
        Node = node;
        transform.position = node.transform.position;
        Edge = null;
        wayPointIndex = 0;
        node.Drones.AddRange(drones);
        Debug.Log(node.name);
        Debug.Log(node.Drones.Count);
        subSwarmView.LandVisualUpdate(this);
        return true;
    }

    public void AskForCommand()
    {
        Simulator.instance.UpdateDrones(this);
        Simulator.instance.UpdateSubSwarm(this);
    }

    void LogState()
    {
        Debug.Log("airSpd: " + airSpd);
        Debug.Log("currVerticalSpd: " + airSpd.y);
        Debug.Log("currHorizontalSpd: " + MathF.Sqrt(airSpd.x * airSpd.x + airSpd.z * airSpd.z));
        Debug.Log("windSpd: " + Globals.WindSpd);
    }

    public SerializableSubSwarm ToSerializableSubSwarm()
    {
        return new SerializableSubSwarm()
        {
            id = id,
            name = gameObject.name,
            airSpd = airSpd,
            parentSwarm = parentSwarm.Id,
            position = transform.position,
            drones = drones.Select(drone => drone.Id).ToList(),
            node = node.Id,
            edge = edge ? edge.Id : "",
            wayPointIndex = wayPointIndex,
            currentState = currentState.ToString()
        };
    }
}
