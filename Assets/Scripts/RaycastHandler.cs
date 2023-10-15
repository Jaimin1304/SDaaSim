using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastHandler : MonoBehaviour
{
    [SerializeField]
    GameObject gizmos3D;

    [SerializeField]
    Material gizmosHovered;

    [SerializeField]
    Material gizmosDefault;

    [SerializeField]
    LayerMask gizmos3DLayer;
    GameObject lastHitGizmos;
    Vector3 initialMousePos;
    Vector3 initialObjectPos;
    string selectedAxis;
    bool isInteractingWithGizmos = false;

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

    void OnDestroy()
    {
        // Unsubscribe from UnityEvents
        uiController.OnDeleteEvent.RemoveListener(Hide3DGizmos);
    }

    void Start()
    {
        // Subscribe to UnityEvents
        uiController.OnDeleteEvent.AddListener(Hide3DGizmos);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        HandleSkywayObjectHit(ray);
        Handle3DGizmosHit(ray);
        if (gizmos3D.activeSelf)
            Adjust3DGizmossScale();
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
                // handle select and 3D gizmos action
                if (hitObject == draggingObject)
                {
                    Select(hitObject);
                    if (Simulator.instance.CurrentState == Simulator.State.Edit)
                    {
                        Show3DGizmos(((MonoBehaviour)hitObject).transform.position);
                    }
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
            if (Input.GetMouseButtonDown(0) && !isInteractingWithGizmos)
            {
                DeSelect();
            }
        }
    }

    void Show3DGizmos(Vector3 position)
    {
        gizmos3D.transform.position = position;
        gizmos3D.SetActive(true);
    }

    void Hide3DGizmos()
    {
        gizmos3D.SetActive(false);
    }

    void Handle3DGizmosHit(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, gizmos3DLayer))
        {
            isInteractingWithGizmos = true;
            GameObject gizmos = hit.transform.gameObject;
            Renderer renderer = gizmos.GetComponent<Renderer>();
            renderer.material = gizmosHovered;
            lastHitGizmos = gizmos;

            if (Input.GetMouseButtonDown(0))
            {
                selectedAxis = gizmos.name; // Assuming gizmoss are named "X", "Y", "Z"
                initialMousePos = Input.mousePosition;
                initialObjectPos = uiController.SelectedComponent.transform.position;
            }
        }
        else
        {
            isInteractingWithGizmos = false;
            if (Input.GetMouseButtonDown(0))
            {
                Hide3DGizmos();
            }
            if (lastHitGizmos != null)
            {
                Renderer renderer = lastHitGizmos.GetComponent<Renderer>();
                renderer.material = gizmosDefault;
                lastHitGizmos = null;
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
                    axis = (objPosition.z - camPosition.z > 0) ? Vector3.right : Vector3.left;
                    dragDistance = deltaMousePos.x * Globals.editModeDragMultiplier;
                    break;
                case "Y":
                    axis = Vector3.up;
                    dragDistance = deltaMousePos.y * Globals.editModeDragMultiplier;
                    break;
                case "Z":
                    axis = (objPosition.x - camPosition.x > 0) ? Vector3.back : Vector3.forward;
                    dragDistance = deltaMousePos.x * Globals.editModeDragMultiplier;
                    break;
            }
            uiController.SelectedComponent.transform.position =
                initialObjectPos + axis * dragDistance;
            gizmos3D.transform.position = uiController.SelectedComponent.transform.position;
            // update affected edge lengths
            WayPoint wayPoint = uiController.SelectedComponent.GetComponent<WayPoint>();
            Node node = uiController.SelectedComponent.GetComponent<Node>();
            if (wayPoint != null)
            {
                wayPoint.Edge.SyncEdge();
            }
            else if (node != null)
            {
                foreach (Edge edge in node.Edges)
                {
                    edge.SyncEdge();
                }
            }
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

    void Adjust3DGizmossScale()
    {
        float distanceFromCamera = Vector3.Distance(
            Camera.main.transform.position,
            gizmos3D.transform.position
        );
        float scaleFactor = distanceFromCamera * Globals.gizmos3DScaleValue;
        gizmos3D.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
}
