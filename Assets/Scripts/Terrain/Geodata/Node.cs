using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using System.IO;

[System.Serializable]
public class Node {
	public int x;
	public int y;
	public int z;

	public float size;
	public bool walkable;
	public Vector3 position;
	public Vector3 center;

	public Node parentNode;
	public float fCost;
	public float gCost;
	public float hCost;

	public Node() {}

	public Node(Vector3 scaledPosition, Vector3 position, float size) {
		x = (int)scaledPosition.x;
		y = (int)scaledPosition.y;
		z = (int)scaledPosition.z;
		this.position = position;
		this.size = size;
		center = new Vector3 (position.x + size / 2f, position.y, position.z + size / 2f);
	}
}
