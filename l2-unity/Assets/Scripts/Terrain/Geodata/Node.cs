using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using System.IO;

[System.Serializable]
public class Node {
	public Vector3 nodeIndex;
	public Vector3 worldPosition;
	public Vector3 center;

	public Node parentNode;
	public float cost;

	public Node() {}

	public Node(Node original) {
		this.nodeIndex = original.nodeIndex;
		this.worldPosition = original.worldPosition;
		this.center = original.center;
		this.parentNode = original.parentNode;
		this.cost = original.cost;
	}

	public Node(Vector3 nodeIndex, Vector3 worldPosition, float size) {
		this.nodeIndex = nodeIndex;
		this.worldPosition = worldPosition;
		center = new Vector3 (worldPosition.x + size / 2f, worldPosition.y, worldPosition.z + size / 2f);
	}
}
