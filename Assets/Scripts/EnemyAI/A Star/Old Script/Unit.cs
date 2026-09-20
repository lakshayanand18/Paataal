//﻿using UnityEngine;
//using System.Collections;

//public class Unit : MonoBehaviour {
//	const float minPathUpdateTime = .2f;
//	const float pathUpdateMoveThreshold = .5f;
//	public Transform target;
//	public float speed = 20;
//	public float turnSpeed = 3;
//	public float turnDst = 5;
//	public float stoppingDst = 10;

//	[Range(0f, 1f)]
//	public float tention = .5f;
//	public int resolution = 20; 

//	Path path;

//    void Start()
//    {
//        StartCoroutine(UpdatePath());
//    }
//    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) 
//	{
//		if (pathSuccessful) {

//			path = new Path(waypoints, tention, resolution, transform.position);

//			StopCoroutine(FollowPath());
//			StartCoroutine(FollowPath());
//		}
//	}

//	IEnumerator UpdatePath() 
//	{
//		if (Time.timeSinceLevelLoad < .3f) 
//		{
//			yield return new WaitForSeconds(.3f);
//		}

//		PathRequestManager.RequestPath(new PathRequest(transform.position,target.position, OnPathFound));
		
//		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
//		Vector3 targetPosOld = target.position;

//		while (true) 
//		{
//			yield return new WaitForSeconds(minPathUpdateTime);
//			if((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
//				targetPosOld = target.position;
//				PathRequestManager.RequestPath(new PathRequest(transform.position,target.position, OnPathFound));
//			}
//		}
//	}

//	IEnumerator FollowPath()
//	{
//		bool followingPath = true;
//		int pathIndex = 0;
//		while (followingPath)
//		{
//			Vector3 currentIndex = path.curvePath.finalPoints[pathIndex];
//			Vector3 direction = (currentIndex - transform.position).normalized;
//			if (Vector3.Distance(currentIndex, transform.position) < 0.4f)
//			{
//				if(pathIndex == path.curvePath.finalPoints.Length - 1)
//				{
//					followingPath = false;
//					break;
//				}

//				else
//				{
//					pathIndex++;
//				}
//			}
//			Quaternion targetRotation = Quaternion.LookRotation(direction);
//			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, .1f);
//			transform.Translate(direction * speed * Time.deltaTime, Space.World);

//            yield return null;
//		}
//	}

//	public void OnDrawGizmos()
//	{
//		if (path != null)
//		{
//			path.DrawWithGizmos();
//		}
//	}
//}