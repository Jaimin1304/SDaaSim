using System;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour
{
    [SerializeField]
    private string id;

    [SerializeField]
    private Node node;

    [SerializeField]
    private bool isAvailable;

    public string Id
    {
        get { return id; }
    }

    public Node Node
    {
        get { return node; }
        set { node = value; }
    }

    public bool IsAvailable
    {
        get { return isAvailable; }
        set { isAvailable = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        IsAvailable = true;
    }

    public void Occupy()
    {
        IsAvailable = false;
    }

    public void Release()
    {
        IsAvailable = true;
    }

    //public SerializablePad ToSerializablePad()
    //{
    //    return new SerializablePad()
    //    {
    //        id = this.id,
    //        node = this.node.Id,
    //        isAvailable = this.isAvailable,
    //    };
    //}
}
