using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SubSwarm : MonoBehaviour
{
    [SerializeField]
    private string id;

    [SerializeField]
    private SubSwarmView subSwarmView;

    [SerializeField]
    private Swarm parentSwarm;

    [SerializeField]
    List<Drone> drones = new();

    [SerializeField]
    private Node node;

    [SerializeField]
    private Edge edge;

    public enum State
    {
        Standby,
        Operating
    }

    [SerializeField]
    State currentState;

    private int wayPointIndex;

    [SerializeField]
    private float speed;

    private bool destReached;

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

    void Awake()
    {
        speed = 3;
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        currentState = State.Operating;
        transform.position = node.transform.position;
        subSwarmView.InitDronePosition(this);
    }

    public void UpdateLogic()
    {
        switch (currentState)
        {
            case State.Standby:
                // stay still, waiting for command
                break;

            case State.Operating:
                // move along path, once reach node, switch to standby
                Tick(Node == Edge.LeftNode);
                break;
        }
        subSwarmView.Visual(this);
    }

    void Tick(bool leftToRight)
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
                ToStandby(targetNode);
                AskForCommand();
                return;
            }
            transform.position = Edge.Path[wayPointIndex];
            wayPointIndex += indexIncrease;
        }
        MoveToTarget(Edge.Path[wayPointIndex]);
    }

    public void MoveToTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    public void ToOperating(Edge edge)
    {
        currentState = State.Operating;
        Edge = edge;
        if (node == edge.LeftNode)
        {
            wayPointIndex = 0;
        }
        else
        {
            wayPointIndex = edge.Path.Count - 2;
        }
    }

    public void ToStandby(Node node)
    {
        currentState = State.Standby;
        Node = node;
        transform.position = node.transform.position;
        Edge = null;
        wayPointIndex = 0;
    }

    public void AskForCommand()
    {
        SkywaySimulator.instance.UpdateSubSwarm(this);
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
