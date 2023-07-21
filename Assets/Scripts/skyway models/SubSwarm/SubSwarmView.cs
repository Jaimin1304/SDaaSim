using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSwarmView : MonoBehaviour
{
    public void Visual(SubSwarm subSwarm)
    {
        Vector3 targetPostition = new Vector3(
            subSwarm.Edge.Path[subSwarm.WayPointIndex].x,
            transform.position.y,
            subSwarm.Edge.Path[subSwarm.WayPointIndex].z
        );
        subSwarm.transform.LookAt(targetPostition);
    }
}
