using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarm : MonoBehaviour
{
    public Request request;
    public List<Drone> drones = new List<Drone>();
    public List<SubSwarm> subSwarms = new List<SubSwarm>();
}
