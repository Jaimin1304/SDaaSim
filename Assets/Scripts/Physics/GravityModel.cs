using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModel : MonoBehaviour
{
    public static GravityModel instance;
    float g0 = 9.807f; // Gravitational acceleration at the Earth's surface, in m/s²
    float earthRadius = 6371000f; // Radius of the Earth, in meters

    void Awake()
    {
        if (instance != null) // Singleton
        {
            Debug.LogError("More than one GameManager in scene!");
            return;
        }
        instance = this;
    }

    public float CalGravity(float altitude)
    {
        // Calculate gravitational acceleration at a given altitude
        float gravity = g0 * ((earthRadius - 2 * altitude) / earthRadius);
        return gravity;
    }
}
