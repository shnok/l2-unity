using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class L2Slot {
    protected int _id;
    protected int _position;
    protected string _name;
    protected string _description;
    protected string _icon;
    protected VisualElement _slotElement;
    protected VisualElement _slotBg;

    public int Id { get { return _id; } }
    public int Position { get { return _position; } }
    public string Name { get { return _name; } }
    public string Description { get { return _description; } }
    public string Icon { get { return _icon; } }

    public L2Slot(VisualElement slotElement, int position, int id, string name, string description, string icon) {
        _slotElement = slotElement;
        _position = position;
        _id = id;
        _name = name;
        _description = description;
        _icon = icon;

        _slotBg = _slotElement.Q<VisualElement>(null, "slot-bg");

        RegisterCallbacks();
    }

    public L2Slot(VisualElement slotElement, int position) {
        _slotElement = slotElement;
        _position = position;

        _slotBg = _slotElement.Q<VisualElement>(null, "slot-bg");

        RegisterCallbacks();
    }

    protected void HandleSlotClick(MouseDownEvent evt) {
        if (evt.button == 0) {
            HandleLeftClick();
        } else if (evt.button == 1) {
            HandleRightClick();
        }
    }

    protected void RegisterCallbacks() {
        _slotElement.RegisterCallback<MouseDownEvent>(HandleSlotClick, TrickleDown.TrickleDown);
    }

    protected void UnregisterCallbacks() {
        _slotElement.UnregisterCallback<MouseDownEvent>(HandleSlotClick, TrickleDown.TrickleDown);
    }

    protected abstract void HandleLeftClick();
    protected abstract void HandleRightClick();
}
