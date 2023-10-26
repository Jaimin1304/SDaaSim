using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DroneView : MonoBehaviour, IHighlightable
{
    [SerializeField]
    Outline outline;

    [SerializeField]
    TextMeshPro nameTag;

    Camera mainCamera;

    // Define a maximum distance for visibility
    [SerializeField]
    private float maxVisibleDistance = 20f; // Set this value based on your needs

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void initVisual(Drone drone)
    {
        // set name tag
        nameTag.text = string.Format(
            "{0} {1}Wh/{2}Wh ({3}%)",
            drone.name,
            drone.CurrBatteryWh,
            drone.BatteryCapacityWh,
            drone.BatteryStatus * 100
        );
        foreach (Payload payload in drone.Payloads)
        {
            nameTag.text += string.Format(" - {0}({1}kg)", payload.name, payload.Weight.ToString());
        }
    }

    public void UpdateVisual(Drone drone)
    {
        // Calculate distance from the camera
        float distance = Vector3.Distance(
            nameTag.transform.position,
            mainCamera.transform.position
        );
        // Check if distance is greater than the threshold
        if (distance > maxVisibleDistance)
        {
            nameTag.gameObject.SetActive(false);
            return; // Return early so the rest of the code doesn't execute
        }
        else
        {
            nameTag.gameObject.SetActive(true);
        }
        // Let the name tag face the camera
        nameTag.transform.rotation = mainCamera.transform.rotation;
        // Scale text size based on distance
        float scaleValue = distance * Globals.textScaleValue;
        nameTag.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        // set name tag
        nameTag.text = string.Format(
            "{0} {1}Wh/{2}Wh ({3}%)",
            drone.name,
            drone.CurrBatteryWh,
            drone.BatteryCapacityWh,
            drone.BatteryStatus * 100
        );
        foreach (Payload payload in drone.Payloads)
        {
            nameTag.text += string.Format(" - {0}({1}kg)", payload.name, payload.Weight.ToString());
        }
    }

    public void Highlight()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void Unhighlight()
    {
        if (outline != null)
            outline.enabled = false;
    }
}
