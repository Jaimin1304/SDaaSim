using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEditor.Search;

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
    Vector3 currEngineSpd = new Vector3(0, 0, 0);

    [SerializeField]
    float epm;

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

    public float Epm
    {
        get { return Epm; }
        set { Epm = value; }
    }

    public Vector3 CurrEngineSpd
    {
        get { return currEngineSpd; }
        set { currEngineSpd = value; }
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
        subSwarmView.UpdateVisual(); // update nametag
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

        if (
            Vector3.Distance(transform.position, Edge.Path[wayPointIndex])
            <= Globals.nodeTouchDistance
        )
        { // reach milestone
            if (
                Vector3.Distance(transform.position, targetNode.transform.position)
                <= Globals.nodeTouchDistance
            )
            { // reach target
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
            transform.position = Edge.Path[wayPointIndex];
            wayPointIndex += indexIncrease;
        }
        MoveToTarget(Edge.Path[wayPointIndex]);
    }

    void LandLogic()
    {
        return;
    }

    void RechargeLogic()
    {
        bool allDronesFullyCharged = true;
        foreach (Drone drone in drones)
        {
            drone.Recharge(Globals.PadRechargeRate);
            if (drone.BatteryStatus < 0.99f) // Check if full recharge
            {
                allDronesFullyCharged = false;
            }
        }
        if (allDronesFullyCharged)
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
        currEngineSpd = CalculateEngineSpeedWithWind(direction);
        transform.position +=
            (currEngineSpd + Globals.WindSpd) * Time.deltaTime * Globals.PlaySpeed;
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
        subSwarmView.LandVisualUpdate(this);
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
        Debug.Log("currEngineSpd: " + currEngineSpd);
        Debug.Log("currVerticalSpd: " + currEngineSpd.y);
        Debug.Log(
            "currHorizontalSpd: "
                + MathF.Sqrt(currEngineSpd.x * currEngineSpd.x + currEngineSpd.z * currEngineSpd.z)
        );
        Debug.Log("windSpd: " + Globals.WindSpd);
    }

    public SerializableSubSwarm ToSerializableSubSwarm()
    {
        return new SerializableSubSwarm()
        {
            id = id,
            parentSwarm = parentSwarm.Id,
            position = transform.position,
            drones = drones.Select(drone => drone.Id).ToList(),
            node = node.Id,
            edge = "",
            wayPointIndex = wayPointIndex,
            currentState = currentState.ToString()
        };
    }
}
