using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/*
The applied anergy consumption model is derived from kirchstein's approach mentioned in paper:
Comparison of energy demands of drone-based and ground-based parcel delivery services
and
Energy consumption models for delivery drones: A comparison and assessment
DOI:10.1016/j.trd.2019.102209
*/

public class KirchsteinECM : MonoBehaviour
{
    public static KirchsteinECM instance;
    public float eta; // Power transfer efficiency from battery to propeller
    public float T; // Thrust
    public float w; // Downwash coefficient
    public float va = 1; // Airspeed
    public float rho; // Air density
    public float[] mk = new float[3]; // Mass array for drone components
    public float[] Ak = new float[3]; // Area array for drone components
    public float[] CDk = new float[3]; // Drag coefficient array for drone components
    public float k; // Factor for induced power [unitless]
    public float k2; // Factor for profile power (m/kg)1/2
    public float k3; // Factor for profile power associated with speed (m/kg)-1/2
    public float g; // Gravitational acceleration
    public float Pavio; // Avionics power, power consumption of electronic equipmentF
    public float etaC; // Battery charging efficiency
    public float theta = 0f; // Flight angle
    public float m; // Total mass
    public float n; // Number of rotors
    public float alpha; // Angle of attack in radians
    public float varsigma = 0.0507f; // Spinning area of one rotor [m2]

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
        eta = Globals.PwrXferEffic;
        w = Globals.DownwashCoeff;
        rho = Globals.AirDensity;

        mk[0] = Globals.BodyMass;
        mk[1] = Globals.BatMass;
        mk[2] = Globals.PayloadMass;

        Ak[0] = Globals.BodyArea;
        Ak[1] = Globals.BatArea;
        Ak[2] = Globals.PayloadArea;

        CDk[0] = Globals.BodyDragCoeff;
        CDk[1] = Globals.BatDragCoeff;
        CDk[2] = Globals.PayloadDragCoeff;

        n = Globals.RotorNum;
        etaC = Globals.BatChargingEffic;
        m = mk.Sum();

        k = Globals.InducedPowFactor;
        k2 = Globals.ProfilePowFactor;
        k3 = Globals.ProfilePowWithSpdFactor;
        g = Globals.g0;
        Pavio = Globals.AvionicsPwr;

        // test different va
        float[] vaValues = { 0.3f, 5, 10, 15, 20, 25 };
        foreach (float vaValue in vaValues)
        {
            va = vaValue; // set va
            float Epm = CalEpm(va, 0, 9.807f, 1.225f, 1f);
            Debug.Log("For va = " + va + ", Energy per meter: " + Epm);
        }
    }

    public float CalEpm(float va, float theta, float g, float rho, float payloadWeight)
    {
        Debug.Log(
            String.Format(
                "va: {0}, theta: {1}, g: {2}, rho: {3}, payloadWeight: {4}",
                va,
                theta,
                g,
                rho,
                payloadWeight
            )
        );
        // assign payload weight
        mk[2] = payloadWeight;

        float sumCDkAk = 0;
        float sumMk = 0;
        for (int k = 0; k < 3; k++)
        {
            sumCDkAk += CDk[k] * Ak[k];
            sumMk += mk[k];
        }
        // Calculate thrust
        T = Mathf.Sqrt(
            Mathf.Pow(g * sumMk, 2)
                + Mathf.Pow(0.5f * rho * sumCDkAk * Mathf.Pow(va, 2), 2)
                + rho * sumCDkAk * Mathf.Pow(va, 2) * m * g * (float)Math.Sin((float)theta)
        );
        //Debug.Log(String.Format("Thrust T: {0}", T));
        // Calculate the angle of attack alpha in radians
        float numerator = 0.5f * rho * sumCDkAk * Mathf.Pow(va, 2);
        float denominator = sumMk * g;
        alpha = Mathf.Atan(numerator / denominator);
        //Debug.Log(String.Format("Angle of attack Alpha: {0}", alpha));
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
