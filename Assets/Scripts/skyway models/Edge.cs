using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    [SerializeField]
    private string id;

    [SerializeField]
    private Node leftNode;

    [SerializeField]
    private Node rightNode;

    [SerializeField]
    private LineRenderer lineRenderer; // Add this field

    private void Awake()
    {
        id = Guid.NewGuid().ToString();
        if (lineRenderer == null)
        {
            createLineRenderer();
        }
    }

    private void Start()
    {
        updateLineRenderer();
    }

    private void Update()
    {
        if (lineRenderer == null)
        {
            createLineRenderer();
        }
        updateLineRenderer();
    }

    private void createLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    private void updateLineRenderer()
    {
        Vector3 heightOffset = new Vector3(0, 0, 0);
        // Set the position of the line to match the nodes
        if (leftNode != null && rightNode != null)
        {
            lineRenderer.SetPosition(0, leftNode.transform.position + heightOffset);
            lineRenderer.SetPosition(1, rightNode.transform.position + heightOffset);
        }
    }
}
