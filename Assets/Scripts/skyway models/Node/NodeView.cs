using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NodeView : MonoBehaviour, IHighlightable
{
    [SerializeField]
    Outline outline;

    [SerializeField]
    TextMeshPro nameTag;

    Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
        outline = GetComponentInChildren<Outline>();
    }

    public void initVisual(Node node)
    {
        nameTag.text = string.Format(
            "{0} [{1}/{2}]",
            node.name,
            node.LandedDrones.Count,
            node.TotalCapacity
        );
    }

    public void UpdateVisual(Node node)
    {
        // Let the name tag face the camera
        nameTag.transform.rotation = mainCamera.transform.rotation;
        // Calculate distance from the camera
        float distance = Vector3.Distance(
            nameTag.transform.position,
            mainCamera.transform.position
        );
        // Scale text size based on distance
        float scaleValue = distance * Globals.textScaleValue;
        nameTag.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        nameTag.text = string.Format(
            "{0} [{1}/{2}]",
            node.name,
            node.LandedDrones.Count,
            node.TotalCapacity
        );
    }

    public void ArrangePads(Node node)
    {
        List<Pad> pads = node.Pads;
        if (pads == null || pads.Count == 0)
            return;
        // 1. Determine the values for m and n
        int n = Mathf.FloorToInt(Mathf.Sqrt(pads.Count));
        int m = Mathf.CeilToInt((float)pads.Count / n);
        float padWidth = 4f;
        float padHeight = 4f;
        // 2. Calculate the position for each Pad using m and n
        for (int i = 0; i < pads.Count; i++)
        {
            int row = i / n; // Calculate the row
            int col = i % n; // Calculate the column
            float xPos = col * padWidth;
            float zPos = row * padHeight;
            // Set the Pad's position
            pads[i].transform.position =
                new Vector3(xPos, 0, zPos)
                + node.transform.position
                + new Vector3(-padWidth * n / 2f, 0, -padHeight * n / 2f);
        }
    }

    public void Highlight()
    {
        outline.enabled = true;
    }

    public void Unhighlight()
    {
        outline.enabled = false;
    }
}
