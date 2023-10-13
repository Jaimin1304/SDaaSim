using System;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour
{
    [SerializeField]
    string id;

    [SerializeField]
    Node node;

    [SerializeField]
    bool rechargeable;

    [SerializeField]
    PadView padView;

    [SerializeField]
    Drone drone;

    public string Id
    {
        get { return id; }
    }

    public Node Node
    {
        get { return node; }
        set { node = value; }
    }

    public bool Rechargeable
    {
        get { return rechargeable; }
        set { rechargeable = value; }
    }

    public Drone Drone
    {
        get { return drone; }
        set { drone = value; }
    }

    void Awake()
    {
        id = Guid.NewGuid().ToString();
    }

    void Start()
    {
        padView.SyncPadView(this);
    }

    public void ChangeRechargeableState(bool newState)
    {
        rechargeable = newState;
        padView.SyncPadView(this);
    }

    public SerializablePad ToSerializablePad()
    {
        return new SerializablePad()
        {
            id = this.id,
            node = this.node.Id,
            rechargeable = this.rechargeable,
        };
    }
}
