using System;
using UnityEngine;

/*
The applied anergy consumption model is derived from kirchstein's approach mentioned in paper:
Comparison of energy demands of drone-based and ground-based parcel delivery services
and
Energy consumption models for delivery drones: A comparison and assessment
*/

public class KirchsteinECM : MonoBehaviour
{
    public float eta = 0.73f; // Power transfer efficiency from battery to propeller
    public float k = 1; // Lifting power markup
    public float T; // Thrust
    public float w; // Downwash coefficient
    public float va = 5; // Airspeed
    public float rho = 1.225f; // Air density
    public float[] mk = { 1.07f, 1f, 1.5f }; // Mass array for drone components
    public float[] Ak = { 0.0599f, 0.0037f, 0.0135f }; // Area array for drone components
    public float[] CDk = { 1.49f, 1f, 2.2f }; // Drag coefficient array for drone components
    public float k2 = 0.790f; // Factor for profile power (m/kg)1/2
    public float k3 = 0.0042f; // Factor for profile power associated with speed (m/kg)-1/2
    public float g = 9.807f; // Gravitational acceleration
    public float Pavio = 10f; // Avionics power, power consumption of electronic equipmentF
    public float etaC = 0.9f; // Battery charging efficiency
    public float theta = 0.9f; // Flight angle
    public float m = 3.57f; // Total mass
    public float n; // Number of rotors
    public float alpha; // Angle of attack, needs to be converted to radians
    public float varsigma; // Rotor area

    void Start()
    {
        float Epm = CalculateEpm();
        Debug.Log("Energy per meter: " + Epm);
    }

    float CalculateEpm()
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
        // Calculate downwash coefficient
        // Calculate the angle of attack alpha in radians
        float numerator = 0.5f * rho * sumCDkAk * Mathf.Pow(va, 2);
        float denominator = sumMk * g;
        float alpha = Mathf.Atan(numerator / denominator);
        // Convert alpha from radians to degrees if needed
        float alphaDegrees = alpha * Mathf.Rad2Deg;
        // Calculate downwash coefficient using attack angle
        CalculateDownwashCoefficient(out float w1, out float w2);
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

        return Epm;
    }

    void CalculateDownwashCoefficient(out float w1, out float w2)
    {
        // Convert angle alpha to radians
        float alphaRad = Mathf.Deg2Rad * alpha;
        // Calculate the coefficients of the quadratic equation
        float A = 4 * n * n * rho * rho * varsigma * varsigma - T * T;
        float B = -2 * T * T * va * Mathf.Sin(alphaRad);
        float C = -T * T * va * va;
        // Calculate the discriminant
        float discriminant = B * B - 4 * A * C;
        // Check if the discriminant is negative, if so, the equation has no real roots
        if (discriminant < 0)
        {
            Debug.LogError("The equation has no real roots.");
            w1 = w2 = float.NaN;
            return;
        }
        // Calculate the two possible values of w
        w1 = (-B + Mathf.Sqrt(discriminant)) / (2 * A);
        w2 = (-B - Mathf.Sqrt(discriminant)) / (2 * A);
    }
}
