﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
	public bool walkable;
	public bool occupied;
	public Vector3 worldPosition;

	public int gridX;
	public int gridY;
	public int gCost;
	public int hCost;
	public Node parent;
	int heapIndex;

	public Node(bool _walkable, bool _occupied, Vector3 _worldPos, int _gridX, int _gridY)
	{
		occupied = _occupied;
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get { 
			return heapIndex;
		}
		set { 
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare)
	{
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0)
		{
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
