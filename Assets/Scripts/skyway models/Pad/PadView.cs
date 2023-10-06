using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadView : MonoBehaviour
{
    [SerializeField]
    Material RechargeableMat;

    [SerializeField]
    Material NonRechargeableMat;

    [SerializeField]
    Transform plane;

    public void SyncPadView(Pad pad)
    {
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        planeRenderer.material = pad.Rechargeable ? RechargeableMat : NonRechargeableMat;
    }
}
