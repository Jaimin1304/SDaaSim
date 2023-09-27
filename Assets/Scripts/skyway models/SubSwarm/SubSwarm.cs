using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        Recharging,
        ArrivedAtTarget
    }

    [SerializeField]
    State currentState;

    int wayPointIndex;

    [SerializeField]
    float speed;

    [SerializeField]
    float epm;

    bool destReached;

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
        subSwarmView.InitDronePosition(this);
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
                // Land at a node and wait for command
                RechargeLogic();
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
                    ToRecharging(targetNode);
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

    void RechargeLogic()
    {
        if (drones.All(drone => drone.BatteryStatus == 1f))
        {
            AskForCommand();
        }
    }

    public void MoveToTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime * Globals.PlaySpeed;
    }

    public void ToFlying(Edge edge)
    {
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
        // restart drone animations
        subSwarmView.ToggleDroneAnimation(this, 1);
    }

    void updateEpm()
    {
        epm = KirchsteinECM.instance.CalculateEpm();
    }

    public void ToHovering(Node node)
    {
        currentState = State.Hovering;
        Node = node;
        transform.position = node.transform.position;
        Edge = null;
        wayPointIndex = 0;
        // restart drone animations
        subSwarmView.ToggleDroneAnimation(this, 1);
    }

    public bool ToRecharging(Node node)
    {
        if (node.Capacity < node.LandedDrones.Count + drones.Count)
        {
            Debug.Log(string.Format("Can't land at node {0}, due to low capacity", node.name));
            return false;
        }
        currentState = State.Recharging;
        Node = node;
        transform.position = node.transform.position;
        Edge = null;
        wayPointIndex = 0;
        node.LandedDrones.AddRange(drones);
        Debug.Log(node.name);
        Debug.Log(node.LandedDrones.Count);
        subSwarmView.LandVisualUpdate(this);
        Debug.Log("Recharging!");
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
