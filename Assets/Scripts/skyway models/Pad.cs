using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour {
    [SerializeField] private int id;
    [SerializeField] private Node node;  // In Unity, this can be another GameObject with a Node component attached
    [SerializeField] private string status = "available";

    [SerializeField] private bool IsAvailable() {
        return status == "available";
    }

    [SerializeField] private void Occupy() {
        if (IsAvailable()) {
            status = "occupied";
        }
    }

    [SerializeField] private void Release() {
        status = "available";
    }
}