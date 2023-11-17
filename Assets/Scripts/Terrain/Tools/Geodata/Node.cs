using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using System.IO;

[System.Serializable]
public class Node  : ICloneable{

	public float nodeSize;
	public bool walkable;
	public Vector3 position;
	public Vector3 center;
	public int gridX, gridY;

	int heapIndex;

	//g price //h distance //f price
	public float g = 0, h = 0, f = 0;
	public Node parentNode;

	public Node(Vector3 position, int x, int y, float nodeSize) {
		this.gridX = x;
		this.gridY = y;
		this.position = position;
		this.nodeSize = nodeSize;
		this.center = new Vector3 (position.x + nodeSize / 2f, position.y, position.z + nodeSize / 2f);
		//GameObject g = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Cube.prefab");
		//GameObject h = GameObject.Instantiate(g);
		//h.transform.position = center;
		
	}

	public object Clone()
	{
		return this.MemberwiseClone();
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = f.CompareTo(nodeToCompare.f);
		if (compare == 0) {
			compare = h.CompareTo(nodeToCompare.h);
		}
		return -compare;
	}
}
