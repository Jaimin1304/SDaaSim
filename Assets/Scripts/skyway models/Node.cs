using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neigbours = new List<Node>();
    public List<Pad> pads = new List<Pad>();
}
