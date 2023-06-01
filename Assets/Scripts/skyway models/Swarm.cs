using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarm : MonoBehaviour
{
    [SerializeField] private Request request;
    [SerializeField] private List<Drone> drones = new List<Drone>();
    [SerializeField] private List<SubSwarm> subSwarms = new List<SubSwarm>();
}
