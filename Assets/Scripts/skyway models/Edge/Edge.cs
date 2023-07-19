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
    List<WayPoint> wayPoints = new List<WayPoint>();

    [SerializeField]
    List<float> subEdgeLengths = new List<float>();

    [SerializeField]
    private float totalLength;

    public float Length
    {
        get { return totalLength; }
    }

    [SerializeField]
    EdgeView edgeView; // Reference to the EdgeView script

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        InitLengths();
        UpdateEdgeVisual();
    }

    void Update()
    {
        UpdateEdgeVisual();
    }

    public string GetId()
    {
        return id;
    }

    public Node GetLeftNode()
    {
        return leftNode;
    }

    public Node GetRightNode()
    {
        return rightNode;
    }

    void InitLengths()
    {
        subEdgeLengths.Clear(); // Clear any previous lengths
        // If there are waypoints, calculate subedge lengths
        if (wayPoints.Count > 0)
        {
            // Calculate the length from the left node to the first waypoint
            float firstLength = Vector3.Distance(
                leftNode.transform.position,
                wayPoints[0].transform.position
            );
            subEdgeLengths.Add(firstLength);
            // Calculate lengths between each pair of waypoints
            for (int i = 0; i < wayPoints.Count - 1; i++)
            {
                float subEdgeLength = Vector3.Distance(
                    wayPoints[i].transform.position,
                    wayPoints[i + 1].transform.position
                );
                subEdgeLengths.Add(subEdgeLength);
            }
            // Calculate the length from the last waypoint to the right node
            float lastLength = Vector3.Distance(
                wayPoints[wayPoints.Count - 1].transform.position,
                rightNode.transform.position
            );
            subEdgeLengths.Add(lastLength);
        }
        else // If there are no waypoints, just calculate length from left node to right node
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

    void UpdateEdgeVisual()
    {
        edgeView.UpdateVisual(
            leftNode.transform.position,
            rightNode.transform.position,
            wayPoints,
            subEdgeLengths,
            totalLength
        );
    }

    public SerializableEdge ToSerializableEdge()
    {
        return new SerializableEdge()
        {
            id = id,
            leftNode = leftNode.GetId(),
            rightNode = rightNode.GetId(),
        };
    }
}
