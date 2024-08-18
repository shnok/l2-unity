using UnityEngine;
public class FpsLimiter : MonoBehaviour {
    [SerializeField] private int focusedFrameRate = 60;
    [SerializeField] private int unfocusedFrameRate = 5;

    private void Start() {
#if UNITY_EDITOR
        this.enabled = false;
#endif
        QualitySettings.vSyncCount = 0;

        // Set the initial frame rate
        SetFrameRate(focusedFrameRate);
    }

    private void Update() {
        // Check if the application window is focused
        bool isFocused = Application.isFocused;

        // Set the frame rate based on focus state
        int targetFrameRate = isFocused ? focusedFrameRate : unfocusedFrameRate;
        SetFrameRate(targetFrameRate);
    }

    private void SetFrameRate(int frameRate) {
        // Set the target frame rate
        Application.targetFrameRate = frameRate;
    }
}
