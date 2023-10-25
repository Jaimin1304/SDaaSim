using UnityEngine;
using System;

public class WayPoint : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    Edge edge;

    public string Id
    {
        get { return id; }
        set { id = value; }
    }

    public Edge Edge
    {
        get { return edge; }
        set { edge = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    public SerializableWayPoint ToSerializableWayPoint()
    {
        return new SerializableWayPoint()
        {
            id = id,
            position = transform.position,
            edge = edge.Id
        };
    }
}
