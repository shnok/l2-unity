using UnityEngine.UIElements;

public class ActionSlot : L2DraggableSlot
{
    public int ActionId { get; private set; }

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

    public void AssignAction(int actionId)
    {
        ActionId = actionId;
        _slotDragManipulator.enabled = true;
        ButtonClickSoundManipulator _buttonClickSoundManipulator = new ButtonClickSoundManipulator(_slotElement);
        _slotElement.RemoveFromClassList("empty");
        _slotBg.style.backgroundImage = IconManager.Instance.LoadTextureByName($"action{actionId.ToString().PadLeft(3, '0')}");
    }
}