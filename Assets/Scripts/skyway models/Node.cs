using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    List<Pad> pads;

    [SerializeField]
    List<Drone> drones;

    [SerializeField]
    List<Edge> edges; // Adding edges list to Node class

    Outline outline;

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start() {
        outline = GetComponentInChildren<Outline>();
    }

    void Update()
    {
        // Create a ray from the mouse cursor on screen in the direction of the camera
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits this Node
        if (Physics.Raycast(ray, out hit) && hit.transform.gameObject == this.gameObject)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
        }
    }

    public string GetId()
    {
        return id;
    }

    public List<Pad> GetPads()
    {
        return pads;
    }

    public List<Drone> GetDrones()
    {
        return drones;
    }

    public List<Edge> GetEdges()
    {
        return edges;
    }

    public SerializableNode ToSerializableNode()
    {
        return new SerializableNode()
        {
            id = id,
            position = transform.position,
            pads = pads.Select(pad => pad.GetId()).ToList(),
            drones = drones.Select(drone => drone.GetId()).ToList(),
            edges = edges.Select(edge => edge.GetId()).ToList(),
        };
    }
}
