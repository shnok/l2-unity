using UnityEngine;

public class TextureAnimator : MonoBehaviour {
    public Material material;
    public float frameIndex;
    public float frameDurationMs = 1000;
    public int framesCount;


    void Start() {
        material = GetComponent<Renderer>().material;
        frameIndex = 1;
        framesCount = ((Texture2DArray)material.GetTexture("_FramesArray")).depth;
    }

    // Update is called once per frame
    void Update() {
        frameIndex += Time.deltaTime * (1f / (frameDurationMs / 1000f));
        if(frameIndex >= framesCount) {
            frameIndex = 0;
        }

        material.SetFloat("_FramesIndex", frameIndex);
    }
}
