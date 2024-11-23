using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedTerrainGrass {

	[Serializable]
	public class GrassCell {
		//[NonSerialized] 
		public int state;						// 0 -> not initialized; 1 -> queued; 2 -> readyToInitialize; 3 -> initialized
		public int index;
		public Vector3 Center;					// center in world space, needed by CullingSpheres
		// public List<int> CellContentIndexes; // = new List<int>();
		public int[] CellContentIndexes;
		public int CellContentCount; 			// = 0;
	}
}