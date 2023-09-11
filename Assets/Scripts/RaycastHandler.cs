using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastHandler : MonoBehaviour
{
    [SerializeField]
    LayerMask layersToHit;
    IHighlightable lastHitObject; // Keep track of the last object hit
    float lastClickTime = 0f; // Time of the last click
    IHighlightable selectedObject;
    IHighlightable draggingNode;

    [SerializeField]
    UIController uiController;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, layersToHit))
        {
            IHighlightable hitObject = hit.transform.GetComponent<IHighlightable>();

            if (hitObject != null)
            {
                hitObject.Highlight();
                if (
                    lastHitObject != null
                    && lastHitObject != hitObject
                    && lastHitObject != selectedObject
                )
                {
                    lastHitObject.Unhighlight();
                }
                lastHitObject = hitObject;
            }

            if (Input.GetMouseButtonDown(0))
            {
                // Check for single click
                if (hit.transform != null)
                {
                    uiController.DisplayDetails(hit.transform.gameObject);
                    // Set the selected object and highlight it
                    if (selectedObject != null)
                    {
                        selectedObject.Unhighlight();
                        draggingNode = hitObject;
                    }
                    selectedObject = hitObject;
                    Debug.Log(selectedObject);
                    selectedObject.Highlight();
                }
                // Check for double click
                if (Time.time - lastClickTime < Globals.doubleClickGap)
                {
                    EnterFocusMode(hit.transform.gameObject);
                }
                lastClickTime = Time.time;
            }

            // Handle the drop action
            if (Input.GetMouseButtonUp(0))
            {
                if (draggingNode != null && hitObject != null && hitObject != draggingNode)
                {
                    Node selectedNode = ((MonoBehaviour)draggingNode).GetComponent<Node>();
                    Node hitNode = ((MonoBehaviour)hitObject).GetComponent<Node>();
                    if (selectedNode != null && hitNode != null)
                    {
                        Simulator.instance.CreateEdge(selectedNode, hitNode);
                    }
                }
                draggingNode = null;
            }
        }
        else
        {
            if (lastHitObject != null)
            {
                if (lastHitObject != selectedObject)
                {
                    lastHitObject.Unhighlight();
                }
                lastHitObject = null;
            }
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("cancel selection");
                if (selectedObject != null)
                {
                    selectedObject.Unhighlight();
                }
            }
        }
    }

    void EnterFocusMode(GameObject centerObject)
    {
        CamController camController = Camera.main.GetComponent<CamController>();
        if (camController != null)
        {
            camController.ToFocusMode(centerObject);
        }
    }
}
