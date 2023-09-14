using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkywayEditor : MonoBehaviour
{
    [SerializeField]
    GameObject gizmoPrefab; // Assign your 3D gizmo prefab in the inspector
    GameObject activeGizmo;
    GameObject selectedObj; // The obj that is currently selected

    void Update()
    {
        // Here we'll add code to handle obj selection and gizmo interaction
    }

    public void SelectObject(GameObject obj)
    {
        selectedObj = obj;
        if (activeGizmo != null)
        {
            Destroy(activeGizmo);
        }
        activeGizmo = Instantiate(gizmoPrefab, obj.transform.position, Quaternion.identity);
    }

    public void DeSelectObject()
    {
        selectedObj = null;
        if (activeGizmo != null)
        {
            Destroy(activeGizmo);
        }
    }
}
