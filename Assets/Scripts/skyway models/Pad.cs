using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    Node node;

    [SerializeField]
    bool isAvailable;

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        isAvailable = true;
    }

    public string GetId()
    {
        return id;
    }

    public Node GetNode()
    {
        return node;
    }

    [SerializeField]
    public bool IsAvailable()
    {
        return isAvailable;
    }

    [SerializeField]
    void Occupy()
    {
        isAvailable = false;
    }

    [SerializeField]
    void Release()
    {
        isAvailable = true;
    }

    public SerializablePad ToSerializablePad()
    {
        return new SerializablePad()
        {
            id = this.id,
            node = this.node.GetId(),
            isAvailable = this.isAvailable,
        };
    }
}
