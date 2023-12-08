#if (UNITY_EDITOR) 
using System.Collections.Generic;

public class L2StaticMeshActor {
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public List<L2StaticMesh> staticMeshes { get; set; }

    override
    public string ToString() {
        return "Position: (" + x + "," + y + "," + z + ")\n" + "StatishMesh count: " + staticMeshes.Count;
    }
}
#endif
