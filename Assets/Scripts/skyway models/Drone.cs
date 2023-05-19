using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public float selfWeight;
    public float maxPayloadWeight;
    public float maxSpeed;
    public float currPayloadWeight;
    public float currSpeed;
    public float batteryStatus;
    public Swarm swarm;
    public SubSwarm subSwarm;
    public List<PayLoad> payloads = new List<PayLoad>();
}
