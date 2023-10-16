using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2StaticMeshActor
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public List<L2StaticMesh> staticMeshes { get; set; }

    override
    public string ToString() {
        return "Position: (" + x + "," + y + "," + z + ")\n" + "StatishMesh count: " + staticMeshes.Count;
    }
}
