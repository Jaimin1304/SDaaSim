using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour {
    public int id;
    public Node node;  // In Unity, this can be another GameObject with a Node component attached
    public string status = "available";

    public bool IsAvailable() {
        return status == "available";
    }

    public void Occupy() {
        if (IsAvailable()) {
            status = "occupied";
        }
    }

    public void Release() {
        status = "available";
    }
}