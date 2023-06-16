using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    Node leftNode;

    [SerializeField]
    Node rightNode;

    [SerializeField]
    LineRenderer lineRenderer; // Add this field

    void Awake()
    {
        id = Guid.NewGuid().ToString();
        if (lineRenderer == null)
        {
            createLineRenderer();
        }
    }

    void Start()
    {
        updateLineRenderer();
    }

    void Update()
    {
        if (lineRenderer == null)
        {
            createLineRenderer();
        }
        updateLineRenderer();
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

    void createLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void updateLineRenderer()
    {
        Vector3 heightOffset = new Vector3(0, 0, 0);
        // Set the position of the line to match the nodes
        if (leftNode != null && rightNode != null)
        {
            lineRenderer.SetPosition(0, leftNode.transform.position + heightOffset);
            lineRenderer.SetPosition(1, rightNode.transform.position + heightOffset);
        }
    }

    public SerializableEdge ToSerializableEdge() { 
        return new SerializableEdge() {
            id = id,
            leftNode = leftNode.GetId(),
            rightNode = rightNode.GetId(),
        };
    }
}
