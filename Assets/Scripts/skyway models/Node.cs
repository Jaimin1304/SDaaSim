using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
    public int id;
    public List<Pad> pads;
    public Vector3 position; 
    public List<Drone> drones;
    public List<Edge> edges;  // Adding edges list to Node class
}