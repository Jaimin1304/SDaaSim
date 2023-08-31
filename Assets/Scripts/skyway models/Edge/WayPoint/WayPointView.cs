using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointView : MonoBehaviour, Highlightable
{
    [SerializeField]
    Outline outline;

    void Awake()
    {
        outline = GetComponentInChildren<Outline>();
    }

    public void Highlight()
    {
        outline.enabled = true;
    }

    public void Unhighlight()
    {
        outline.enabled = false;
    }
}
