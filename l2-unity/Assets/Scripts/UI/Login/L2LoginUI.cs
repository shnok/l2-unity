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
    public VisualElement RootElement { get { return _rootElement; } }

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

        LoginWindow.Instance.AddWindow(rootVisualContainer);
        CharSelectWindow.Instance.AddWindow(rootVisualContainer);
        CharSelectWindow.Instance.HideWindow();
        CharCreationWindow.Instance.AddWindow(rootVisualContainer);
        CharCreationWindow.Instance.HideWindow();
        LicenseWindow.Instance.AddWindow(rootVisualContainer);
        LicenseWindow.Instance.HideWindow();
        ServerSelectWindow.Instance.AddWindow(rootVisualContainer);
        ServerSelectWindow.Instance.HideWindow();
    }

    public void ShowServerSelectWindow() {
        LoginWindow.Instance.HideWindow();
        LicenseWindow.Instance.HideWindow();
        ServerSelectWindow.Instance.ShowWindow();
    }

    public void ShowLicenseWindow() {
        LoginWindow.Instance.HideWindow();
        LicenseWindow.Instance.ShowWindow();
        ServerSelectWindow.Instance.HideWindow();
    }

    public void ShowCharSelectWindow() {
        LoginWindow.Instance.HideWindow();
        CharCreationWindow.Instance.HideWindow();
        CharSelectWindow.Instance.ShowWindow();
        ServerSelectWindow.Instance.HideWindow();
    }

    public void ShowLoginWindow() {
        CharSelectWindow.Instance.HideWindow();
        LoginWindow.Instance.ShowWindow();
        CharCreationWindow.Instance.HideWindow();
        CharSelectWindow.Instance.HideWindow();
        LicenseWindow.Instance.HideWindow();
        ServerSelectWindow.Instance.HideWindow();
    }

    public void ShowCharCreationWindow() {
        CharSelectWindow.Instance.HideWindow();
        CharCreationWindow.Instance.ShowWindow();
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
