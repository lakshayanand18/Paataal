using UnityEngine;

public struct Plane 
{
    Vector3 normalVector;           // n
    Vector3 pointOnThePlane;        // a
    bool approachSide;

    public Plane(Vector3 vectorCrosingThePlane, Vector3 normalVector, Vector3 characterPosition)
    {
        pointOnThePlane = vectorCrosingThePlane;
        this.normalVector = normalVector.normalized;
        approachSide = false;
        approachSide = GetSide(characterPosition);
    }

    bool GetSide(Vector3 p)
    {
        return Vector3.Dot(p - pointOnThePlane, normalVector) > 0;                     //ap . n
    }

    public bool HasCrossedPlane(Vector3 p)
    {
        return GetSide(p) != approachSide;
    }

    public void DrawWithGizmos(float size){
        Vector3 axisA = Vector3.Cross(normalVector, Vector3.up);
        if (axisA.sqrMagnitude < 0.001f)
            axisA = Vector3.Cross(normalVector, Vector3.right);

        axisA.Normalize();
        Vector3 axisB = Vector3.Cross(normalVector, axisA);

        // Draw plane cross
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pointOnThePlane - axisA * size, pointOnThePlane + axisA * size);
        Gizmos.DrawLine(pointOnThePlane - axisB * size, pointOnThePlane + axisB * size);
        // Draw normal vector
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pointOnThePlane, pointOnThePlane + normalVector * size);
    }
}