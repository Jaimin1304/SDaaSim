using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    float length = 0;

    [SerializeField]
    EdgeView edgeView; // Reference to the EdgeView script

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        length = Vector3.Distance(leftNode.transform.position, rightNode.transform.position);
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

    void UpdateEdgeVisual()
    {
        edgeView.UpdateVisual(leftNode.transform.position, rightNode.transform.position, length);
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
