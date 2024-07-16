using UnityEngine;
using UnityEngine.UIElements;

public abstract class L2UI : MonoBehaviour
{
    private VisualElement _rootElement;

    [SerializeField] private bool _uiLoaded = false;
    [SerializeField] private Focusable _focusedElement;
    [SerializeField] private bool _mouseOverUI = false;
    [SerializeField] private VisualElement _loadingElement;

    protected VisualElement _rootVisualContainer;
    protected VisualElement _popupVisualContainer;

    public bool MouseOverUI { get { return _mouseOverUI; } set { _mouseOverUI = value; } }
    public bool UILoaded { get { return _uiLoaded; } set { _uiLoaded = value; } }
    public VisualElement RootElement { get { return _rootElement; } }


    protected virtual void Update() {
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

    public void BlurFocus() {
        if (_focusedElement != null) {
            _focusedElement.Blur();
        }
    }

    protected virtual void LoadUI() {
        _rootVisualContainer = _rootElement.Q<VisualElement>("UIContainer");
        _popupVisualContainer = _rootElement.Q<VisualElement>("UIContainerPopup");
        _loadingElement = _rootElement.Q<VisualElement>("Loading");
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
