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

    void CalculateDownwashCoefficient(out List<float> wSolutions)
    {
        Debug.Log("w cal start");
        // Define ν (nu) and R (rotor disc area)
        float nu = va; // Assuming ν is the airspeed va, adjust as needed based on actual scenario
        float R = n * varsigma; // Assuming R is n times the spinning area of one rotor, adjust as needed based on actual scenario

        // Define the function representing the quartic equation and its derivative
        Func<float, float> f = w =>
            w * w * (w * w - 2 * w * nu * Mathf.Sin(alpha) + nu * nu)
            - (T * T) / (4 * rho * rho * R * R);
        Func<float, float> df = w =>
            4 * w * (w * w - 2 * w * nu * Mathf.Sin(alpha) + nu * nu)
            + 2 * w * w * (2 * w - 2 * nu * Mathf.Sin(alpha));

        // Initialize the list of solutions
        wSolutions = new List<float>();

        // Define the initial guess and tolerance
        float initialGuess = 1.0f; // Adjust as needed
        float tolerance = 0.01f;

        // Implement Newton's method
        float w = initialGuess;
        for (int i = 0; i < 10000; i++) // Limit the number of iterations to avoid infinite loop
        {
            float wNext = w - f(w) / df(w);
            if (Mathf.Abs(wNext - w) < tolerance)
            {
                wSolutions.Add(wNext);
                Debug.Log("new w added");
                break;
            }
            w = wNext;
        }
        // Check the found solutions and determine which one is physically feasible based on the actual scenario
    }

    //void CalculateDownwashCoefficient(out float[] wSolutions)
    //{
    //    // Define ν (nu) and R (rotor disc area)
    //    float nu = va; // Assuming ν is the airspeed va, adjust as needed based on actual scenario
    //    float R = n * varsigma; // Assuming R is n times the spinning area of one rotor, adjust as needed based on actual //scenario
    //
    //    // Calculate the coefficients of the equation
    //    float A = (nu * Mathf.Sin(alpha)) * (nu * Mathf.Sin(alpha));
    //    float B = 2 * (nu * Mathf.Sin(alpha)) * (nu * Mathf.Cos(alpha));
    //    float C = (nu * Mathf.Cos(alpha)) * (nu * Mathf.Cos(alpha));
    //    float D = -(T * T) / (4 * rho * rho * R * R);
    //
    //    // Use numerical methods to solve the quartic equation, such as Newton's method, Durand-Kerner method, etc.
    //    // This might require support from some mathematical libraries, or you can implement your own solver
    //    // wSolutions = SolveQuarticEquation(A, B, C, D);
    //
    //    // Here, wSolutions should contain all the roots of the quartic equation, and you need to determine which one is //physically feasible based on the actual scenario
    //}
}
