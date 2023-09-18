using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Edge : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    Node leftNode;

    [SerializeField]
    Node rightNode;

    [SerializeField]
    List<WayPoint> wayPoints = new();

    [SerializeField]
    List<float> subEdgeLengths = new();

    [SerializeField]
    float totalLength;

    [SerializeField]
    EdgeView edgeView;

    List<Vector3> path = new();

    public string Id
    {
        get { return id; }
    }

    public Node LeftNode
    {
        get { return leftNode; }
        set { leftNode = value; }
    }

    public Node RightNode
    {
        get { return rightNode; }
        set { rightNode = value; }
    }

    public List<WayPoint> WayPoints
    {
        get { return wayPoints; }
        set { wayPoints = value; }
    }

    public float TotalLength
    {
        get { return totalLength; }
    }

    public List<Vector3> Path
    {
        get { return path; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        CalLengths();
        InitPath();
        UpdateEdgeVisual();
    }

    void Update()
    {
        UpdateEdgeVisual();
    }

    public void CalLengths()
    {
        subEdgeLengths.Clear(); // Clear any previous lengths
        // If there are no WayPoints, just calculate length from left node to right node
        if (WayPoints.Count == 0)
        {
            float length = Vector3.Distance(
                leftNode.transform.position,
                rightNode.transform.position
            );
            subEdgeLengths.Add(length);
            totalLength = subEdgeLengths.Sum();
            return;
        }
        // Calculate the length from the left node to the first waypoint
        float firstLength = Vector3.Distance(
            leftNode.transform.position,
            WayPoints[0].transform.position
        );
        subEdgeLengths.Add(firstLength);
        // Calculate lengths between each pair of WayPoints
        for (int i = 0; i < WayPoints.Count - 1; i++)
        {
            float subEdgeLength = Vector3.Distance(
                WayPoints[i].transform.position,
                WayPoints[i + 1].transform.position
            );
            subEdgeLengths.Add(subEdgeLength);
        }
        // Calculate the length from the last waypoint to the right node
        float lastLength = Vector3.Distance(
            WayPoints[WayPoints.Count - 1].transform.position,
            rightNode.transform.position
        );
        subEdgeLengths.Add(lastLength);
        // calculate the total length
        totalLength = subEdgeLengths.Sum();
    }

    void InitPath()
    {
        path.Add(leftNode.transform.position);
        for (int i = 0; i < wayPoints.Count; i++)
        {
            path.Add(wayPoints[i].transform.position);
            wayPoints[i].Edge = this;
        }
        path.Add(rightNode.transform.position);
    }

    void UpdateEdgeVisual()
    {
        edgeView.UpdateVisual(
            leftNode.transform.position,
            rightNode.transform.position,
            WayPoints,
            subEdgeLengths,
            totalLength,
            gameObject.name
        );
    }

    public SerializableEdge ToSerializableEdge()
    {
        return new SerializableEdge()
        {
            id = id,
            leftNode = leftNode.Id,
            rightNode = rightNode.Id,
            totalLength = totalLength,
        };
    }
}
