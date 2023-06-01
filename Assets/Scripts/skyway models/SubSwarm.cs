using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSwarm : MonoBehaviour
{
    [SerializeField] private Swarm master;
    [SerializeField] private List<Drone> drones = new List<Drone>();
    [SerializeField] private Node target;
}
