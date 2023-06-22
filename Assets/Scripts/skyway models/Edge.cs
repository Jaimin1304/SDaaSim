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
    float length = 0;

    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    TextMeshPro lengthText;

    Camera mainCamera;

    void Awake()
    {
        id = Guid.NewGuid().ToString();
        if (lineRenderer == null)
        {
            CreateLineRenderer();
        }
        mainCamera = Camera.main;
    }

    void Start()
    {
        UpdateVisual();
        length = Vector3.Distance(leftNode.transform.position, rightNode.transform.position);
        lengthText.text = length.ToString("F2"); // Display with 2 decimal places
    }

    void Update()
    {
        if (lineRenderer == null)
        {
            CreateLineRenderer();
        }
        UpdateVisual();

        if (mainCamera != null)
        {
            // Let the length text face the camera
            lengthText.transform.rotation = mainCamera.transform.rotation;
            // Calculate distance from the camera
            float distance = Vector3.Distance(
                lengthText.transform.position,
                mainCamera.transform.position
            );
            // Scale text size based on distance
            float scaleValue = distance * 0.03f; // Adjust this value as needed
            lengthText.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        }
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

    void CreateLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void UpdateVisual()
    {
        Vector3 heightOffset = new Vector3(0, 1, 0);
        if (leftNode != null && rightNode != null)
        {
            // Set the position of the line to match the nodes
            lineRenderer.SetPosition(0, leftNode.transform.position + heightOffset);
            lineRenderer.SetPosition(1, rightNode.transform.position + heightOffset);
            // Position the text in the middle of the edge and slightly above it
            Vector3 middlePosition =
                (leftNode.transform.position + rightNode.transform.position) / 2 + heightOffset * 2;
            lengthText.transform.position = middlePosition;
        }
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
