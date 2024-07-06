using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class L2Window : MonoBehaviour {
    protected VisualTreeAsset _windowTemplate;
    protected VisualElement _windowEle;
    private bool _isWindowHidden = false;
    private MouseOverDetectionManipulator _mouseOverDetection;

    void Start() {
        _isWindowHidden = false;
        LoadAssets();
    }

    protected abstract void LoadAssets();

    protected VisualTreeAsset LoadAsset(string assetPath) {
        VisualTreeAsset asset = Resources.Load<VisualTreeAsset>(assetPath);
        if (asset == null) {
            Debug.LogError($"Could not load {assetPath} template.");
        }

        return asset;
    }

    public void AddWindow(VisualElement root) {
        if (_windowTemplate == null) {
            return;
        }
        StartCoroutine(BuildWindow(root));
    }

    protected void InitWindow(VisualElement root) {
        _windowEle = _windowTemplate.Instantiate()[0];
        _mouseOverDetection = new MouseOverDetectionManipulator(_windowEle);
        _windowEle.AddManipulator(_mouseOverDetection);

        if (_isWindowHidden) {
            _mouseOverDetection.Disable();
        }

        root.Add(_windowEle);
    }

    protected abstract IEnumerator BuildWindow(VisualElement root);

    public void HideWindow() {
        _isWindowHidden = true;
        _windowEle.style.display = DisplayStyle.None;
        _mouseOverDetection.Disable();
    }

    public void ShowWindow() {
        _isWindowHidden = false;
        _windowEle.style.display = DisplayStyle.Flex;
        _mouseOverDetection.Enable();
    }

    public void ToggleHideWindow() {
        if(_isWindowHidden) {
            ShowWindow();
        } else {
            HideWindow();
        }
    }

    protected VisualElement GetElementById(string id) {
        var btn = _windowEle.Q<VisualElement>(id);
        if (btn == null) {
            Debug.LogError(id + " can't be found.");
            return null;
        }

        return btn;
    }
}
