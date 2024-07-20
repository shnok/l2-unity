using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventoryWindow : L2PopupWindow
{

    //private VisualTreeAsset _testUITemplate;
    //public VisualElement minimal_panel;
    //private ButtonInventory _buttonInventory;
    //private VisualElement boxHeader;
    //private VisualElement boxContent;
    //private VisualElement background;
    //private VisualElement rootWindow;
    //private VisualElement[] _menuItems;
    //private VisualElement[] _inventoryRows;
    //private bool isHide;
    //private ModelRows[] _activeRows;
    //private ModelRows _lastSelectRow;
    //private MouseOverDetectionManipulator _mouseOverDetection;
    //private Texture2D _selectFrame;
    //private Texture2D _blackFrame;

    //private EquipInventory _equipInventory;


    //string[] fillBackground = { "Data/UI/ShortCut/demo_skills/fill_black", "Data/UI/Window/Inventory/bg_windows/blue_tab_v5" };

    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private VisualTreeAsset _inventorySlotTemplate;

    public VisualTreeAsset InventorySlotTemplate { get { return _inventorySlotTemplate; } }

    private VisualElement _inventoryTabView;

    [SerializeField] private List<InventoryTab> _tabs;
    private InventoryTab _activeTab;

    private static InventoryWindow _instance;
    public static InventoryWindow Instance {
        get { return _instance; }
    }
    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }

        /* TO REMOVE FOR TESTING ONLY */
        ItemTable.Instance.Initialize();
        ItemNameTable.Instance.Initialize();
        ItemStatDataTable.Instance.Initialize();
        ArmorgrpTable.Instance.Initialize();
        EtcItemgrpTable.Instance.Initialize();
        WeapongrpTable.Instance.Initialize();
        ItemTable.Instance.CacheItems();
    }

    private void OnDestroy() {
        _instance = null;
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryWindow");
        _tabTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryTab");
        _tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryTabHeader");
        _inventorySlotTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/Slot");
    }

    protected override void InitWindow(VisualElement root) {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        CreateTabs();
    }


    private void CreateTabs() {
        _inventoryTabView = GetElementById("InventoryTabView");

        VisualElement tabHeaderContainer = _inventoryTabView.Q<VisualElement>("tab-header-container");
        if (tabHeaderContainer == null) {
            Debug.LogError("tab-header-container is null");
        }
        VisualElement tabContainer = _inventoryTabView.Q<VisualElement>("tab-content-container");

        if (tabContainer == null) {
            Debug.LogError("tab-content-container");
        }

        for (int i = _tabs.Count -1; i >= 0; i--) {
            VisualElement tabElement = _tabTemplate.CloneTree()[0];
            // tabElement.name = _tabs[i].TabName;
            tabElement.name = _tabs[i].TabName;
            tabElement.AddToClassList("unselected-tab");

            VisualElement tabHeaderElement = _tabHeaderTemplate.CloneTree()[0];
            tabHeaderElement.name = _tabs[i].TabName;
            tabHeaderElement.Q<Label>().text = _tabs[i].TabName;

            tabHeaderContainer.Add(tabHeaderElement);
            tabContainer.Add(tabElement);

            _tabs[i].Initialize(_windowEle, tabElement, tabHeaderElement);
        }

        if (_tabs.Count > 0) {
            SwitchTab(_tabs[0]);
        }
    }

    public bool SwitchTab(InventoryTab switchTo) {
        if (_activeTab != switchTo) {
            if (_activeTab != null) {
                _activeTab.TabContainer.AddToClassList("unselected-tab");
                _activeTab.TabHeader.RemoveFromClassList("active");
            }

            switchTo.TabContainer.RemoveFromClassList("unselected-tab");
            switchTo.TabHeader.AddToClassList("active");
            //ScrollDown(switchTo.Scroller);

            _activeTab = switchTo;
            return true;
        }

        return false;
    }

    //private void Awake()
    //{
    //    if (_instance == null)
    //    {
    //        _instance = this;
    //        _buttonInventory = new ButtonInventory(this);
    //        _menuItems = new VisualElement[6];
    //        _inventoryRows = new VisualElement[8];
    //         _selectFrame = Resources.Load<Texture2D>(fillBackground[1]);
    //        _blackFrame  = Resources.Load<Texture2D>(fillBackground[0]);
    //        _equipInventory = new EquipInventory(this);
    //        CreateAcive();

    //    }
    //    else
    //    {
    //        Destroy(this);
    //    }
    //}

    //private void CreateAcive()
    //{
    //        var model0 = new ModelRows(0, new int[9]);
    //        var model1 = new ModelRows(1, new int[9]);
    //        var model2 = new ModelRows(2, new int[9]);
    //        var model3 = new ModelRows(3, new int[9]);
    //        var model4 = new ModelRows(4, new int[9]);
    //        var model5 = new ModelRows(5, new int[9]);
    //        var model6 = new ModelRows(6, new int[9]);
    //        var model7 = new ModelRows(7, new int[9]);

    //       _activeRows = new ModelRows[8]{ model0 , model1,model2 ,model3 ,model4, model5, model6, model7 };
    //}

    //public void AddWindow(VisualElement root)
    //{
    //    if (_testUITemplate == null)
    //    {
    //        return;
    //    }


    //    StartCoroutine(BuildWindow(root));
    //}


    //public IEnumerator BuildWindow(VisualElement root)
    //{
    //    var testUI = _testUITemplate.Instantiate()[0];
    //     boxHeader = testUI.Q<VisualElement>(className:"drag-area");
    //     boxContent = testUI.Q<VisualElement>(className: "inventory_content");
    //     background = testUI.Q<VisualElement>(className: "background_over");


    //     rootWindow = testUI.Q<VisualElement>(className: "root_windows");
    //     CreateTab(boxContent, _menuItems);
    //     CreateRowsInventory(boxContent, _inventoryRows);
    //     RegisterClickAllRows(_inventoryRows);
    //     InitTestData(_inventoryRows);
    //     ChangeMenuSelect(0);

    //    _mouseOverDetection = new MouseOverDetectionManipulator(rootWindow);
    //    testUI.AddManipulator(_mouseOverDetection);

    //    DragManipulator drag = new DragManipulator(boxHeader, testUI);
    //    boxHeader.AddManipulator(drag);

    //    _buttonInventory.RegisterButtonCloseWindow(rootWindow, "btn-close-frame");
    //    _buttonInventory.RegisterClickWindow(boxContent, boxHeader);

    //    //Menu CLick
    //    _buttonInventory.RegisterClickMenuAll(_menuItems[0]);
    //    _buttonInventory.RegisterClickMenuEquip(_menuItems[1]);
    //    _buttonInventory.RegisterClickMenuSupplies(_menuItems[2]);
    //    _buttonInventory.RegisterClickMenuCrafting(_menuItems[3]);
    //    _buttonInventory.RegisterClickMenuMisc(_menuItems[4]);
    //    _buttonInventory.RegisterClickMenuQuest(_menuItems[5]);

    //    _equipInventory.registerButtonEquip(boxContent);

    //    HideElements(true);
    //    root.Add(testUI);
    //    yield return new WaitForEndOfFrame();
    //}

    //internal void UnEquip(ModelItemDemo item) {
    //    throw new NotImplementedException();
    //}

    //private void AddActive(int grows_id , int imgbox_id)
    //{
    //    _activeRows[grows_id].AddgArr(imgbox_id, 1);
    //}



    //private void AddInfo(int grows_id, int imgbox_id , DemoL2JItem demoL2j)
    //{
    //    _activeRows[grows_id].AddInfo(imgbox_id, demoL2j);
    //}

    //private void RemoveActive(int grows_id, int imgbox_id)
    //{
    //    _activeRows[grows_id].gArr[imgbox_id] = 0;
    //}
    //private VisualElement[] CreateTab(VisualElement boxContent, VisualElement[] _menuItems)
    //{
    //    _menuItems[0] = boxContent.Q<VisualElement>(className: "alltab");
    //    _menuItems[1] = boxContent.Q<VisualElement>(className: "equiptab");
    //    _menuItems[2] = boxContent.Q<VisualElement>(className: "suppliestab");
    //    _menuItems[3] = boxContent.Q<VisualElement>(className: "craftingtab");
    //    _menuItems[4] = boxContent.Q<VisualElement>(className: "misctab");
    //    _menuItems[5] = boxContent.Q<VisualElement>(className: "questtab");
    //    return _menuItems;
    //}


    //private VisualElement[] CreateRowsInventory(VisualElement boxContent, VisualElement[] _inventoryRows)
    //{
    //    _inventoryRows[0] = boxContent.Q<VisualElement>(className: "rows0");
    //    _inventoryRows[1] = boxContent.Q<VisualElement>(className: "rows1");
    //    _inventoryRows[2] = boxContent.Q<VisualElement>(className: "rows2");
    //    _inventoryRows[3] = boxContent.Q<VisualElement>(className: "rows3");
    //    _inventoryRows[4] = boxContent.Q<VisualElement>(className: "rows4");
    //    _inventoryRows[5] = boxContent.Q<VisualElement>(className: "rows5");
    //    _inventoryRows[6] = boxContent.Q<VisualElement>(className: "rows6");
    //    _inventoryRows[7] = boxContent.Q<VisualElement>(className: "rows7");

    //    return _menuItems;
    //}

    //private void RegisterClickAllRows(VisualElement[] _inventoryRows)
    //{
    //    for(int i=0; i <_inventoryRows.Length; i++)
    //    {

    //        var rows = _inventoryRows[i];
    //        if (rows != null)
    //        {
    //            var grow0 = rows.Q<VisualElement>(className: "grow0");
    //            var grow1 = rows.Q<VisualElement>(className: "grow1");
    //            var grow2 = rows.Q<VisualElement>(className: "grow2");
    //            var grow3 = rows.Q<VisualElement>(className: "grow3");
    //            var grow4 = rows.Q<VisualElement>(className: "grow4");
    //            var grow5 = rows.Q<VisualElement>(className: "grow5");
    //            var grow6 = rows.Q<VisualElement>(className: "grow6");
    //            var grow7 = rows.Q<VisualElement>(className: "grow7");
    //            var grow8 = rows.Q<VisualElement>(className: "grow8");

    //            _buttonInventory.RegisterClickInventoryCell(grow0);
    //            _buttonInventory.RegisterClickInventoryCell(grow1);
    //            _buttonInventory.RegisterClickInventoryCell(grow2);
    //            _buttonInventory.RegisterClickInventoryCell(grow3);
    //            _buttonInventory.RegisterClickInventoryCell(grow4);
    //            _buttonInventory.RegisterClickInventoryCell(grow5);
    //            _buttonInventory.RegisterClickInventoryCell(grow6);
    //            _buttonInventory.RegisterClickInventoryCell(grow7);
    //            _buttonInventory.RegisterClickInventoryCell(grow8);

    //        }

    //    }
    //}

    //public void InitTestData(VisualElement[] _inventoryRows)
    //{
    //    var rows0 = _inventoryRows[0];
    //    List<DemoL2JItem> demo = createL2jData();
    //    SetInventory(demo, rows0);
    //    ElseEquip(demo);
    //}

    //private void ElseEquip(List<DemoL2JItem> listDemo)
    //{
    //    foreach (DemoL2JItem itemL2j in listDemo)
    //    {
    //        if (itemL2j.Equipped == 1)
    //        {
    //            //0 - type_weapon
    //            if(itemL2j.Type2 == 0)
    //            {
    //                ModelItemDemo model = new ModelItemDemo(itemL2j);
    //                _equipInventory.AddEquipList(0, model);
    //                _equipInventory.EquipItemNoInventory(model, itemL2j.Type2 , boxContent);
    //            }

    //        }
    //    }

    //}

    //public List<ModelItemDemo> filterEquip(List<ModelItemDemo> listDemo)
    //{
    //    List<ModelItemDemo> filteritems = new List<ModelItemDemo>();

    //    foreach(ModelItemDemo item in listDemo)
    //    {
    //        if(item.Equipped() == 0)
    //        {
    //            filteritems.Add(item);
    //        }
    //    }
    //    return filteritems;
    //}

    //private void SetInventory(List<DemoL2JItem> demo, VisualElement rows0)
    //{
    //    Texture2D fill = Resources.Load<Texture2D>(fillBackground[0]);

    //    foreach (DemoL2JItem item in demo)
    //    {
    //        if(item.Equipped == 0)
    //        {
    //            AddInfo(0, item.Location, item);
    //            var grow = rows0.Q<VisualElement>(className: "grow" + item.Location);
    //            var img = rows0.Q<VisualElement>(className: "imgbox" + item.Location);
    //            var model = _activeRows[0].GetInfo(item.Location);
    //            SetBackground(fill, grow);
    //            SetBackground(model.Icon(), img);

    //            //0 - rows the first line
    //            //depending on the cell number, you can calculate the row
    //            AddActive(0, item.Location);

    //        }

    //    }
    //    // SetBackground(demo1.Icon(), img0);
    //    //SetBackground(demo2.Icon(), img1);
    //    //SetBackground(demo3.Icon(), img2);

    //    //var grow0 = rows0.Q<VisualElement>(className: "grow0");
    //    //var grow1 = rows0.Q<VisualElement>(className: "grow1");
    //    //var grow2 = rows0.Q<VisualElement>(className: "grow2");

    //    //var img0 = rows0.Q<VisualElement>(className: "imgbox0");
    //    //var img1 = rows0.Q<VisualElement>(className: "imgbox1");
    //    //var img2 = rows0.Q<VisualElement>(className: "imgbox2");
    //}

    //private List<DemoL2JItem> createL2jData()
    //{

    //    DemoL2JItem demo2 = new DemoL2JItem();
    //    demo2.ObjectId = 268465500;
    //    demo2.ItemId = 45573;
    //    demo2.T1 = 34;
    //    demo2.Quantity = 1;
    //    demo2.Type2 = 1;
    //    demo2.Filler = 0;
    //    demo2.Equipped = 0;
    //    demo2.Slot = 64;
    //    demo2.Enchant = 0;
    //    demo2.Location = 1;
    //   // demo2.Pet = 0;
    //    demo2.AugmentationBonus = 0;
    //    demo2.Mana = -1;
    //    demo2.Time = -9999;


    //    DemoL2JItem demo = new DemoL2JItem();
    //    //adena TYPE2_MONEY = 4;
    //    demo.ObjectId = 268473060;
    //    demo.ItemId = 57;
    //    demo.T1 = 5;
    //    demo.Quantity = 4;
    //    demo.Type2 = 4;
    //    demo.Filler = 0;
    //    demo.Equipped = 0;
    //    demo.Slot = 0;
    //    demo.Enchant = 0;
    //    //demo.Pet = 0;
    //    demo.AugmentationBonus = 0;
    //    demo.Mana = -1;
    //    demo.Time = -9999;
    //    demo.Location = 0;

    //    DemoL2JItem demo1 = new DemoL2JItem();

    //    //Dagger equip weapon  TYPE2_WEAPON = 0;
    //    demo1.ObjectId = 268463170;
    //    demo1.ItemId = 2369;
    //    //location
    //    demo1.T1 = 5;
    //    demo1.Quantity = 0;
    //    //// different lists for armor, weapon, etc
    //    //0-weapon
    //    //5 etc
    //    demo1.Type2 = 0;
    //    demo1.Filler = 0;
    //    demo1.Equipped = 1;
    //    demo1.Slot = 128;
    //    demo1.Enchant = 0;
    //    //demo.Pet = 0;
    //    demo1.AugmentationBonus = 0;
    //    demo1.Mana = -1;
    //    demo1.Time = -9999;
    //    demo1.Location = 2;

    //    List<DemoL2JItem> demoL2JItems = new List<DemoL2JItem>
    //    {
    //        demo1,
    //        demo2,
    //        demo
    //    };
    //    return demoL2JItems;

    //}

    //private void SetBackground(Texture2D imgSource1 , VisualElement element)
    //{
    //    if (imgSource1 != null)
    //    {
    //        element.style.backgroundImage = new StyleBackground(imgSource1);
    //    }
    //}

    //public void SelectRows(VisualElement grow)
    //{
    //    if(grow != null)
    //    {

    //        var rows = grow.parent;

    //        var rows_id = rows.name.Replace("Rows", "");
    //        var grow_id = grow.name.Replace("GRow", "");

    //        int parce_int_grow = Int32.Parse(grow_id);
    //        int parce_int_rows = Int32.Parse(rows_id);
    //        //if not empty
    //        if(IsActiveRow(parce_int_rows, parce_int_grow))
    //        {

    //            UpdateBackGroundSelectElement(grow);

    //            if (_lastSelectRow == null)
    //            {
    //                UpdateLastPosition(parce_int_rows, parce_int_grow);
    //            }
    //            else
    //            {
    //                int last_row = _lastSelectRow.rows;
    //                int last_grow = _lastSelectRow.lastActiveGRow;

    //                if (IsActiveRow(last_row, last_grow))
    //                { 
    //                    UpdateBackGroundLastElement(last_row, last_grow, _blackFrame);
    //                }
    //                else
    //                {
    //                    UpdateBackGroundReset(last_row, last_grow);
    //                }

    //                UpdateLastPosition(parce_int_rows, parce_int_grow);
    //            }
    //        }
    //        else
    //        {
    //            //if empty call
    //            if (_lastSelectRow == null) {
    //                UpdateLastPosition(parce_int_rows, parce_int_grow);
    //                UpdateBackGroundSelectElement(grow);
    //            }
    //            else
    //            {
    //                int last_row = _lastSelectRow.rows;
    //                int last_grow = _lastSelectRow.lastActiveGRow;

    //                UpdateBackGroundReset(last_row, last_grow);
    //                UpdateBackGroundLastElement(parce_int_rows, parce_int_grow, _selectFrame);
    //                UpdateLastPosition(parce_int_rows, parce_int_grow);
    //            }

    //        }
    //    }

    //}


    //private void UpdateBackGroundSelectElement(VisualElement grow)
    //{
    //    Texture2D blueFrame = Resources.Load<Texture2D>(fillBackground[1]);
    //    SetBackground(blueFrame, grow);
    //}
    //private void UpdateBackGroundLastElement(int last_row , int last_grow , Texture2D textuteRefresh)
    //{
    //    VisualElement row = _inventoryRows[last_row];
    //    var grow_elem = row.Q<VisualElement>(className: "grow" + last_grow);
    //    SetBackground(textuteRefresh, grow_elem);
    //}

    //private void UpdateBackGroundReset(int last_row, int last_grow)
    //{
    //    VisualElement row = _inventoryRows[last_row];
    //    var grow_elem = row.Q<VisualElement>(className: "grow" + last_grow);
    //    grow_elem.style.backgroundImage = null;
    //}
    //private void UpdateLastPosition(int parce_int_rows , int parce_int_grow)
    //{
    //    var model = GetRowActive(parce_int_rows);
    //    model.lastActiveGRow = parce_int_grow;
    //    _lastSelectRow = model;
    //}
    //private bool IsActiveRow(int row , int grow)
    //{
    //    if(_activeRows[row].gArr[grow] == 1){
    //        return true;
    //    }
    //    return false;
    //}

    //public void DeactiveRow(int row, int grow)
    //{
    //    _activeRows[row].gArr[grow] = 0;
    //}

    //public void ClearLastSelect()
    //{
    //    _lastSelectRow = null;
    //}

    //private ModelRows GetRowActive(int row)
    //{
    //    return _activeRows[row];
    //}



    //public void HideElements(bool is_hide)
    //{
    //    HideElements(is_hide, rootWindow);
    //}

    //public void BringFront()
    //{
    //    background.parent.BringToFront();
    //    boxHeader.parent.BringToFront();
    //    boxContent.parent.BringToFront();
    //   // Debug.Log("VringFront");
    //}

    //public void SendToBack()
    //{
    //    background.parent.SendToBack();
    //    boxHeader.parent.SendToBack();
    //    boxContent.parent.SendToBack();
    //}


    //public void HideElements(bool is_hide, VisualElement rootWindows)
    //{
    //    if (is_hide) {
    //        isHide = is_hide;
    //        boxHeader.style.display = DisplayStyle.None;
    //        boxContent.style.display = DisplayStyle.None;
    //        background.style.display = DisplayStyle.None;
    //        rootWindows.style.display = DisplayStyle.None;
    //        if(_mouseOverDetection != null) _mouseOverDetection.Disable();
    //        SendToBack();
    //    } else {
    //        isHide = is_hide;
    //        boxHeader.style.display = DisplayStyle.Flex;
    //        boxContent.style.display = DisplayStyle.Flex;
    //        background.style.display = DisplayStyle.Flex;
    //        rootWindows.style.display = DisplayStyle.Flex;
    //        BringFront();
    //        if (_mouseOverDetection != null) _mouseOverDetection.Enable();

    //    }
    //}

    //public void ChangeMenuSelect(int indexMenu)
    //{
    //    for(int i = 0; i < _menuItems.Length; i++)
    //    {
    //        var item = _menuItems[i];

    //        if(i == indexMenu)
    //        {
    //            var line = item.Q<VisualElement>(className:"line"+ i);
    //            var btn = item.Q<UnityEngine.UIElements.Button>(className: "btn" + i); 
    //            var label1 = item.Q<UnityEngine.UIElements.Label>(className: "label"+i);
    //            HideTabLine(true, line);
    //            bigBtn(btn, label1);

    //        }
    //        else
    //        {
    //            var line = item.Q<VisualElement>(className: "line" + i);
    //            var btn = item.Q<UnityEngine.UIElements.Button>(className: "btn" + i);
    //            var label1 = item.Q<UnityEngine.UIElements.Label>(className: "label"+i);
    //            HideTabLine(false, line);
    //            normBtn(btn, label1);
    //        }
    //    }
    //}


    //public void EquipItem(VisualElement grow)
    //{
    //    if(grow != null)
    //    {
    //        var rows = grow.parent;

    //        var rows_id = rows.name.Replace("Rows", "");
    //        var grow_id = grow.name.Replace("GRow", "");

    //        int parce_int_grow = Int32.Parse(grow_id);
    //        int parce_int_rows = Int32.Parse(rows_id);

    //        VisualElement img = grow.Children().First();

    //        ModelItemDemo demo = _activeRows[parce_int_rows].GetInfo(parce_int_grow);
    //        if(demo.Type2() == 0)
    //        {
    //            EquipToType(img, grow, demo, parce_int_rows, parce_int_grow, "slot_weapon");
    //        }
    //        else if (demo.Type2() == 1)
    //        {
    //            EquipToType(img, grow, demo, parce_int_rows, parce_int_grow , "slot_head");
    //        }

    //    }

    //}

    //private void EquipToType(VisualElement img , VisualElement grow , ModelItemDemo demo , int parce_int_rows , int parce_int_grow , string typeName)
    //{
    //    img.style.backgroundImage = StyleKeyword.None;
    //    grow.style.backgroundImage = StyleKeyword.None;

    //    VisualElement equip_img = boxContent.Q<VisualElement>(className: typeName);
    //    Texture2D blackFrame = Resources.Load<Texture2D>(fillBackground[0]);
    //    equip_img.parent.style.backgroundImage = new StyleBackground(blackFrame);
    //    equip_img.style.backgroundImage = demo.Icon();
    //    DeactiveRow(parce_int_rows, parce_int_grow);
    //    int last_row = _lastSelectRow.rows;
    //    int last_grow = _lastSelectRow.lastActiveGRow;

    //    if (last_row == parce_int_rows & parce_int_rows == last_grow)
    //    {
    //        ClearLastSelect();
    //    }
    //}



    //private void HideTabLine(bool is_hide , VisualElement line)
    //{
    //    if (is_hide)
    //    {
    //        line.style.display = DisplayStyle.None;
    //    }
    //    else
    //    {
    //        line.style.display = DisplayStyle.Flex;
    //    }

    //}

    //private void bigBtn(UnityEngine.UIElements.Button btn , UnityEngine.UIElements.Label label1)
    //{
    //    btn.style.height = 32;
    //    btn.style.top = -3;
    //    label1.style.top = 2;
    //}

    //private void normBtn(UnityEngine.UIElements.Button btn, UnityEngine.UIElements.Label label1)
    //{
    //    btn.style.height = 28;
    //    btn.style.top = 0;
    //    label1.style.top = 0;
    //}

    //public bool isHideWindow()
    //{
    //    return isHide;
    //}

    //public void ToggleHideWindow() {
    //    HideElements(!isHide);
    //}

}
