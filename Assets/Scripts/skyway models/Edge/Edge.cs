using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Edge : MonoBehaviour
{
    [SerializeField]
    private string id;

    [SerializeField]
    private Node leftNode;

    [SerializeField]
    private Node rightNode;

    [SerializeField]
    private List<WayPoint> wayPoints = new List<WayPoint>();

    [SerializeField]
    private List<float> subEdgeLengths = new List<float>();

    [SerializeField]
    private float totalLength;

    [SerializeField]
    private EdgeView edgeView;

    private List<Vector3> path = new List<Vector3>();

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
        InitLengths();
        InitPath();
        UpdateEdgeVisual();
    }

    void Update()
    {
        UpdateEdgeVisual();
    }

    void InitLengths()
    {
        subEdgeLengths.Clear(); // Clear any previous lengths
        // If there are WayPoints, calculate subedge lengths
        if (WayPoints.Count > 0)
        {
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
        }
        else // If there are no WayPoints, just calculate length from left node to right node
        {
            float length = Vector3.Distance(
                leftNode.transform.position,
                rightNode.transform.position
            );
            subEdgeLengths.Add(length);
        }
        // calculate the total length
        totalLength = subEdgeLengths.Sum();
    }

    void InitPath()
    {
        path.Add(leftNode.transform.position);
        for (int i = 0; i < wayPoints.Count; i++)
        {
            path.Add(wayPoints[i].transform.position);
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
            totalLength
        );
    }

    public SerializableEdge ToSerializableEdge()
    {
        return new SerializableEdge()
        {
            id = id,
            leftNode = leftNode.Id,
            rightNode = rightNode.Id,
        };
    }
}
