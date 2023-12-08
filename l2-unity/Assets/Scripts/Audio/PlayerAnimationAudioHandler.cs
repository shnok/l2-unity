using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationAudioHandler : MonoBehaviour
{
    SurfaceDetector surfaceDetector;
    private void Start() {
        surfaceDetector = transform.parent.GetComponent<SurfaceDetector>();    
    }

    public void PlayRunStepSound() {
        if(surfaceDetector.GetSurfaceTag() != null) {
            AudioManager.GetInstance().PlayStepSound(surfaceDetector.GetSurfaceTag(), transform.position);
        }
    }
}
