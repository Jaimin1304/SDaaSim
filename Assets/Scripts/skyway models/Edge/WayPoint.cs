using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    [SerializeField]
    Outline outline;

    [SerializeField]
    Utils utils;

    // Update is called once per frame
    void Update()
    {
        utils.Outline(outline, this.gameObject);
    }
}
