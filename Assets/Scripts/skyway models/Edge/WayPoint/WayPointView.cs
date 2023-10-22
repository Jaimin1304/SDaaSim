using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointView : MonoBehaviour, IHighlightable
{
    [SerializeField]
    Outline outline;

    void Awake()
    {
        outline = GetComponentInChildren<Outline>();
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
