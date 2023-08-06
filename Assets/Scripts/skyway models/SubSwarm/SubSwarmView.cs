using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSwarmView : MonoBehaviour
{
    public void Visual(SubSwarm subSwarm)
    {
        if (subSwarm.CurrentState == SubSwarm.State.Operating)
        {
            Vector3 targetPostition = new Vector3(
                subSwarm.Edge.Path[subSwarm.WayPointIndex].x,
                transform.position.y,
                subSwarm.Edge.Path[subSwarm.WayPointIndex].z
            );
            subSwarm.transform.LookAt(targetPostition);
        }
    }

    public void InitDronePosition(SubSwarm subSwarm)
    {
        // Display the subswarm as a wedge formation
        int centerIndex = subSwarm.Drones.Count / 2;
        float displayOffset = Globals.droneGapView * centerIndex;
        for (int i = 0; i < subSwarm.Drones.Count; i++)
        {
            subSwarm.Drones[i].transform.position =
                subSwarm.transform.position
                + new Vector3(
                    Globals.droneGapView * i - displayOffset,
                    Globals.droneHeightOffset,
                    -Globals.droneGapView * Mathf.Abs(centerIndex - i)
                );
        }
    }
}
