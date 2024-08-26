using UnityEngine.UIElements;

public class ActionSlot : L2DraggableSlot
{
    public int ActionId { get; private set; }
    public ActionData Action { get; private set; }

    public ActionSlot(VisualElement slotElement, int position, SlotType slotType) : base(position, slotElement, slotType, true, false)
    {
        _slotElement = slotElement;
        _position = position;
    }

    protected override void HandleLeftClick()
    {
        PlayerActions.Instance.UseAction((ActionType)ActionId);
    }

    protected override void HandleRightClick()
    {
    }

    protected override void HandleMiddleClick()
    {
    }

    public void AssignAction(ActionType actionType)
    {
        ButtonClickSoundManipulator _buttonClickSoundManipulator = new ButtonClickSoundManipulator(_slotElement);
        _slotDragManipulator.enabled = true;
        _slotElement.RemoveFromClassList("empty");

        ActionId = (int)actionType;
        Action = ActionNameTable.Instance.GetAction(actionType);
        _slotBg.style.backgroundImage = IconManager.Instance.LoadTextureByName(Action.Icon);

        if (_tooltipManipulator != null)
        {
            _tooltipManipulator.SetText(Action.Name);
        }
    }
}