using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static bool HasEdgeBetween(Node node1, Node node2)
    {
        return node1.HasEdgeTo(node2) || node2.HasEdgeTo(node1);
    }
}
