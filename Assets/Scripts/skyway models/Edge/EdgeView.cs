// EdgeView.cs - Handles Edge visualization
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EdgeView : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    TextMeshPro lengthText;

    Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
        if (lineRenderer == null)
        {
            CreateLineRenderer();
        }
    }

    void CreateLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
    }

    public void UpdateVisual(Vector3 leftNodePos, Vector3 rightNodePos, float length)
    {
        Vector3 heightOffset = new Vector3(0, 1, 0);
        lineRenderer.SetPosition(0, leftNodePos + heightOffset);
        lineRenderer.SetPosition(1, rightNodePos + heightOffset);

        Vector3 middlePosition = (leftNodePos + rightNodePos) / 2 + heightOffset * 2;
        lengthText.transform.position = middlePosition;

        lengthText.text = length.ToString("F2");

        // Let the length text face the camera
        lengthText.transform.rotation = mainCamera.transform.rotation;

        // Calculate distance from the camera
        float distance = Vector3.Distance(
            lengthText.transform.position,
            mainCamera.transform.position
        );

        // Scale text size based on distance
        float scaleValue = distance * 0.03f;
        lengthText.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
    }
}
