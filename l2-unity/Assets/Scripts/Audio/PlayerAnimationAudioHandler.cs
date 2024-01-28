using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationAudioHandler : MonoBehaviour
{
    SurfaceDetector surfaceDetector;
    private void Start() {
        surfaceDetector = transform.GetComponentInParent<SurfaceDetector>(true);    
    }

    public void PlayRunStepSound() {
        if(surfaceDetector.GetSurfaceTag() != null) {
            AudioManager.GetInstance().PlayStepSound(surfaceDetector.GetSurfaceTag(), transform.position);
        }
    }
}
