using UnityEngine.UIElements;

public class SlotHoverDetectManipulator : PointerManipulator
{
    private L2Slot _slot;
    private VisualElement _root;
    public bool Hovering { get; private set; }

    public SlotHoverDetectManipulator(VisualElement target, L2Slot slot)
    {
        _slot = slot;
        _root = target;
        this.target = target;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        _root.RegisterCallback<PointerOverEvent>(PointerOverHandler);
        _root.RegisterCallback<PointerOutEvent>(PointerOutHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        _root.UnregisterCallback<PointerOverEvent>(PointerOverHandler);
        _root.UnregisterCallback<PointerOutEvent>(PointerOutHandler);
    }


    public void PointerOverHandler(PointerOverEvent evt)
    {
        Hovering = true;
        L2SlotManager.Instance.SetHoverSlot(_slot);
    }

    public void PointerOutHandler(PointerOutEvent evt)
    {
        Hovering = false;
        L2SlotManager.Instance.SetHoverSlot(null);
    }
}