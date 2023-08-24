using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NodeView : MonoBehaviour
{
    [SerializeField]
    TextMeshPro nameTag;

    Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void initVisual(Node node)
    {
        nameTag.text = string.Format("{0} [{1}/{2}]", node.name, node.LandedDrones.Count, node.Capacity);
    }

    public void UpdateVisual(Node node)
    {
        // Let the name tag face the camera
        nameTag.transform.rotation = mainCamera.transform.rotation;
        // Calculate distance from the camera
        float distance = Vector3.Distance(
            nameTag.transform.position,
            mainCamera.transform.position
        );
        // Scale text size based on distance
        float scaleValue = distance * 0.03f;
        nameTag.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        nameTag.text = string.Format("{0} [{1}/{2}]", node.name, node.LandedDrones.Count, node.Capacity);
    }
}
