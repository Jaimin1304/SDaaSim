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
            HandleHitObject(hit);
            // Check for mouse button events
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseButtonDown(hit);
            }
            if (Input.GetMouseButtonUp(0))
            {
                HandleMouseButtonUp(hit);
            }
        }
        else
        {
            HandleNoHit();
        }
    }

    void HandleHitObject(RaycastHit hit)
    {
        // Highlight the hit object if it implements IHighlightable
        IHighlightable hitObject = hit.transform.GetComponent<IHighlightable>();
        if (hitObject != null)
        {
            hitObject.Highlight();
            // Unhighlight the previous hit object if different from current and selected
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
    }

    void HandleMouseButtonDown(RaycastHit hit)
    {
        IHighlightable hitObject = hit.transform.GetComponent<IHighlightable>();
        // Assign hitObject to draggingObject on single click
        draggingObject = hitObject;
        // Enter focus mode on double click
        if (Time.time - lastClickTime < Globals.doubleClickGap)
        {
            EnterFocusMode(hit.transform.gameObject);
        }
        lastClickTime = Time.time;
    }

    void HandleMouseButtonUp(RaycastHit hit)
    {
        IHighlightable hitObject = hit.transform.GetComponent<IHighlightable>();
        // Connect nodes on drop
        if (draggingObject != null && hitObject != null && hitObject != draggingObject)
        {
            CreateEdgeBetweenNodes(draggingObject, hitObject);
        }
        // Handle node selection
        if (hitObject == draggingObject)
        {
            SelectNode(hitObject);
        }
        draggingObject = null;
    }

    void CreateEdgeBetweenNodes(IHighlightable from, IHighlightable to)
    {
        Node fromNode = ((MonoBehaviour)from).GetComponent<Node>();
        Node toNode = ((MonoBehaviour)to).GetComponent<Node>();
        if (fromNode != null && toNode != null)
        {
            Simulator.instance.CreateEdge(fromNode, toNode);
        }
    }

    void SelectNode(IHighlightable node)
    {
        Select(node);
        if (Simulator.instance.CurrentState == Simulator.State.Edit)
        {
            Show3DGizmos(((MonoBehaviour)node).transform.position);
        }
    }

    void HandleNoHit()
    {
        // Unhighlight previous hit object
        if (lastHitObject != null && lastHitObject != selectedObject)
        {
            lastHitObject.Unhighlight();
            lastHitObject = null;
        }
        // Deselect on click outside objects
        if (Input.GetMouseButtonDown(0) && !isInteractingWithGizmos)
        {
            DeSelect();
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
        // Raycast to check for gizmo interaction
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, gizmos3DLayer))
        {
            HandleGizmoSelection(hit);
        }
        else
        {
            HandleDeselection();
        }
        // Handle drag if the mouse button is held down
        if (Input.GetMouseButton(0) && selectedAxis != null)
        {
            HandleDragging();
        }
        // Reset the selected axis when the mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            selectedAxis = null;
        }
    }

    private void HandleGizmoSelection(RaycastHit hit)
    {
        // Handle gizmo selection logic
        isInteractingWithGizmos = true;
        GameObject gizmos = hit.transform.gameObject;
        HighlightGizmo(gizmos, gizmosHovered);
        lastHitGizmos = gizmos;
        if (Input.GetMouseButtonDown(0))
        {
            StartDragging(gizmos);
        }
    }

    private void HandleDeselection()
    {
        // Handle gizmo deselection logic
        isInteractingWithGizmos = false;
        if (Input.GetMouseButtonDown(0))
        {
            Hide3DGizmos();
        }

        if (lastHitGizmos != null)
        {
            HighlightGizmo(lastHitGizmos, gizmosDefault);
            lastHitGizmos = null;
        }
    }

    private void StartDragging(GameObject gizmos)
    {
        // Initialize dragging variables
        selectedAxis = gizmos.name; // Gizmos named "X", "Y", "Z"
        initialMousePos = Input.mousePosition;
        initialObjectPos = uiController.SelectedComponent.transform.position;
    }

    private void HandleDragging()
    {
        // Calculate delta and update positions
        Vector3 deltaMousePos = Input.mousePosition - initialMousePos;
        Vector3 axis = DetermineDragAxis();
        float dragDistance = CalculateDragDistance(deltaMousePos, axis);
        // Update position based on dragging
        UpdatePositions(axis, dragDistance);
        UpdateAffectedComponents();
    }

    private Vector3 DetermineDragAxis()
    {
        // Logic to determine which axis is being dragged
        Vector3 camPosition = Camera.main.transform.position;
        Vector3 objPosition = ((MonoBehaviour)selectedObject).transform.position;
        switch (selectedAxis)
        {
            case "X":
                return (objPosition.z - camPosition.z > 0) ? Vector3.right : Vector3.left;
            case "Y":
                return Vector3.up;
            case "Z":
                return (objPosition.x - camPosition.x > 0) ? Vector3.back : Vector3.forward;
            default:
                return Vector3.zero;
        }
    }

    private float CalculateDragDistance(Vector3 deltaMousePos, Vector3 axis)
    {
        // Calculate drag distance based on mouse movement
        return axis == Vector3.up
            ? deltaMousePos.y * Globals.editModeDragMultiplier
            : deltaMousePos.x * Globals.editModeDragMultiplier;
    }

    private void UpdatePositions(Vector3 axis, float dragDistance)
    {
        // Update positions of UI components and 3D gizmos
        Vector3 newPosition = initialObjectPos + axis * dragDistance;
        uiController.SelectedComponent.transform.position = newPosition;
        gizmos3D.transform.position = newPosition;
    }

    private void UpdateAffectedComponents()
    {
        // Update waypoints, nodes, or other components affected by the drag
        Component component = uiController.SelectedComponent.GetComponent<Component>();
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
            // if node is startnode for swarm, update swarm position as well
            UpdateSwarmPosition(node);
        }
    }

    private void UpdateSwarmPosition(Node node)
    {
        foreach (Request request in Simulator.instance.Skyway.Requests)
        {
            if (request.StartNode != node)
            {
                break;
            }
            foreach (SubSwarm subSwarm in request.Swarm.SubSwarms)
            {
                subSwarm.transform.position = uiController.SelectedComponent.transform.position;
            }
        }
    }

    private void HighlightGizmo(GameObject gizmo, Material material)
    {
        // Change the material of the gizmo to indicate selection or deselection
        Renderer renderer = gizmo.GetComponent<Renderer>();
        renderer.material = material;
    }

    void Select(IHighlightable obj)
    {
        if (obj == null)
        {
            return;
        }
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
