using UnityEngine;

public class SkinnedMeshSync : MonoBehaviour {
    [SerializeField] private SkinnedMeshRenderer _rootSkinnedRenderer;
    [SerializeField] private SkinnedMeshRenderer[] _destSkinnedRenderer;

    void Start() {
        SyncMesh();
    }

    public void SyncMesh() {
        _destSkinnedRenderer = new SkinnedMeshRenderer[8];
        byte childIndex = 0;

        for (byte i = 0; i < transform.parent.childCount; i++) {
            Transform child = transform.parent.GetChild(i);
            if (child != transform) {
                _destSkinnedRenderer[childIndex++] = child.GetComponentInChildren<SkinnedMeshRenderer>();
            } else {
                _rootSkinnedRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();
            }
        }

        foreach (var renderer in _destSkinnedRenderer) {
            if (renderer != null) {
                renderer.bones = _rootSkinnedRenderer.bones;
            }
            //renderer.rootBone = _rootSkinnedRenderer.rootBone;
        }
    }
}
