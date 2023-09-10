using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastHandler : MonoBehaviour
{
    [SerializeField]
    LayerMask layersToHit;

    IHighlightable lastHitObject; // Keep track of the last object hit

    float lastClickTime = 0f; // Time of the last click

    [SerializeField]
    UIController uiController;

    IHighlightable selectedObject;

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
