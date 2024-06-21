#if (UNITY_EDITOR) 
using UnityEngine;

[System.Serializable]
public class Brush {
    public string name;
    public string csgOper;
    public string group;
    public string[] polyFlags;
    public Vector3 mainScale;
    public Vector3 postScale;
    public Vector3 position;
    public Vector3 prePivot;
    public Model model;

    public override string ToString() {
        return $"Name: {name}, csgOper: {csgOper}, group: {group}, mainScale: {mainScale}";
    }
}
#endif