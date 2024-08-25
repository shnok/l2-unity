using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionWindow : L2PopupWindow
{
    private const int SLOTS_PER_ROW = 8;
    private VisualTreeAsset _slotTemplate;
    private VisualElement _basicContainer;
    private VisualElement _partyContainer;
    private VisualElement _tokenContainer;
    private VisualElement _socialContainer;

    private static ActionWindow _instance;
    public static ActionWindow Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ActionWindow");
        _slotTemplate = LoadAsset("Data/UI/_Elements/Template/ActionSlot");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        VisualElement dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);

        Label _windowName = (Label)GetElementById("windows-name-label");
        _windowName.text = "Actions";

        _basicContainer = GetElementById("BasicSlots");
        _partyContainer = GetElementById("PartySlots");
        _tokenContainer = GetElementById("TokenSlots");
        _socialContainer = GetElementById("SocialSlots");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        // Center window
        _windowEle.style.left = new Length(50, LengthUnit.Percent);
        _windowEle.style.top = new Length(50, LengthUnit.Percent);
        _windowEle.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent)));

        int position = 0;
        for (int i = 0; i < SLOTS_PER_ROW * 4; i++)
        {
            VisualElement slotElement = _slotTemplate.Instantiate()[0];
            _basicContainer.Add(slotElement);

            ActionSlot slot = new ActionSlot(slotElement, position++);
        }

        for (int i = 0; i < SLOTS_PER_ROW * 2; i++)
        {
            VisualElement slotElement = _slotTemplate.Instantiate()[0];
            _partyContainer.Add(slotElement);

            ActionSlot slot = new ActionSlot(slotElement, position++);
        }

        for (int i = 0; i < SLOTS_PER_ROW * 2; i++)
        {
            VisualElement slotElement = _slotTemplate.Instantiate()[0];
            _tokenContainer.Add(slotElement);

            ActionSlot slot = new ActionSlot(slotElement, position++);
        }

        for (int i = 0; i < SLOTS_PER_ROW * 3; i++)
        {
            VisualElement slotElement = _slotTemplate.Instantiate()[0];
            _socialContainer.Add(slotElement);

            ActionSlot slot = new ActionSlot(slotElement, position++);
        }

        // int slotCount = InventoryWindow.Instance.SlotCount;
        // _inventorySlots = new InventorySlot[slotCount];

        // L2Slot.SlotType slotType = L2Slot.SlotType.Inventory;
        // if (!MainTab)
        // {
        //     slotType = L2Slot.SlotType.InventoryBis;
        // }

        // for (int i = 0; i < slotCount; i++)
        // {
        //     VisualElement slotElement = InventoryWindow.Instance.InventorySlotTemplate.Instantiate()[0];
        //     _contentContainer.Add(slotElement);

        //     InventorySlot slot = new InventorySlot(i, slotElement, this, slotType);
        //     _inventorySlots[i] = slot;
        // }
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        AudioManager.Instance.PlayUISound("window_open");
        L2GameUI.Instance.WindowOpened(this);
    }

    public override void HideWindow()
    {
        base.HideWindow();
        AudioManager.Instance.PlayUISound("window_close");
        L2GameUI.Instance.WindowClosed(this);
    }

}