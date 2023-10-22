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

    [SerializeField]
    SphereCollider borderCollider;

    List<Vector3> path = new();

    public string Id
    {
        get { return id; }
    }

    public EdgeView EdgeView
    {
        get { return edgeView; }
        set { edgeView = value; }
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

    public SphereCollider BorderCollider
    {
        get { return borderCollider; }
        set { borderCollider = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        CalLengths();
        CalPath();
        UpdateEdgeVisual();
        edgeView.UpdateEdgeThickness();
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

    void CalPath()
    {
        path.Clear();
        path.Add(leftNode.transform.position);
        for (int i = 0; i < wayPoints.Count; i++)
        {
            path.Add(wayPoints[i].transform.position);
            wayPoints[i].Edge = this;
        }
        path.Add(rightNode.transform.position);
    }

    public void SyncEdge()
    {
        CalLengths();
        CalPath();
        UpdateEdgeVisual();
    }

    public bool RemoveWayPoint(WayPoint wayPoint)
    {
        if (!wayPoints.Contains(wayPoint))
        {
            return false;
        }
        wayPoints.Remove(wayPoint);
        Destroy(wayPoint.gameObject);
        SyncEdge();
        return true;
    }

    void UpdateEdgeVisual()
    {
        edgeView.UpdateVisual(
            this,
            leftNode.transform.position,
            rightNode.transform.position,
            WayPoints,
            subEdgeLengths,
            totalLength,
            gameObject.name
        );
    }

    public void MoveEdgeToPosition(Vector3 newPosition)
    {
        // Step 1: Calculate the difference in position
        Vector3 positionDifference = newPosition - transform.position;
        // Step 2: Move the edge object to the new position
        transform.position = newPosition;
        // Step 3: Move all child objects in the opposite direction to keep their world space position
        foreach (Transform child in transform)
        {
            child.position -= positionDifference;
        }
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
