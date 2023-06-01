using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private string id;
    [SerializeField] private List<Pad> pads;
    [SerializeField] private List<Drone> drones;
    [SerializeField] private List<Edge> edges; // Adding edges list to Node class

    private void Awake() 
    {
        id = Guid.NewGuid().ToString();
    }
}
