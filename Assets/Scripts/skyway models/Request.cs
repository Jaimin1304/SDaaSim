using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request : MonoBehaviour
{
    [SerializeField] private Node startNode;
    [SerializeField] private Node destNode;
    [SerializeField] private List<PayLoad> payloads = new List<PayLoad>();
}
