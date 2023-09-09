using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastHandler : MonoBehaviour
{
    [SerializeField]
    LayerMask layersToHit;

    IHighlightable lastHitObject; // Keep track of the last object you hit

    float lastClickTime = 0f; // Time of the last click

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, layersToHit))
        {
            IHighlightable hitObject = hit.transform.GetComponent<IHighlightable>();
            if (hitObject != null)
            {
                hitObject.Highlight();
                if (lastHitObject != null && lastHitObject != hitObject)
                {
                    lastHitObject.Unhighlight();
                }
                lastHitObject = hitObject;
            }
            // Check for double click
            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time - lastClickTime < Globals.doubleClickGap)
                {
                    // Double click detected
                    EnterFocusMode(hit.transform.gameObject);
                }
                lastClickTime = Time.time;
            }
        }
        else
        {
            if (lastHitObject != null)
            {
                lastHitObject.Unhighlight();
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
