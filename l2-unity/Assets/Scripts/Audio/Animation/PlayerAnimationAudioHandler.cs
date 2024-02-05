using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationAudioHandler : MonoBehaviour
{
    private SurfaceDetector _surfaceDetector;
    private void Start() {
        _surfaceDetector = transform.GetComponentInParent<SurfaceDetector>(true);    
    }

    public void PlayRunStepSound() {
        if(_surfaceDetector.GetSurfaceTag() != null) {
            AudioManager.Instance.PlayStepSound(_surfaceDetector.GetSurfaceTag(), transform.position);
        }
    }
}
