using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class CharacterInventoryWindow : MonoBehaviour
{

    private VisualTreeAsset _testUITemplate;
    public VisualElement minimal_panel;
    private ButtonInventory _buttonInventory;
    private VisualElement boxHeader;
    private VisualElement boxContent;
    private VisualElement background;
    private VisualElement rootWindow;
    private VisualElement[] _menuItems;
    private VisualElement[] _inventoryRows;
    private bool isHide;
    private ModelRows[] _activeRows;
    private ModelRows _lastSelectRow;
    private MouseOverDetectionManipulator _mouseOverDetection;

    /// <summary>
    /// /TEST DATA
    /// </summary>
    string[] data = { "Data/UI/Window/Inventory/demo_image_row/accessary_ring_of_holy_spirit_i00",
            "Data/UI/Window/Inventory/demo_image_row/shield_another_worlds_shield_i00",
            "Data/UI/Window/Inventory/demo_image_row/armor_helmet_i00" };

    string[] fillBackground = { "Data/UI/ShortCut/demo_skills/fill_black", "Data/UI/Window/Inventory/bg_windows/blue_tab_v5" };



    public void Start()
    {
        if (_testUITemplate == null)
        {
            _testUITemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Windows/CharacterInventory");
        }

        if (_testUITemplate == null)
        {
            Debug.LogError("Could not load status window template.");
        }
    }

    private static CharacterInventoryWindow _instance;
    public static CharacterInventoryWindow Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _buttonInventory = new ButtonInventory(this);
            _menuItems = new VisualElement[6];
            _inventoryRows = new VisualElement[8];
            CreateAcive();

        }
        else
        {
            Destroy(this);
        }
    }

    private void CreateAcive()
    {
            var model0 = new ModelRows(0, new int[9]);
            var model1 = new ModelRows(1, new int[9]);
            var model2 = new ModelRows(2, new int[9]);
            var model3 = new ModelRows(3, new int[9]);
            var model4 = new ModelRows(4, new int[9]);
            var model5 = new ModelRows(5, new int[9]);
            var model6 = new ModelRows(6, new int[9]);
            var model7 = new ModelRows(7, new int[9]);

           _activeRows = new ModelRows[8]{ model0 , model1,model2 ,model3 ,model4, model5, model6, model7 };
    }

    public void AddWindow(VisualElement root)
    {
        if (_testUITemplate == null)
        {
            return;
        }


        StartCoroutine(BuildWindow(root));
    }


    public IEnumerator BuildWindow(VisualElement root)
    {
        var testUI = _testUITemplate.Instantiate()[0];
         boxHeader = testUI.Q<VisualElement>(className:"drag-area");
         boxContent = testUI.Q<VisualElement>(className: "inventory_content");
         background = testUI.Q<VisualElement>(className: "background_over");


         rootWindow = testUI.Q<VisualElement>(className: "root_windows");
         CreateTab(boxContent, _menuItems);
         CreateRowsInventory(boxContent, _inventoryRows);
         RegisterClickAllRows(_inventoryRows);
         InitTestData(_inventoryRows);
         ChangeMenuSelect(0);

        _mouseOverDetection = new MouseOverDetectionManipulator(rootWindow);
        testUI.AddManipulator(_mouseOverDetection);

        DragManipulator drag = new DragManipulator(boxHeader, testUI);
        boxHeader.AddManipulator(drag);
        
        _buttonInventory.RegisterButtonCloseWindow(rootWindow, "btn-close-frame");
        _buttonInventory.RegisterClickWindow(boxContent, boxHeader);

        //Menu CLick
        _buttonInventory.RegisterClickMenuAll(_menuItems[0]);
        _buttonInventory.RegisterClickMenuEquip(_menuItems[1]);
        _buttonInventory.RegisterClickMenuSupplies(_menuItems[2]);
        _buttonInventory.RegisterClickMenuCrafting(_menuItems[3]);
        _buttonInventory.RegisterClickMenuMisc(_menuItems[4]);
        _buttonInventory.RegisterClickMenuQuest(_menuItems[5]);

        HideElements(true);
        root.Add(testUI);
        yield return new WaitForEndOfFrame();
    }

    private void AddActive(int grows_id , int imgbox_id)
    {
        _activeRows[grows_id].gArr[imgbox_id] = 1;
    }

    private void RemoveActive(int grows_id, int imgbox_id)
    {
        _activeRows[grows_id].gArr[imgbox_id] = 0;
    }
    private VisualElement[] CreateTab(VisualElement boxContent, VisualElement[] _menuItems)
    {
        _menuItems[0] = boxContent.Q<VisualElement>(className: "alltab");
        _menuItems[1] = boxContent.Q<VisualElement>(className: "equiptab");
        _menuItems[2] = boxContent.Q<VisualElement>(className: "suppliestab");
        _menuItems[3] = boxContent.Q<VisualElement>(className: "craftingtab");
        _menuItems[4] = boxContent.Q<VisualElement>(className: "misctab");
        _menuItems[5] = boxContent.Q<VisualElement>(className: "questtab");
        return _menuItems;
    }


    private VisualElement[] CreateRowsInventory(VisualElement boxContent, VisualElement[] _inventoryRows)
    {
        _inventoryRows[0] = boxContent.Q<VisualElement>(className: "rows0");
        _inventoryRows[1] = boxContent.Q<VisualElement>(className: "rows1");
        _inventoryRows[2] = boxContent.Q<VisualElement>(className: "rows2");
        _inventoryRows[3] = boxContent.Q<VisualElement>(className: "rows3");
        _inventoryRows[4] = boxContent.Q<VisualElement>(className: "rows4");
        _inventoryRows[5] = boxContent.Q<VisualElement>(className: "rows5");
        _inventoryRows[6] = boxContent.Q<VisualElement>(className: "rows6");
        _inventoryRows[7] = boxContent.Q<VisualElement>(className: "rows7");

        return _menuItems;
    }

    private void RegisterClickAllRows(VisualElement[] _inventoryRows)
    {
        for(int i=0; i <_inventoryRows.Length; i++)
        {
            
            var rows = _inventoryRows[i];
            if (rows != null)
            {
                var grow0 = rows.Q<VisualElement>(className: "grow0");
                var grow1 = rows.Q<VisualElement>(className: "grow1");
                var grow2 = rows.Q<VisualElement>(className: "grow2");
                var grow3 = rows.Q<VisualElement>(className: "grow3");
                var grow4 = rows.Q<VisualElement>(className: "grow4");
                var grow5 = rows.Q<VisualElement>(className: "grow5");
                var grow6 = rows.Q<VisualElement>(className: "grow6");
                var grow7 = rows.Q<VisualElement>(className: "grow7");
                var grow8 = rows.Q<VisualElement>(className: "grow8");

                _buttonInventory.RegisterClickInventoryCell(grow0);
                _buttonInventory.RegisterClickInventoryCell(grow1);
                _buttonInventory.RegisterClickInventoryCell(grow2);
                _buttonInventory.RegisterClickInventoryCell(grow3);
                _buttonInventory.RegisterClickInventoryCell(grow4);
                _buttonInventory.RegisterClickInventoryCell(grow5);
                _buttonInventory.RegisterClickInventoryCell(grow6);
                _buttonInventory.RegisterClickInventoryCell(grow7);
                _buttonInventory.RegisterClickInventoryCell(grow8);

            }

        }
    }

    public void InitTestData(VisualElement[] _inventoryRows)
    {
   

        var rows0 = _inventoryRows[0];
        var grow0 = rows0.Q<VisualElement>(className: "grow0");
        var grow1 = rows0.Q<VisualElement>(className: "grow1");
        var grow2 = rows0.Q<VisualElement>(className: "grow2");

        var img0 = rows0.Q<VisualElement>(className: "imgbox0");
        var img1 = rows0.Q<VisualElement>(className: "imgbox1");
        var img2 = rows0.Q<VisualElement>(className: "imgbox2");

        Texture2D fill = Resources.Load<Texture2D>(fillBackground[0]);

        Texture2D imgSource1 = Resources.Load<Texture2D>(data[0]);
        Texture2D imgSource2 = Resources.Load<Texture2D>(data[1]);
        Texture2D imgSource3 = Resources.Load<Texture2D>(data[2]);

        SetBackground(fill, grow0);
        SetBackground(fill, grow1);
        SetBackground(fill, grow2);

        SetBackground(imgSource1, img0);
        SetBackground(imgSource2, img1);
        SetBackground(imgSource3, img2);

        AddActive(0, 0);
        AddActive(0, 1);
        AddActive(0, 2);
    }

    private void SetBackground(Texture2D imgSource1 , VisualElement element)
    {
        if (imgSource1 != null)
        {
            element.style.backgroundImage = new StyleBackground(imgSource1);
        }
    }

    public void SelectRows(VisualElement grow)
    {
        if(grow != null)
        {
           
            var rows = grow.parent;

            var rows_id = rows.name.Replace("Rows", "");
            var grow_id = grow.name.Replace("GRow", "");

            int parce_int_grow = Int32.Parse(grow_id);
            int parce_int_rows = Int32.Parse(rows_id);

            if(IsActiveRow(parce_int_rows, parce_int_grow))
            {

                UpdateBackGroundSelectElement(grow);

                if (_lastSelectRow == null)
                {
                    UpdateLastPosition(parce_int_rows, parce_int_grow);
                }
                else
                {
                    int last_row = _lastSelectRow.rows;
                    int last_grow = _lastSelectRow.lastActiveGRow;

                    UpdateBackGroundLastElement(last_row, last_grow);
                    UpdateLastPosition(parce_int_rows, parce_int_grow);

                }
            }
        }
       
    }

    private void UpdateBackGroundSelectElement(VisualElement grow)
    {
        Texture2D blueFrame = Resources.Load<Texture2D>(fillBackground[1]);
        SetBackground(blueFrame, grow);
    }
    private void UpdateBackGroundLastElement(int last_row , int last_grow)
    {
        VisualElement row = _inventoryRows[last_row];
        var grow_elem = row.Q<VisualElement>(className: "grow" + last_grow);
        Texture2D blackFrame = Resources.Load<Texture2D>(fillBackground[0]);
        SetBackground(blackFrame, grow_elem);

    }
    private void UpdateLastPosition(int parce_int_rows , int parce_int_grow)
    {
        var model = GetRowActive(parce_int_rows);
        model.lastActiveGRow = parce_int_grow;
        _lastSelectRow = model;
    }
    private bool IsActiveRow(int row , int grow)
    {
        if(_activeRows[row].gArr[grow] == 1){
            return true;
        }
        return false;
    }

    private ModelRows GetRowActive(int row)
    {
        return _activeRows[row];
    }



    public void HideElements(bool is_hide)
    {
        HideElements(is_hide, rootWindow);
    }

    public void BringFront()
    {
        background.parent.BringToFront();
        boxHeader.parent.BringToFront();
        boxContent.parent.BringToFront();
       // Debug.Log("VringFront");
    }

    public void SendToBack()
    {
        background.parent.SendToBack();
        boxHeader.parent.SendToBack();
        boxContent.parent.SendToBack();
    }

 
    public void HideElements(bool is_hide, VisualElement rootWindows)
    {
        if (is_hide) {
            isHide = is_hide;
            boxHeader.style.display = DisplayStyle.None;
            boxContent.style.display = DisplayStyle.None;
            background.style.display = DisplayStyle.None;
            rootWindows.style.display = DisplayStyle.None;
            _mouseOverDetection.Disable();
            SendToBack();
        } else {
            isHide = is_hide;
            boxHeader.style.display = DisplayStyle.Flex;
            boxContent.style.display = DisplayStyle.Flex;
            background.style.display = DisplayStyle.Flex;
            rootWindows.style.display = DisplayStyle.Flex;
            BringFront();
            _mouseOverDetection.Enable();

        }
    }

    public void ChangeMenuSelect(int indexMenu)
    {
        for(int i = 0; i < _menuItems.Length; i++)
        {
            var item = _menuItems[i];
            
            if(i == indexMenu)
            {
                var line = item.Q<VisualElement>(className:"line"+ i);
                var btn = item.Q<UnityEngine.UIElements.Button>(className: "btn" + i); 
                var label1 = item.Q<UnityEngine.UIElements.Label>(className: "label"+i);
                HideTabLine(true, line);
                bigBtn(btn, label1);

            }
            else
            {
                var line = item.Q<VisualElement>(className: "line" + i);
                var btn = item.Q<UnityEngine.UIElements.Button>(className: "btn" + i);
                var label1 = item.Q<UnityEngine.UIElements.Label>(className: "label"+i);
                HideTabLine(false, line);
                normBtn(btn, label1);
            }
        }
    }

    private void HideTabLine(bool is_hide , VisualElement line)
    {
        if (is_hide)
        {
            line.style.display = DisplayStyle.None;
        }
        else
        {
            line.style.display = DisplayStyle.Flex;
        }
        
    }

    private void bigBtn(UnityEngine.UIElements.Button btn , UnityEngine.UIElements.Label label1)
    {
        btn.style.height = 32;
        btn.style.top = -3;
        label1.style.top = 2;
    }

    private void normBtn(UnityEngine.UIElements.Button btn, UnityEngine.UIElements.Label label1)
    {
        btn.style.height = 28;
        btn.style.top = 0;
        label1.style.top = 0;
    }

    public bool isHideWindow()
    {
        return isHide;
    }

    public void ToggleHideWindow() {
        HideElements(!isHide);
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}
