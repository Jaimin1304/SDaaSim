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

    [SerializeField]
    Material material;

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
        lineRenderer.material = material;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = Globals.edgeLineWidth;
        lineRenderer.endWidth = Globals.edgeLineWidth;
        lineRenderer.startColor = material.color;
        lineRenderer.endColor = material.color;
    }

    public void UpdateVisual(
        Vector3 leftNodePos,
        Vector3 rightNodePos,
        List<WayPoint> wayPoints,
        List<float> lengths,
        float totalLength,
        string gameObjectName
    )
    {
        Vector3 heightOffset = new Vector3(0, 1, 0);

        // set waypoints as lineRenderer positions
        lineRenderer.positionCount = wayPoints.Count + 2;
        lineRenderer.SetPosition(0, leftNodePos + heightOffset);
        for (int i = 0; i < wayPoints.Count; i++)
        {
            lineRenderer.SetPosition(i + 1, wayPoints[i].transform.position);
        }
        lineRenderer.SetPosition(wayPoints.Count + 1, rightNodePos + heightOffset);

        // Position the text in the middle of the edge and slightly above it
        if (wayPoints.Count > 0)
        {
            Vector3 middlePosition =
                wayPoints[wayPoints.Count / 2].transform.position + heightOffset * 4;
            lengthText.transform.position = middlePosition;
        }
        else
        {
            Vector3 middlePosition = (leftNodePos + rightNodePos) / 2 + heightOffset * 4;
            lengthText.transform.position = middlePosition;
        }

        lengthText.text = gameObjectName + "\n" + totalLength.ToString("F2") + "m";
        // Let the length text face the camera
        lengthText.transform.rotation = mainCamera.transform.rotation;
        // Calculate distance from the camera
        float distance = Vector3.Distance(
            lengthText.transform.position,
            mainCamera.transform.position
        );
        // Scale text size based on distance
        float scaleValue = distance * Globals.textScaleValue;
        lengthText.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
    }
}
