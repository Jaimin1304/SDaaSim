using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] private Swarm swarm;
    [SerializeField] private SubSwarm subSwarm;
    [SerializeField] private Vector3 position;
    [SerializeField] private float selfWeight;
    [SerializeField] private float speed;
    [SerializeField] private float maxPayloadWeight;
    [SerializeField] private float batteryStatus;
    [SerializeField] private List<PayLoad> payloads = new List<PayLoad>();
}
