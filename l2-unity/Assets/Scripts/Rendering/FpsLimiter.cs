using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FpsLimiter : MonoBehaviour {
    public int focusedFrameRate = 60;
    public int unfocusedFrameRate = 5;

    private void Start() {
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
