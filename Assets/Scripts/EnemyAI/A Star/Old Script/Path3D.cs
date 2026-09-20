using UnityEngine;

public class Path3D
{
   public readonly Vector3[] lookPoints;
   public readonly Plane[] turnBoundaries;
   
   public readonly int finishLineIndex;
   public readonly float slowDownIndex;

   public Path3D(Vector3[] waypoints, Vector3 playerPosition, float turnDst, float stoppingDst)
    {
        lookPoints = waypoints;
        turnBoundaries = new Plane[lookPoints.Length];
        finishLineIndex = turnBoundaries.Length - 1;

        Vector3 previousPoint = playerPosition;
        for (int i = 0; i < lookPoints.Length; i++)
        {
            Vector3 currentPoint = lookPoints[i];
            Vector3 dirOfVector = (currentPoint - previousPoint).normalized;
            Vector3 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint: currentPoint - dirOfVector * turnDst;
            turnBoundaries[i] = new Plane(turnBoundaryPoint, dirOfVector, playerPosition);
            previousPoint = turnBoundaryPoint;
        }

        float dstFormEndPoint = 0;
        for (int i = lookPoints.Length - 1; i > 0; i--)
        {
            dstFormEndPoint += Vector3.Distance(lookPoints[i], lookPoints [i-1]);
           if (dstFormEndPoint > stoppingDst)
            {
                slowDownIndex = i;
                break;
            }
        }
    }

    public void DrawWithGizmos() {

		Gizmos.color = Color.black;
		foreach (Vector3 p in lookPoints) {
			Gizmos.DrawCube (p + Vector3.up, Vector3.one);
		}

		Gizmos.color = Color.white;
		foreach (Plane l in turnBoundaries) {
			l.DrawWithGizmos (10);
		}

	}


    
}
