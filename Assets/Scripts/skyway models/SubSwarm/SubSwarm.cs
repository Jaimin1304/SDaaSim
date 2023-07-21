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
    List<Drone> drones = new List<Drone>();

    [SerializeField]
    private Node node;
    public Node Node
    {
        get { return node; }
        set { node = value; }
    }

    [SerializeField]
    private Edge edge;
    public Edge Edge
    {
        get { return edge; }
        set { edge = value; }
    }

    public enum State
    {
        Standby,
        Operating
    }

    public State currentState;

    int wayPointIndex;
    public int WayPointIndex
    {
        get { return wayPointIndex; }
        set { wayPointIndex = value; }
    }

    [SerializeField]
    float speed;

    void Awake() 
    {
        speed = 5;
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        currentState = State.Operating;
        transform.position = node.transform.position;
    }

    void Update()
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

        if (Vector3.Distance(transform.position, Edge.Path[wayPointIndex]) <= speed)
        {
            if (Vector3.Distance(transform.position, targetNode.transform.position) <= speed)
            {
                ToStandby(targetNode);
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
    }

    public void ToStandby(Node node)
    {
        currentState = State.Standby;
        Node = node;
        transform.position = node.transform.position;
        Edge = null;
    }

    public void AskForCommand() { }

    public string GetId()
    {
        return id;
    }

    public Swarm GetSwarm()
    {
        return parentSwarm;
    }

    public List<Drone> GetDrones()
    {
        return drones;
    }

    public SerializableSubSwarm ToSerializableSubSwarm()
    {
        return new SerializableSubSwarm()
        {
            id = id,
            position = transform.position,
            drones = drones.Select(drone => drone.ToSerializableDrone()).ToList(),
            node = node.GetId(),
            edge = edge.GetId()
        };
    }
}
