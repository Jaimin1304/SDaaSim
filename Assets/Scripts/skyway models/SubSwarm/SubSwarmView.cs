using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubSwarmView : MonoBehaviour
{
    [SerializeField]
    TextMeshPro nameTag;

    Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void InitVisual(string tagName)
    {
        nameTag.text = tagName;
    }

    public void UpdateVisual()
    {
        // Calculate distance from the camera
        float distance = Vector3.Distance(
            nameTag.transform.position,
            mainCamera.transform.position
        );
        // Let the name tag face the camera
        nameTag.transform.rotation = mainCamera.transform.rotation;
        // Scale text size based on distance
        float scaleValue = distance * Globals.textScaleValue;
        nameTag.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
    }

    public void Visual(SubSwarm subSwarm)
    {
        if (subSwarm.CurrentState == SubSwarm.State.Flying)
        {
            Vector3 targetPostition = new Vector3(
                subSwarm.Edge.Path[subSwarm.WayPointIndex].x,
                transform.position.y,
                subSwarm.Edge.Path[subSwarm.WayPointIndex].z
            );
            subSwarm.transform.LookAt(targetPostition);
        }
    }

    public void LandVisualUpdate(SubSwarm subSwarm)
    {
        // stop animation
        ToggleDroneAnimation(subSwarm, 0);
        //subSwarm.transform.position = subSwarm.Node.transform.position;
        subSwarm.transform.position = new Vector3(
            subSwarm.Node.transform.position.x,
            subSwarm.Node.transform.position.y - 2.2f,
            subSwarm.Node.transform.position.z
        );
    }

    public void ToggleDroneAnimation(SubSwarm subSwarm, float start)
    {
        foreach (Drone drone in subSwarm.Drones)
        {
            Animator droneAnimator = drone.GetComponentInChildren<Animator>();
            if (droneAnimator != null)
            {
                droneAnimator.speed = start;
            }
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
