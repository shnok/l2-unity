using UnityEngine;
using UnityEngine.UIElements;

public class MouseOverDetectionManipulator : PointerManipulator
{
    private bool _enabled;
    private bool _overThisManipulator = false;
    private L2UI _ui;

    public bool MouseOver { get { return _overThisManipulator; } }

    public MouseOverDetectionManipulator(VisualElement target)
    {
        _enabled = true;
        this.target = target;
        if (L2GameUI.Instance != null)
        {
            _ui = L2GameUI.Instance;
        }
        else
        {
            _ui = L2LoginUI.Instance;
        }
    }

    public void Enable()
    {
        _enabled = true;
    }

    public void Disable()
    {
        _enabled = false;

        if (_overThisManipulator)
        {
            _overThisManipulator = false;
            _ui.MouseOverUI = false;
        }
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerEnterEvent>(PointerEnterHandler);
        target.RegisterCallback<PointerOverEvent>(PointerOverHandler);
        target.RegisterCallback<PointerOutEvent>(PointerOutHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerEnterEvent>(PointerEnterHandler);
        target.UnregisterCallback<PointerOverEvent>(PointerOverHandler);
        target.UnregisterCallback<PointerOutEvent>(PointerOutHandler);
    }

    private void PointerEnterHandler(PointerEnterEvent evt)
    {
        if (_enabled)
        {
            _ui.MouseOverUI = true;
            _overThisManipulator = true;
        }
    }

    private void PointerOverHandler(PointerOverEvent evt)
    {
        if (_enabled)
        {
            // Debug.LogWarning(target);
            _ui.MouseOverUI = true;
            _overThisManipulator = true;
        }
    }

    private void PointerOutHandler(PointerOutEvent evt)
    {
        if (_enabled)
        {
            _ui.MouseOverUI = false;
            _overThisManipulator = false;
        }
    }
}
