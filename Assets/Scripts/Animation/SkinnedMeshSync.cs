using UnityEngine;

public class SkinnedMeshSync : MonoBehaviour {
    [SerializeField] private SkinnedMeshRenderer rootSkinnedRenderer;
    [SerializeField] private SkinnedMeshRenderer[] destSkinnedRenderer;

    // Start is called before the first frame update
    void Start() {
        destSkinnedRenderer = new SkinnedMeshRenderer[transform.parent.childCount - 1];
        for(int i = 0; i < transform.parent.childCount; i++) {
            Transform child = transform.parent.GetChild(i);
            if(child != transform) {
                destSkinnedRenderer[i] = child.GetComponentInChildren<SkinnedMeshRenderer>();
            } else {
                rootSkinnedRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();
            }
        }

        foreach(var renderer in destSkinnedRenderer) {
            renderer.bones = rootSkinnedRenderer.bones;
            renderer.rootBone = rootSkinnedRenderer.rootBone;
        }
    }
}
