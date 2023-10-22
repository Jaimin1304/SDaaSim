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
            // update broder & collider position
            transparentBroder.transform.position = middlePosition;
            edge.BorderCollider.center = middlePosition - edge.transform.position;
            edge.MoveEdgeToPosition(middlePosition);
        }
        else
        {
            Vector3 middlePosition = (leftNodePos + rightNodePos) / 2 + heightOffset * 4;
            lengthText.transform.position = middlePosition;
            // update broder & collider position
            transparentBroder.transform.position = middlePosition;
            edge.BorderCollider.center = middlePosition - edge.transform.position;
            edge.MoveEdgeToPosition(middlePosition);
        }

        lengthText.text = gameObjectName + "\n" + totalLength.ToString("F2") + "m";
        // Let the length text face the camera
        lengthText.transform.rotation = mainCamera.transform.rotation;
        // Calculate distance from the camera
        float distance = Vector3.Distance(
            lengthText.transform.position,
            mainCamera.transform.position
        );
        // Scale text, border and collider based on distance
        float scaleValue = distance * Globals.textScaleValue;
        lengthText.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        int scaleConst = 5;
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
