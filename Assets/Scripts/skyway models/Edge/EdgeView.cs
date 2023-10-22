// EdgeView.cs - Handles Edge visualization
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EdgeView : MonoBehaviour, IHighlightable
{
    [SerializeField]
    Outline outline;

    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    TextMeshPro lengthText;

    [SerializeField]
    Material material;

    [SerializeField]
    GameObject transparentBroder;

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
        lineRenderer.startWidth = Globals.EdgeThickness;
        lineRenderer.endWidth = Globals.EdgeThickness;
        lineRenderer.startColor = material.color;
        lineRenderer.endColor = material.color;
    }

    public void UpdateVisual(
        Edge edge,
        Vector3 leftNodePos,
        Vector3 rightNodePos,
        List<WayPoint> wayPoints,
        List<float> lengths,
        float totalLength,
        string gameObjectName
    )
    {
        Vector3 heightOffset = new Vector3(0, Globals.edgeHeightOffset, 0);
        // Set up line renderer with waypoints.
        SetLineRendererPositions(leftNodePos, rightNodePos, wayPoints, heightOffset);
        // Calculate the middle position.
        Vector3 middlePosition = CalculateMiddlePosition(
            leftNodePos,
            rightNodePos,
            wayPoints,
            heightOffset
        );
        // Update text, border, and collider positions.
        UpdatePositionsAndText(edge, middlePosition, gameObjectName, totalLength);
    }

    void SetLineRendererPositions(
        Vector3 leftNodePos,
        Vector3 rightNodePos,
        List<WayPoint> wayPoints,
        Vector3 heightOffset
    )
    {
        lineRenderer.positionCount = wayPoints.Count + 2;
        lineRenderer.SetPosition(0, leftNodePos + heightOffset);
        for (int i = 0; i < wayPoints.Count; i++)
        {
            lineRenderer.SetPosition(i + 1, wayPoints[i].transform.position);
        }
        lineRenderer.SetPosition(wayPoints.Count + 1, rightNodePos + heightOffset);
    }

    Vector3 CalculateMiddlePosition(
        Vector3 leftNodePos,
        Vector3 rightNodePos,
        List<WayPoint> wayPoints,
        Vector3 heightOffset
    )
    {
        if (wayPoints.Count > 0)
        {
            return wayPoints[wayPoints.Count / 2].transform.position + heightOffset * 4;
        }
        else
        {
            return (leftNodePos + rightNodePos) / 2 + heightOffset * 4;
        }
    }

    void UpdatePositionsAndText(
        Edge edge,
        Vector3 middlePosition,
        string gameObjectName,
        float totalLength
    )
    {
        // Position the length text and set its content.
        lengthText.transform.position = middlePosition;
        lengthText.text = gameObjectName + "\n" + totalLength.ToString("F2") + "m";
        // Position the transparent border.
        transparentBroder.transform.position = middlePosition;
        // Adjust edge collider and move edge.
        edge.BorderCollider.center = middlePosition - edge.transform.position;
        edge.MoveEdgeToPosition(middlePosition);
        // Let the length text face the camera.
        lengthText.transform.rotation = mainCamera.transform.rotation;
        // Scale UI elements based on distance to camera.
        AdjustScaleBasedOnDistance(middlePosition, edge);
    }

    void AdjustScaleBasedOnDistance(Vector3 position, Edge edge)
    {
        float distance = Vector3.Distance(position, mainCamera.transform.position);
        float scaleValue = distance * Globals.textScaleValue;
        const int scaleConst = 5;
        lengthText.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        transparentBroder.transform.localScale = new Vector3(
            scaleValue * scaleConst,
            scaleValue * scaleConst,
            scaleValue * scaleConst
        );
        edge.BorderCollider.radius = scaleValue * scaleConst;
    }

    public void UpdateEdgeThickness()
    {
        if (lineRenderer == null)
        {
            CreateLineRenderer();
        }
        lineRenderer.startWidth = Globals.EdgeThickness;
        lineRenderer.endWidth = Globals.EdgeThickness;
    }

    public void Highlight()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void Unhighlight()
    {
        if (outline != null)
            outline.enabled = false;
    }
}
