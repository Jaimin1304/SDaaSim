using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastHandler : MonoBehaviour
{
    [SerializeField]
    GameObject arrow3D;

    [SerializeField]
    Material arrowHovered;

    [SerializeField]
    Material arrowDefault;

    [SerializeField]
    LayerMask arrow3DLayer;
    GameObject lastHitArrow;
    Vector3 initialMousePos;
    Vector3 initialObjectPos;
    string selectedAxis;
    bool isInteractingWithArrow = false;

    [SerializeField]
    LayerMask skywayObjectLayer;

    IHighlightable lastHitObject; // Keep track of the last object hit
    float lastClickTime = 0f; // Time of the last click
    IHighlightable selectedObject;
    IHighlightable draggingObject;

    [SerializeField]
    UIController uiController;

    public IHighlightable SelectedObject
    {
        get { return selectedObject; }
        set { selectedObject = value; }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        HandleSkywayObjectHit(ray);
        Handle3DArrowHit(ray);
        if (arrow3D.activeSelf)
            Adjust3DArrowsScale();
    }

    void HandleSkywayObjectHit(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, skywayObjectLayer))
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
                    // Set the selected object and highlight it
                    draggingObject = hitObject;
                }
                // Check for double click
                if (Time.time - lastClickTime < Globals.doubleClickGap)
                {
                    EnterFocusMode(hit.transform.gameObject);
                }
                lastClickTime = Time.time;
            }
            if (Input.GetMouseButtonUp(0))
            {
                // Handle the drop action
                if (draggingObject != null && hitObject != null && hitObject != draggingObject)
                {
                    Node selectedNode = ((MonoBehaviour)draggingObject).GetComponent<Node>();
                    Node hitNode = ((MonoBehaviour)hitObject).GetComponent<Node>();
                    if (selectedNode != null && hitNode != null)
                    {
                        Simulator.instance.CreateEdge(selectedNode, hitNode);
                    }
                }
                // handle select and 3D arrow action
                if (hitObject == draggingObject)
                {
                    Select(hitObject);
                    Show3DArrow(((MonoBehaviour)hitObject).transform.position);
                }
                draggingObject = null;
            }
        }
        else
        {
            if (lastHitObject != null)
            {
                if (lastHitObject != selectedObject)
                    lastHitObject.Unhighlight();
                lastHitObject = null;
            }
            // handle cancel selection
            if (Input.GetMouseButtonDown(0) && !isInteractingWithArrow)
            {
                DeSelect();
            }
        }
    }

    void Show3DArrow(Vector3 position)
    {
        arrow3D.transform.position = position;
        arrow3D.SetActive(true);
    }

    void Hide3DArrow()
    {
        arrow3D.SetActive(false);
    }

    void Handle3DArrowHit(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, arrow3DLayer))
        {
            isInteractingWithArrow = true;
            GameObject arrow = hit.transform.gameObject;
            Renderer renderer = arrow.GetComponent<Renderer>();
            renderer.material = arrowHovered;
            lastHitArrow = arrow;

            if (Input.GetMouseButtonDown(0))
            {
                selectedAxis = arrow.name; // Assuming arrows are named "X", "Y", "Z"
                initialMousePos = Input.mousePosition;
                initialObjectPos = uiController.SelectedComponent.transform.position;
            }
        }
        else
        {
            isInteractingWithArrow = false;
            if (Input.GetMouseButtonDown(0))
            {
                Hide3DArrow();
            }
            if (lastHitArrow != null)
            {
                Renderer renderer = lastHitArrow.GetComponent<Renderer>();
                renderer.material = arrowDefault;
                lastHitArrow = null;
            }
        }
        if (Input.GetMouseButton(0) && selectedAxis != null)
        {
            Vector3 deltaMousePos = Input.mousePosition - initialMousePos;
            Vector3 axis = Vector3.zero;
            Vector3 camPosition = Camera.main.transform.position;
            Vector3 objPosition = ((MonoBehaviour)selectedObject).transform.position;
            float dragDistance = 0f;

            switch (selectedAxis)
            {
                case "X":
                    axis = (objPosition.z - camPosition.z > 0) ? Vector3.right: Vector3.left;
                    dragDistance = deltaMousePos.x * Globals.editModeDragMultiplier;
                    break;
                case "Y":
                    axis = Vector3.up;
                    dragDistance = deltaMousePos.y * Globals.editModeDragMultiplier;
                    break;
                case "Z":
                    axis = (objPosition.x - camPosition.x > 0) ? Vector3.back: Vector3.forward;
                    dragDistance = deltaMousePos.x * Globals.editModeDragMultiplier;
                    break;
            }
            uiController.SelectedComponent.transform.position =
                initialObjectPos + axis * dragDistance;
            arrow3D.transform.position = uiController.SelectedComponent.transform.position;
        }
        if (Input.GetMouseButtonUp(0))
        {
            selectedAxis = null;
        }
    }

    void Select(IHighlightable obj)
    {
        Debug.Log(String.Format("select {0}", obj));
        selectedObject?.Unhighlight();
        selectedObject = obj;
        obj.Highlight();
        uiController.SelectedComponent = ((MonoBehaviour)obj).gameObject;
    }

    void DeSelect()
    {
        Debug.Log("cancel selection");
        selectedObject?.Unhighlight();
        selectedObject = null;
        uiController.SelectedComponent = null;
    }

    void EnterFocusMode(GameObject centerObject)
    {
        CamController camController = Camera.main.GetComponent<CamController>();
        camController?.ToFocusMode(centerObject);
    }

    void Adjust3DArrowsScale()
    {
        float distanceFromCamera = Vector3.Distance(
            Camera.main.transform.position,
            arrow3D.transform.position
        );
        float scaleFactor = distanceFromCamera * Globals.arrow3DScaleValue;
        arrow3D.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
}
