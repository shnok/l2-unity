using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class L2LoginUI : MonoBehaviour
{
    private VisualElement _rootElement;

    [SerializeField] private bool _uiLoaded = false;
    [SerializeField] private Focusable _focusedElement;
    [SerializeField] private VisualElement _loadingElement;

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

        _loadingElement = _rootElement.Q<VisualElement>("Loading");

        StartLoading();

        LoginWindow.Instance.AddWindow(rootVisualContainer);
        CharSelectWindow.Instance.AddWindow(rootVisualContainer);
        CharSelectWindow.Instance.HideWindow();
    }

    public void OnLogin() {
        LoginWindow.Instance.HideWindow();
        CharSelectWindow.Instance.ShowWindow();

    }

    public void OnCharSelect() {
        CharSelectWindow.Instance.HideWindow();

    }

    public void OnRelogin() {
        CharSelectWindow.Instance.HideWindow();
        LoginWindow.Instance.ShowWindow();
    }

    public void StartLoading() {
        if (_loadingElement != null) {
            _loadingElement.style.display = DisplayStyle.Flex;
        }
    }

    public void StopLoading() {
        if (_loadingElement != null) {
            _loadingElement.style.display = DisplayStyle.None;
        }
    }
}
