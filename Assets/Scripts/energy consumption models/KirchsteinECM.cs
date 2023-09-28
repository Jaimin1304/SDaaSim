using System;
using UnityEngine;
using System.Collections.Generic;

/*
The applied anergy consumption model is derived from kirchstein's approach mentioned in paper:
Comparison of energy demands of drone-based and ground-based parcel delivery services
and
Energy consumption models for delivery drones: A comparison and assessment
*/

public class KirchsteinECM : MonoBehaviour
{
    public static KirchsteinECM instance;
    public float eta = 0.73f; // Power transfer efficiency from battery to propeller
    public float k = 1; // Lifting power markup
    public float T; // Thrust
    public float w = 1f; // Downwash coefficient
    public float va = 1; // Airspeed
    public float rho = 1.225f; // Air density
    public float[] mk = { 1.07f, 1f, 1.5f }; // Mass array for drone components
    public float[] Ak = { 0.0599f, 0.0037f, 0.0135f }; // Area array for drone components
    public float[] CDk = { 1.49f, 1f, 2.2f }; // Drag coefficient array for drone components
    public float k2 = 0.790f; // Factor for profile power (m/kg)1/2
    public float k3 = 0.0042f; // Factor for profile power associated with speed (m/kg)-1/2
    public float g = 9.807f; // Gravitational acceleration
    public float Pavio = 10f; // Avionics power, power consumption of electronic equipmentF
    public float etaC = 0.9f; // Battery charging efficiency
    public float theta = 0f; // Flight angle
    public float m = 3.57f; // Total mass
    public float n = 4; // Number of rotors
    public float alpha; // Angle of attack in radians
    public float varsigma = 0.0507f; // Spinning area of one rotor [m2]

    void Start()
    {
        // test different va
        float[] vaValues = { 0.3f, 5, 10, 15, 25 };
        foreach (float vaValue in vaValues)
        {
            va = vaValue; // set va
            float Epm = CalculateEpm();
            Debug.Log("For va = " + va + ", Energy per meter: " + Epm);
        }
    }

    public float CalculateEpm()
    {
        float sumCDkAk = 0;
        float sumMk = 0;
        for (int k = 0; k < 3; k++)
        {
            sumCDkAk += CDk[k] * Ak[k];
            sumMk += mk[k];
        }

        T = Mathf.Sqrt(
            Mathf.Pow(g * sumMk, 2)
                + Mathf.Pow(0.5f * rho * sumCDkAk * Mathf.Pow(va, 2), 2)
                + rho * sumCDkAk * Mathf.Pow(va, 2) * m * g * (float)Math.Sin((float)theta)
        );
        Debug.Log(String.Format("Thrust T: {0}", T));
        // Calculate downwash coefficient
        // Calculate the angle of attack alpha in radians
        float numerator = 0.5f * rho * sumCDkAk * Mathf.Pow(va, 2);
        float denominator = sumMk * g;
        alpha = Mathf.Atan(numerator / denominator);
        Debug.Log(String.Format("Angle of attack Alpha: {0}", alpha));
        // Calculate downwash coefficient using attack angle
        // CalculateDownwashCoefficient(out List<float> wSolutions);
        // Debug.Log("Possible w: " + string.Join(", ", wSolutions));
        // Calculate Epm
        float Epm =
            (1 / eta)
                * (
                    (k * T * w / va)
                    + (0.5f * rho * sumCDkAk * Mathf.Pow(va, 2))
                    + (k2 * Mathf.Pow(g * sumMk, 1.5f) / va)
                    + (k3 * Mathf.Sqrt(g * sumMk) * va)
                )
            + (Pavio / (etaC * va));
        //Debug.Log(String.Format("Joule per meter Epm: {0}", Epm));
        return Epm;
    }
}
