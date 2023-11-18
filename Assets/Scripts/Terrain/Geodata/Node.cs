using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using System.IO;

[System.Serializable]
public class Node {
	public float size;
	public bool walkable;
	public Vector3 position;
	public Vector3 center;

	public Node(Vector3 position, float size) {
		this.position = position;
		this.size = size;
		center = new Vector3 (position.x + size / 2f, position.y, position.z + size / 2f);
	}
}
