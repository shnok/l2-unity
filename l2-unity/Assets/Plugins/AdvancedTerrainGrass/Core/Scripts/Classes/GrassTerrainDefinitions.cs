using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AdvancedTerrainGrass {
	[PreferBinarySerialization]
	[Serializable]
	public class GrassTerrainDefinitions : ScriptableObject {

		[Header("Serialized Grass Data")]
		public Vector3 TerrainPosition;
		[SerializeField] public List <DetailLayerMap> DensityMaps;
		public GrassCell[] Cells;
		public GrassCellContent[] CellContent;
		public int maxBucketDensity = 0;
		public int[] LayersMaxDensity;
	}

//	Members
	[PreferBinarySerialization]
	[Serializable] public class DetailLayerMap {
		[SerializeField] public byte[] mapByte;
	}
}
