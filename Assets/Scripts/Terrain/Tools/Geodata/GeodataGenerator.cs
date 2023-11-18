﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GeodataGenerator : MonoBehaviour {
	public static Dictionary<Vector3, Node> terrain = new Dictionary<Vector3, Node>();
	public Transform terrainTransform;
	public int terrainWidth = 512;
	public int scaledTerrainWidth;
	public float nodeSize = .25f;
	public float characterHeight = 2f;
	public float erosionThreshold = 1.5f;
	public bool drawGizmos = true;
	public LayerMask walkableMask;
	public LayerMask obstacleMask;
	public LayerMask allowWalkMask;
	public bool export;
	public string exportPath = "terraindata.dat";

	void Start() {
		scaledTerrainWidth = (int)(terrainWidth / nodeSize);

        System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
		s.Start();
		GenerateGeodata();
		s.Stop();

		Debug.Log("Built the terrain in " + s.ElapsedMilliseconds + " ms");

		if(export) {
			GeodataExporter.Export(exportPath, terrain);
		}
	}

	void GenerateGeodata() {
		Vector3 roundedTerrainPos = VectorUtils.floorToNearest(terrainTransform.position, nodeSize);
		Debug.Log(roundedTerrainPos);
		for (int x = 0; x < scaledTerrainWidth; x++) {
			for (int z = 0; z < scaledTerrainWidth; z++) {

				float oldHeight = 0;

				Vector3 size = new Vector3(nodeSize / 2f - nodeSize / 10f, 0.1f, nodeSize / 2f - nodeSize / 10f);
				Vector3 origin = new Vector3(x * nodeSize + nodeSize / 2f, 750f, z * nodeSize + nodeSize / 2f);
				origin = origin + new Vector3(roundedTerrainPos.x, 0, roundedTerrainPos.z);
				RaycastHit[] hits = Physics.BoxCastAll (origin, size, Vector3.down, Quaternion.identity, 1000f, walkableMask);

				hits = hits.OrderBy(h=>h.distance).ToArray();

				for(int i = 0; i < hits.Length ; i++) {
					if (i == 0)
						oldHeight = hits [i].point.y;

					float y = hits[i].point.y - roundedTerrainPos.y;
					Vector3 nodePos = new Vector3 (x * nodeSize, y, z * nodeSize);
					nodePos = VectorUtils.floorToNearest(nodePos, nodeSize);

					if((oldHeight - hits[i].point.y) > characterHeight || i == 0) {
						Node n = new Node (nodePos + roundedTerrainPos, nodeSize);
						n.walkable = IsNodeWalkable(n.center);

						if(n.walkable) {
							Vector3 scaledNodePos = new Vector3(x, nodePos.y / nodeSize, z);
							terrain.Add (scaledNodePos, n);
						}

						oldHeight = hits [i].point.y;
					}
				}
			}
		}
	}

	public bool IsNodeWalkable(Vector3 nodeCenter) {
		bool walkable = true;

		/* Covered */
		if(CheckObstacle(nodeCenter, obstacleMask, characterHeight)) {
			walkable = false;
		}

		if(CheckObstacle(nodeCenter, allowWalkMask, characterHeight)) {
			walkable = true;
		}

		/* Erosion */
		if(CheckErosion(nodeCenter, erosionThreshold)) {
			walkable = false;
		}

		return walkable;
	}

	private bool CheckErosion(Vector3 nodeCenter, float erosionThreshold) {

		RaycastHit hit;
		for(int x = -1; x <= 1; x++) {
			for(int z = -1; z <= 1; z++) {
				if(!Physics.Raycast(nodeCenter + new Vector3(x * nodeSize, 0, z * nodeSize) + Vector3.up, Vector3.down, out hit, erosionThreshold + 1)) {
					return true;
				}
			}
		}
		return false;
	}

	private bool CheckObstacle(Vector3 nodeCenter, LayerMask mask, float characterHeight) {
		/* Vertical cast */
		Vector3 size = new Vector3(nodeSize / 2f - nodeSize / 20f, nodeSize / 20f, nodeSize / 2f - nodeSize / 20f);
		bool obstacle = false;
		float verticalOffset = 3f;
		if(Physics.BoxCast(nodeCenter - Vector3.up * verticalOffset, size, Vector3.up, Quaternion.identity, characterHeight + verticalOffset, mask)) {
			//obstacle = true;
		}

		/* Horizontal cast */
		float castHeight = nodeSize * 1.25f;
		float castOffset = nodeSize * 0.4f;
		float castLength = nodeSize* 0.8f;

		Vector3 origin = nodeCenter + Vector3.up * castHeight;
		if(Physics.Linecast(origin + Vector3.left * castOffset, origin + Vector3.right * castLength, mask)) {
			return true;
		}
		if(Physics.Linecast(origin + Vector3.right * castOffset, origin + Vector3.left * castLength, mask)) {
			return true;
		}
		if(Physics.Linecast(origin + Vector3.forward * castOffset, origin + Vector3.back * castLength, mask)) {
			return true;
		}
		if(Physics.Linecast(origin + Vector3.back * castOffset, origin + Vector3.forward * castLength, mask)) {
			return true;
		}

		return obstacle;
	}



	/*int Flatten(int width, int height, int x, int y, int z) {
		return (y * height * width) + (z * width) + x;
	}

	public Vector3 UnFlatten(int index) {
		float y = (float)index / (scaledTerrainHeight * scaledTerrainWidth);
		float vy = (float)index % (scaledTerrainHeight * scaledTerrainWidth);
		float z = vy / (scaledTerrainWidth);
		float x = vy % (scaledTerrainWidth);
		
		return new Vector3(Mathf.Round(x), Mathf.Round(y), Mathf.Floor(z));
	}*/

	void OnDrawGizmos() {

		if (!drawGizmos || !Application.isPlaying)
			return;

		int count = 0;
		if(terrain.Count > 0) {
			foreach(KeyValuePair<Vector3, Node> n in terrain) {
				if(count >= 250000) {
					return;
				}
				Vector3 cubeSize = new Vector3 (nodeSize - nodeSize/10f, 0.1f, nodeSize - nodeSize / 10f);
				if (n.Value.walkable) {
					Gizmos.color = Color.green;
				} else {
					Gizmos.color = Color.red;
				}

				Gizmos.DrawCube (n.Value.center, cubeSize);
			}
		}

	}
}