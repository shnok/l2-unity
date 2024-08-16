using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class L2Slot {
    [SerializeField] protected int _id;
    [SerializeField] protected int _position;
    protected string _name;
    protected string _description;
    protected string _icon;
    protected VisualElement _slotElement;
    protected VisualElement _slotBg;

    public int Id { get { return _id; } set { _id = value; } }
    public int Position { get { return _position; } set { _position = value; } }
    public string Name { get { return _name; } set { _name = value; } }
    public string Description { get { return _description; } set { _description = value; } }
    public string Icon { get { return _icon; } set { _icon = value; } }
    public VisualElement SlotBg { get { return _slotBg; } }
    public VisualElement SlotElement { get { return _slotElement; } }

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
        _slotElement.RegisterCallback<PointerOverEvent>(PointerOverHandler);
        _slotElement.RegisterCallback<PointerOutEvent>(PointerOutHandler);
    }

    public void UnregisterCallbacks() {
        _slotElement.UnregisterCallback<MouseDownEvent>(HandleSlotClick, TrickleDown.TrickleDown);
        _slotElement.UnregisterCallback<PointerOverEvent>(PointerOverHandler);
        _slotElement.UnregisterCallback<PointerOutEvent>(PointerOutHandler);
    }

    public void PointerOverHandler(PointerOverEvent evt) {
        L2SlotManager.Instance.SetHoverSlot(this);
    }

    public void PointerOutHandler(PointerOutEvent evt) {
        L2SlotManager.Instance.SetHoverSlot(null);
    }

    protected virtual void HandleLeftClick() {}
    protected virtual void HandleRightClick() {}
    
    public void SetSelected() {
        Debug.Log($"Slot {_position} selected.");
        _slotElement.AddToClassList("selected");
    }

    public void UnSelect() {
        Debug.Log($"Slot {_position} unselected.");
        _slotElement.RemoveFromClassList("selected");
    }
}
