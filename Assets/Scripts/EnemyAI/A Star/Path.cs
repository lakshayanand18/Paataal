using UnityEngine;

public class Path
{
    
    public readonly Vector3[] lookpoints;
    public readonly Plane[] planes;

    public readonly int finalpoint;
    public Vector3 enemyPosition;
    public float tention;
    public int resolution;

    public Path(Vector3[] curveWaypoints,  Vector3 enemyPosition) //float tention, int resolution,
    {
        lookpoints = curveWaypoints;
        //this.tention = tention;
        //this.resolution = resolution;
        this.enemyPosition = enemyPosition;

        planes = new Plane[curveWaypoints.Length];
        finalpoint = planes.Length - 1;

        Vector3 dir = (curveWaypoints[1] - curveWaypoints[0]).normalized;
        planes[0] = new Plane(curveWaypoints[0], dir, enemyPosition);

        for (int i = 1; i < curveWaypoints.Length; i++)
        {
            Vector3 dirOfVector = (curveWaypoints[i] - curveWaypoints[i - 1]).normalized;
            planes[i] = new Plane(curveWaypoints[i], dirOfVector, enemyPosition);
        }
    }

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach (var waypoint in lookpoints)
            Gizmos.DrawSphere(waypoint, 5f);
        
        //Gizmos.color = Color.red;
        //foreach (Vector3 p in curvePath.finalPoints)
        //    Gizmos.DrawSphere(p, 5f);

        Gizmos.color = Color.white;
        foreach (Plane l in planes)
        {
            l.DrawWithGizmos(10);
        }
    }
}
