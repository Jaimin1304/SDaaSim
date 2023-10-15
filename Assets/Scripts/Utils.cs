using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static bool HasEdgeBetween(Node node1, Node node2)
    {
        return node1.HasEdgeTo(node2) || node2.HasEdgeTo(node1);
    }

    public static float[] SolveQuadratic(float A, float B, float C)
    {
        float discriminant = B * B - 4 * A * C;
        if (discriminant < 0)
        {
            Debug.LogError("No real roots!");
            return null;
        }
        float root1 = (-B + Mathf.Sqrt(discriminant)) / (2 * A);
        float root2 = (-B - Mathf.Sqrt(discriminant)) / (2 * A);
        return new float[] { root1, root2 };
    }
}
