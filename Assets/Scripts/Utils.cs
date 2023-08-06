using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public void Outline(Outline outline, GameObject gameObject)
    {
        // Create a ray from the mouse cursor on screen in the direction of the camera
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Check if the ray hits this Node
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.gameObject == gameObject)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
        }
    }
}
