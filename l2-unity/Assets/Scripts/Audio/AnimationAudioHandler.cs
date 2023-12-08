using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAudioHandler : MonoBehaviour
{
    SurfaceDetector surfaceDetector;
    private void Start() {
        surfaceDetector = transform.parent.GetComponent<SurfaceDetector>();    
    }

    public void PlayRunStepSound() {
        AudioManager.GetInstance().PlayStepSound(surfaceDetector.GetSurfaceTag(), transform.position);
    }
}
