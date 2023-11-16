using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDensityModel : MonoBehaviour
{
    public static AirDensityModel instance;

    // Define basic constants
    float T0 = 288.15f; // Standard temperature at sea level in Kelvin
    float P0 = 101325f; // Standard atmospheric pressure at sea level in Pascals
    float L = 0.0065f; // Temperature lapse rate in Kelvin per meter
    float R = 287.05f; // Specific gas constant for air in Joules/(kg·Kelvin)

    void Awake()
    {
        if (instance != null) // Singleton
        {
            Debug.LogError("More than one GameManager in scene!");
            return;
        }
        instance = this;
    }

    void Start()
    {
        // Test: Calculate and log the air density at sea level
        Debug.Log("Air Density at Sea Level: " + CalAirDensity(0, 9.807f) + " kg/m^3");
    }

    public float CalAirDensity(float altitude, float g)
    {
        // Calculate the air density at a given altitude
        float temperature = T0 - L * altitude; // Calculate the temperature at the current altitude
        float pressure = P0 * Mathf.Pow((temperature / T0), (g / (R * L))); // Calculate the pressure at the current altitude
        float airDensity = pressure / (R * temperature); // Calculate the air density
        return airDensity;
    }
}
