using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using UnityEngine.Splines;
public class Pathfinding : MonoBehaviour 
{
	Grid3D grid;

    void Awake()
    {
        grid = GetComponent<Grid3D>();
    }
    //finds the path between two points in 3D space
    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        Vector3[] curvePath = new Vector3[0];
        bool pathSuccess = false;
        Node3D startNode = grid.NodeFromWorldPoint(request.pathStart);
        Node3D targetNode = grid.NodeFromWorldPoint(request.pathEnd);
        startNode.parent = startNode;

        if(startNode.walkable3D && targetNode.walkable3D)
        {
            Heap<Node3D> openSet = new Heap<Node3D>(grid.MaxSize);          // nodes to be evaluated, node is open
            HashSet<Node3D> closedSet = new HashSet<Node3D>();              // nodes already evaluated, node is closed
            openSet.Add(startNode);

            while (openSet.Count > 0)                                       //while there are nodes to be evaluated
            {
                Node3D currentNode = openSet.RemoveFirst();                 //get the node with the lowest fCost
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    // print("path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node3D neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable3D || closedSet.Contains(neighbour))
                    {  
                        //skip if the node is not walkable or is already in closed set
                        continue;
                    }

                    int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }
        callback(new PathResult(waypoints, pathSuccess, request.callback));
    }

    Vector3[] RetracePath(Node3D startNode, Node3D endNode)
    {
        List<Node3D> path = new List<Node3D>();
        Node3D currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node3D> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        
        Vector3 directionOld = Vector3.zero;
        
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 directionNew = new Vector3(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY, path[i - 1].gridZ - path[i].gridZ);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        waypoints.Add(path[path.Count - 1].worldPosition);
        return waypoints.ToArray();
    }
    

	int GetDistance(Node3D nodeA, Node3D nodeB) {  //calculate distance between two nodes in 3D space
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int dstZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

		int min = Mathf.Min(dstX, Mathf.Min(dstY, dstZ));
        int max = Mathf.Max(dstX, Mathf.Max(dstY, dstZ));
        int mid = dstX + dstY + dstZ - min - max;

        return 17 * min + 14 * (mid - min) + 10 * (max - mid);
	}
}






























// Vector3[] simplified = SimplifyPath(path);
        // Vector3[] smoothed = SmoothPath(simplified);
        // return smoothed;




//     Vector3[] SmoothPath(Vector3[] waypoints)
// {
//     if (waypoints.Length < 2)
//         return waypoints;

//     List<Vector3> smoothPath = new List<Vector3>();
//     smoothPath.Add(waypoints[0]);
//     int currentIndex = 0;

//     while (currentIndex < waypoints.Length - 1)
//     {
//         int nextIndex = waypoints.Length - 1;

//         // Go as far forward as possible without hitting obstacles
//         for (int i = waypoints.Length - 1; i > currentIndex; i--)
//         {
//             if (!Physics.Linecast(waypoints[currentIndex], waypoints[i], grid.unwalkableMask3D))
//             {
//                 nextIndex = i;
//                 break;
//             }
//         }

//         smoothPath.Add(waypoints[nextIndex]);
//         currentIndex = nextIndex;
//     }
//     smoothPath.Reverse();
//     return smoothPath.ToArray();
// }
