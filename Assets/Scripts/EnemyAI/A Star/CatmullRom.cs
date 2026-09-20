using System;
using UnityEngine;

public class CatmullRom
{
    //tension = some value greater than 0, defaulting to 1
    [Range(0.1f, 1f)]
    public float tention;

    //points = a list of at least 4 coordinates
    public Vector3[] lookPoints;
    public Vector3[] finalPoints;
    public Vector3 playerPosition;
    int resolution;

    public CatmullRom(Vector3[] lookPoints, float tention, int resolution)
    {
        this.lookPoints = lookPoints;
        this.tention = tention;
        this.resolution = resolution;
        

        finalPoints = CatmullRomCurve(tention, lookPoints, resolution);
    }

    public static Vector3[] CatmullRomCurve(float tention, Vector3[] p, int resolution)
    {

        float t = 0f;
        int totalIndex = 0;

        Vector3[] finalpoints = new Vector3[p.Length + 2];

        // generate first and last tengent
        Vector3 firstTengent = p[0] + (p[0] - p[1]);
        Vector3 lastTengent = p[p.Length - 1] + (p[p.Length - 1] - p[p.Length - 2]);
        finalpoints[0] = firstTengent;
        finalpoints[finalpoints.Length - 1] = lastTengent;

        for (int i = 0; i <= p.Length - 1; i++)
        {
            finalpoints[i + 1] = p[i];
        }

        int segmentCount = finalpoints.Length - 3;
        int steps = segmentCount * (resolution);
        Vector3[] Curve = new Vector3[steps];

        for (int i = 1; i <= finalpoints.Length - 3; i++)
        {

            Vector3 p0 = finalpoints[i - 1];
            Vector3 p1 = finalpoints[i];
            Vector3 v1 = p1;
            Vector3 p2 = finalpoints[i + 1];
            Vector3 v2 = p2;
            Vector3 p3 = finalpoints[i + 2];

            float s = 2 * tention;
            Vector3 dv1 = (p2 - p0) / s;
            Vector3 dv2 = (p3 - p1) / s;

            float k = (float)t * resolution;
            int index = (int)k;


            for (index = 0; index < resolution; index++)
            {
                t = (float)index / resolution;

                float t2 = t * t;
                float t3 = t2 * t;

                float c0 = 2 * t3 - 3 * t2 + 1;
                float c1 = t3 - 2 * t2 + t;
                float c2 = -2 * t3 + 3 * t2;
                float c3 = t3 - t2;

                Curve[totalIndex] = c0 * v1 + c1 * dv1 + c2 * v2 + c3 * dv2;
                totalIndex++;
            }
        }
        return Curve;
    }
}

