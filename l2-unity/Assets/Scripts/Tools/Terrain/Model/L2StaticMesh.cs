#if (UNITY_EDITOR) 
using UnityEngine;

public class L2StaticMesh {
    public string staticMesh { get; set; }
    public string actorClass { get; set; }
    public float y { get; set; }
    public float x { get; set; }
    public float z { get; set; }
    //public int yaw { get; set; }
    //public int roll { get; set; }
    //public int pitch { get; set; }
    public Vector3 eulerAngles;
    public float scale { get; set; }
    public float scaleX { get; set; }
    public float scaleY { get; set; }
    public float scaleZ { get; set; }
}
#endif
