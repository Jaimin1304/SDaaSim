using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    [SerializeField]
    Edge edge;

    public Edge Edge
    {
        get { return edge; }
        set { edge = value; }
    }
}
