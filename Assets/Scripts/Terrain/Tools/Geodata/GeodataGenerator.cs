using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class GeodataGenerator : MonoBehaviour {

	public static Dictionary<Vector3, Node> terrain = new Dictionary<Vector3, Node>();
	public int terrainWidth = 512;
	public int terrainHeight = 64;
	public float nodeSize = .25f;
	public float characterHeight = 2f;
	public float erosionThreshold = 1.5f;
	public bool drawGizmos = true;
	public LayerMask walkableMask;
	public LayerMask obstacleMask;
	public LayerMask allowWalkMask;
	public bool export;
	public string exportPath = "terraindata.dat";

	// Use this for initialization
	void Start () {
		Stopwatch s = new Stopwatch ();
		s.Start ();
		CreateTerrain ();
		UnityEngine.Debug.Log ("Built the terrain in " + s.ElapsedMilliseconds + " ms");
		s.Stop ();

		//obstacleMask = ~walkableMask;

		if(export) {
			GeodataExporter.Export(exportPath, terrain, terrainWidth, terrainHeight);
		}
	}

	float floorToNearest(float value, float step) {
		return step * Mathf.Floor(value / step);

	}

	void CreateTerrain() {

		Vector3 size = new Vector3 (nodeSize /2f - nodeSize / 10f, 0.1f, nodeSize/2f - nodeSize / 10f);

		for (int x = -terrainWidth / 2; x < terrainWidth / 2; x++) {
			for (int z = -terrainWidth / 2; z < terrainWidth / 2; z++) {

				float oldHeight = 0;

				RaycastHit[] hits = Physics.BoxCastAll (new Vector3 (x * nodeSize + nodeSize / 2f, 50, z * nodeSize + nodeSize / 2f)
				                    + transform.position, size, -Vector3.up, Quaternion.identity, 100f, walkableMask);
				
				UnityEngine.Profiling.Profiler.BeginSample ("Calc");
				hits = hits.OrderBy(h=>h.distance).ToArray();
				UnityEngine.Profiling.Profiler.EndSample ();

				for(int i = 0; i < hits.Length ; i++) {
					
					if (i == 0)
						oldHeight = hits [i].point.y;
					
					//Vector3 nodePos = new Vector3 (Mathf.Floor (transform.position.x) + x * nodeSize, Mathf.Floor(hits[i].point.y), Mathf.Floor (transform.position.z) + x * nodeSize);
					Vector3 nodePos = new Vector3 (floorToNearest(transform.position.x + x * nodeSize, nodeSize), floorToNearest(hits[i].point.y, nodeSize), floorToNearest(transform.position.z + z * nodeSize, nodeSize));

					if((oldHeight - hits[i].point.y) > characterHeight || i == 0) {
						Node n = new Node (nodePos, 0, 0, nodeSize);

						terrain.Add (nodePos, n);

						n.walkable = IsNodeWalkable(n.center);

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
		Vector3 size = new Vector3(nodeSize / 2f - nodeSize / 20f, nodeSize / 20f, nodeSize / 2f - nodeSize / 20f);

		bool obstacle = false;

		float verticalOffset = 3f;
		if(Physics.BoxCast(nodeCenter - Vector3.up * verticalOffset, size, Vector3.up, Quaternion.identity, characterHeight + verticalOffset, mask)) {
			obstacle = true;
		}

		float castHeight = nodeSize * 2;
		if(Physics.BoxCast(nodeCenter + Vector3.up * castHeight + Vector3.left * nodeSize / 2f, size, Vector3.right, Quaternion.identity, nodeSize, mask)) {
			return true;
		}
		if(Physics.BoxCast(nodeCenter + Vector3.up * castHeight + Vector3.right * nodeSize / 2f, size, Vector3.left, Quaternion.identity, nodeSize, mask)) {
			return true;
		}
		if(Physics.BoxCast(nodeCenter + Vector3.up * castHeight + Vector3.forward * nodeSize / 2f, size, Vector3.back, Quaternion.identity, nodeSize, mask)) {
			return true;
		}
		if(Physics.BoxCast(nodeCenter + Vector3.up * castHeight + Vector3.back * nodeSize / 2f, size, Vector3.forward, Quaternion.identity, nodeSize, mask)) {
			return true;
		}

		return obstacle;

	}



	/*public static Node GetNode(Vector3 pos) {

		pos.Set(Mathf.Floor (pos.x),  Mathf.Floor (pos.y),  Mathf.Floor (pos.z));

		for (int i = -25; i <= 15; i++) { 
			Node n;
			if (terrain.TryGetValue (pos + Vector3.up * (float)i/10, out n)) {
				return n;
			}
		}

		return null;
	}*/

	/*public static Node[] GetNodeNeighbors(Node n) {

		Node[] returnList = new Node[8];

		int i = 0;
		for (int x = -1; x <= 1; x++) {
			for (int z = -1; z <= 1; z++) {
				if (x == 0 && z == 0)
					continue;

				Node node = GetNode (new Vector3 (x, 0, z) + n.position);

				if(node != null)
					returnList[i] = (node);

				i++;
			}

		}

		return returnList;
	}*/



	void OnDrawGizmos() {

		if (!drawGizmos)
			return;

		if (terrain.Count > 0) {
			foreach (KeyValuePair<Vector3, Node> n in terrain) {
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
