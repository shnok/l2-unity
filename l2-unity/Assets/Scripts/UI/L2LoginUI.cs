using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class L2LoginUI : MonoBehaviour
{
    private VisualElement _rootElement;

    [SerializeField] private bool _uiLoaded = false;
    [SerializeField] private Focusable _focusedElement;

    public bool UILoaded { get { return _uiLoaded; } set { _uiLoaded = value; } }

    private static L2LoginUI _instance;
    public static L2LoginUI Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }
    public void Update() {
        if (_rootElement != null && !float.IsNaN(_rootElement.resolvedStyle.width) && _uiLoaded == false) {
            LoadUI();
            _uiLoaded = true;
        } else {
            _rootElement = GetComponent<UIDocument>().rootVisualElement;
        }

        if (_uiLoaded) {
            _focusedElement = _rootElement.focusController.focusedElement;
        }
    }

    private void LoadUI() {
        VisualElement rootVisualContainer = _rootElement.Q<VisualElement>("UIContainer");
        
        LoginWindow.Instance.AddWindow(rootVisualContainer);
    }
}
