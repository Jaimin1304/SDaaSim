using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubSwarmView : MonoBehaviour
{
    [SerializeField]
    TextMeshPro nameTag;

    [SerializeField]
    TextMeshPro engineSpdTag;

    [SerializeField]
    TextMeshPro windSpdTag;

    [SerializeField]
    Material engineSpdMaterial;

    [SerializeField]
    Material windSpdMaterial;

    Camera mainCamera;

    [SerializeField]
    LineRenderer engineSpdLineRenderer;

    [SerializeField]
    LineRenderer windSpdLineRenderer;

    void Awake()
    {
        mainCamera = Camera.main;
        InitLineRenderer(engineSpdLineRenderer);
        InitLineRenderer(windSpdLineRenderer);
    }

    void InitLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
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
        engineSpdTag.transform.rotation = mainCamera.transform.rotation;
        windSpdTag.transform.rotation = mainCamera.transform.rotation;
        // Scale text size based on distance
        float scaleValue = distance * Globals.textScaleValue;
        nameTag.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        engineSpdTag.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        windSpdTag.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
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
        // Stop animation
        ToggleDroneAnimation(subSwarm, 0);
        // Drone.transform.position = pad.tansform.position
        foreach (Drone drone in subSwarm.Drones)
        {
            drone.transform.position = new Vector3(
                drone.Pad.transform.position.x,
                drone.Pad.transform.position.y + 2f,
                drone.Pad.transform.position.z
            );
        }
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

    public void SetFlyPosition(SubSwarm subSwarm)
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

    public void DrawEngineSpeed(SubSwarm subSwarm)
    {
        Vector3 startPosition = subSwarm.transform.position;
        Vector3 endPosition = startPosition + subSwarm.CurrEngineSpd;
        engineSpdLineRenderer.SetPosition(0, startPosition);
        engineSpdLineRenderer.SetPosition(1, endPosition);
        engineSpdTag.transform.position = endPosition;
    }

    public void DrawWindSpeed(SubSwarm subSwarm)
    {
        Vector3 startPosition = subSwarm.transform.position;
        Vector3 endPosition = startPosition + Globals.WindSpd;
        windSpdLineRenderer.SetPosition(0, startPosition);
        windSpdLineRenderer.SetPosition(1, endPosition);
        windSpdTag.transform.position = endPosition;
    }

    public void SetLandPosition(SubSwarm subSwarm, List<Pad> pads)
    {
        for (int i = 0; i < subSwarm.Drones.Count; i++)
        {
            subSwarm.Drones[i].transform.position = pads[i].transform.position;
        }
    }
}
