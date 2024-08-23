using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class AbstractSkillbar
{
    protected VisualElement _windowEle;
    protected VisualElement _skillbarWindow;
    private int _skillbarIndex;
    protected int _page;
    protected bool _horizontalBar;
    protected List<VisualElement> _slotAnchors;
    protected List<L2Slot> _barSlots;
    protected ArrowInputManipulator _arrowInputManipulator;

    public int Page { get { return _page; } }
    public int SkillbarIndex { get { return _skillbarIndex; } }

    public AbstractSkillbar(VisualElement skillbarWindowElement, int skillbarIndex, int page, bool horizontalBar)
    {
        _skillbarIndex = skillbarIndex;
        _skillbarWindow = skillbarWindowElement;
        _page = page;
        _horizontalBar = horizontalBar;
        _slotAnchors = new List<VisualElement>();
    }

    public IEnumerator BuildWindow(VisualTreeAsset template, VisualElement container)
    {
        _windowEle = template.Instantiate()[0];
        container.Add(_windowEle);

        yield return new WaitForEndOfFrame();

        VisualElement dragArea = _windowEle.Q<VisualElement>("DragArea");
        _windowEle.AddManipulator(new DragManipulator(dragArea, _skillbarWindow));

        for (int i = 1; i <= 12; i++)
        {
            VisualElement anchor = _windowEle.Q<VisualElement>("SlotAnchor" + i);
            _slotAnchors.Add(anchor);
        }

        VisualElement arrowInput = _windowEle.Q<VisualElement>(null, "skill-arrow-input");
        string[] arrowInputValues = new string[10];
        for (int i = 1; i <= 10; i++)
        {
            arrowInputValues[i - 1] = i.ToString();
        }

        _arrowInputManipulator = new ArrowInputManipulator(arrowInput, null, arrowInputValues, _page,
        (index, value) => SkillbarWindow.Instance.OnPageChanged(SkillbarIndex, index));

        UpdateVisuals();
        UpdateShortcuts();
    }

    public void ChangePage(int page)
    {
        _page = page;
        _arrowInputManipulator.SelectIndex(page);

        UpdateShortcuts();
    }

    public void UpdateShortcuts()
    {
        // Clean up slot callbacks and manipulators
        if (_barSlots != null)
        {
            foreach (L2Slot slot in _barSlots)
            {
                slot.UnregisterCallbacks();
                slot.ClearManipulators();
            }
            _barSlots.Clear();
        }

        _barSlots = new List<L2Slot>();

        // Clean up gear anchors from any child visual element
        for (int i = 0; i < _slotAnchors.Count; i++)
        {
            VisualElement anchor = _slotAnchors[i];

            // Clear childs
            anchor.Clear();

            // Create slots
            VisualElement slotElement = SkillbarWindow.Instance.BarSlotTemplate.Instantiate()[0];
            anchor.Add(slotElement);

            SKillbarSlot slot = new SKillbarSlot(_page, i, slotElement, L2Slot.SlotType.SkillBar);
            _barSlots.Add(slot);
        }

    }

    protected abstract void UpdateVisuals();

    public void HideBar()
    {
        _windowEle.style.display = DisplayStyle.None;
    }

    public void ShowBar()
    {
        _windowEle.style.display = DisplayStyle.Flex;
    }
}
