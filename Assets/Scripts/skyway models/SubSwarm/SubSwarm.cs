using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

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
    Vector3 currSpd = new Vector3(0, 0, 0);

    [SerializeField]
    float speed;

    [SerializeField]
    float epm;

    public string Id
    {
        get { return id; }
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

    void Awake()
    {
        speed = 15;
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        currentState = State.Flying;
        transform.position = node.transform.position;
        subSwarmView.SetFlyPosition(this);
        subSwarmView.InitVisual(gameObject.name);
    }

    void Update()
    {
        subSwarmView.UpdateVisual(); // update nametag
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
        currSpd = CalculateSpeed(direction);
        Debug.Log("target: " + target);
        Debug.Log("direction: " + direction);
        Debug.Log("currSpd: " + currSpd);
        transform.position += currSpd * Time.deltaTime * Globals.PlaySpeed;
    }

    Vector3 CalculateSpeed(Vector3 direction)
    {
        float maxLiftSpd = Globals.MaxLiftSpd;
        float maxDescentSpd = Globals.MaxDescnetSpd;
        float maxHorizontalSpd = Globals.MaxHorizontalSpd;
        // Determine whether it is lift or descent
        float maxVerticalSpd = direction.y >= 0 ? maxLiftSpd : maxDescentSpd;
        Debug.Log("maxVerticalSpd: " + maxVerticalSpd);
        float horizontalDirectionSpd = Mathf.Sqrt(
            direction.x * direction.x + direction.z * direction.z
        );
        Vector3 speed = new Vector3();
        // Check whether the direction speed will hit the vertical or horizontal speed limit
        float currHorizontalSpd;
        if (
            direction.y / horizontalDirectionSpd
            >= maxVerticalSpd / maxHorizontalSpd
        )
        {
            speed.y = maxVerticalSpd;
            currHorizontalSpd = speed.y * horizontalDirectionSpd / Mathf.Abs(direction.y);
        }
        else
        {
            currHorizontalSpd = maxHorizontalSpd;
            speed.y = currHorizontalSpd * direction.y / horizontalDirectionSpd;
        }
        speed.x = currHorizontalSpd * direction.x / horizontalDirectionSpd;
        speed.z = currHorizontalSpd * direction.z / horizontalDirectionSpd;

        return speed;
    }

    float CalFlightAngle()
    {
        float horizontalSpeed = Mathf.Sqrt(currSpd.x * currSpd.x + currSpd.z * currSpd.z);
        float thetaRadians = Mathf.Atan2(currSpd.y, horizontalSpeed);
        //float thetaDegrees = thetaRadians * Mathf.Rad2Deg;
        return thetaRadians;
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

    public SerializableSubSwarm ToSerializableSubSwarm()
    {
        return new SerializableSubSwarm()
        {
            id = id,
            position = transform.position,
            drones = drones.Select(drone => drone.Id).ToList(),
            node = node.Id,
            edge = "",
            wayPointIndex = wayPointIndex,
            currentState = currentState.ToString()
        };
    }
}
