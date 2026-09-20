using UnityEngine;

public class Node3D : IHeapItem<Node3D>	
{
    public bool walkable3D;
	public Vector3 worldPosition;

	public int gridX;
	public int gridY;
	public int gridZ;
	public int movementPenalty;
	public int gCost;
	public int hCost;
	public Node3D parent;
	int heapIndex;
	public Node3D(bool _walkable3D, Vector3 _worldPos, int _gridX, int _gridY, int _gridZ , int _movementPenalty)
	{
		walkable3D = _walkable3D;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		gridZ = _gridZ;
		movementPenalty = _movementPenalty;
	}

	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}
	public int HeapIndex
	{
		get
		{
			return heapIndex;
		}
		set
		{
			heapIndex = value;
		}
	}	

	public int CompareTo(Node3D nodeToCompare)
	{
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0)
		{
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
