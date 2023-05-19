using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public Swarm swarm;
    public SubSwarm subSwarm;
    public Vector3 position;
    public float selfWeight;
    public float currSpeed;
    public float maxPayloadWeight;
    public float maxSpeed;
    public float currPayloadWeight;
    public float batteryStatus;
    public List<PayLoad> payloads = new List<PayLoad>();
}
