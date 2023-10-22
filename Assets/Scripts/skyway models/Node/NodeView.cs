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

    [SerializeField]
    GameObject transparentBroder;

    [SerializeField]
    BoxCollider borderCollider;

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
            node.Drones.Count,
            node.Pads.Count
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
            node.Drones.Count,
            node.Pads.Count
        );
    }

    public void ArrangePads(Node node)
    {
        List<Pad> pads = new List<Pad>(node.RechargePads);
        pads.AddRange(node.NonRechargePads);
        if (pads == null || pads.Count == 0)
            return;
        // 1. Determine the values for m and n
        int n = Mathf.CeilToInt(Mathf.Sqrt(pads.Count));
        int m = Mathf.CeilToInt((float)pads.Count / n);
        float padGap = Globals.padGap;
        // 2. Calculate the position for each Pad using m and n
        for (int i = 0; i < pads.Count; i++)
        {
            int row = i / n; // Calculate the row
            int col = i % n; // Calculate the column
            float xPos = row * padGap;
            float zPos = col * padGap;
            // Set the Pad's position
            pads[i].transform.position =
                new Vector3(xPos, 0, zPos)
                + node.transform.position
                + new Vector3(-padGap * (m - 1) / 2f, 0, -padGap * (n - 1) / 2f);
        }
        // set broder scale
        transparentBroder.transform.localScale = new Vector3(
            m * padGap,
            transparentBroder.transform.localScale.y,
            n * padGap
        );
        // set box collider
        borderCollider.size = new Vector3(m * padGap, borderCollider.size.y, n * padGap);
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
