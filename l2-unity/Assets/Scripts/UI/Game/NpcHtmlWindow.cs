using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class NpcHtmlWindow : L2PopupWindow
{
    private VisualTreeAsset _l2Button;
    private VisualTreeAsset _l2Input;
    private VisualTreeAsset _htmlTable;
    private VisualTreeAsset _htmlRow;
    private VisualTreeAsset _htmlCell;
    private VisualTreeAsset _hyperlink;
    private VisualTreeAsset _htmlWrapper;
    private VisualTreeAsset _l2dropdown;
    private VisualElement _content;
    private ScrollView _scrollView;
    private Label _windowName;
    private bool _centerEverything;
    [SerializeField] private List<HtmlNode> _nodes;
    [SerializeField] private Dictionary<string, string> _inputValues;
    private static NpcHtmlWindow _instance;
    public static NpcHtmlWindow Instance
    {
        get { return _instance; }
    }
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
        _l2Button = LoadAsset("Data/UI/_Elements/Components/L2Button/L2Button");
        _l2Input = LoadAsset("Data/UI/_Elements/Components/L2Input/L2Input");
        _l2dropdown = LoadAsset("Data/UI/_Elements/Components/L2Dropdown/L2Dropdown");
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/NpcHtmlWindow/NpcHtmlWindow");
        _htmlTable = LoadAsset("Data/UI/_Elements/Game/NpcHtmlWindow/HtmlTable");
        _htmlRow = LoadAsset("Data/UI/_Elements/Game/NpcHtmlWindow/HtmlRow");
        _htmlCell = LoadAsset("Data/UI/_Elements/Game/NpcHtmlWindow/HtmlCell");
        _htmlWrapper = LoadAsset("Data/UI/_Elements/Game/NpcHtmlWindow/HtmlWrapper");
        _hyperlink = LoadAsset("Data/UI/_Elements/Game/NpcHtmlWindow/HtmlHyperlink");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle, this);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
    }
    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _windowEle.style.left = new Length(50, LengthUnit.Percent);
        _windowEle.style.top = new Length(50, LengthUnit.Percent);
        _windowEle.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent)));

        _windowName = (Label)GetElementById("windows-name-label");
        _windowName.text = "Chat";

        _scrollView = _windowEle.Q<ScrollView>("HtmlContent");
        var _scroller = _scrollView.verticalScroller;
        var highBtn = _scroller.Q<RepeatButton>("unity-high-button");
        var lowBtn = _scroller.Q<RepeatButton>("unity-low-button");
        _content = _windowEle.Q<VisualElement>("ContentWrapper");

        highBtn.AddManipulator(new ButtonClickSoundManipulator(highBtn));
        lowBtn.AddManipulator(new ButtonClickSoundManipulator(lowBtn));

        HideWindow(true);

        L2GameUI.Instance.WindowLoadComplete();

        //         RefreshContent(0,
        // @"<html><body>Newbie Helper:<br>
        // Welcome to Einhovant's School of Wizardry. I will be teaching you the basics of combat.<br>
        //  Please click on <font color=""LEVEL"">Quest</font>, in your Chat window.<br>
        // <a action=""bypass -h npc_%objectId%_Quest"">Quest</a><br>
        // </body></html>
        // ", 0);
        //         RefreshContent(0,
        // @"<html><title>Server menu</title><body><center><table width=260><tr><td><button value=""Main"" action=""bypass -h admin_admin"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Game"" action=""bypass -h admin_admin 2"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Effects"" action=""bypass -h admin_admin 3"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Server"" action=""bypass -h admin_admin 4"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td></tr></table><img src=""l2ui.SquareWhite"" width=275 height=1><br><table width=240><tr><td>QuickBox</td><td><edit var=""menu_command"" width=120 height=15></td><td><button value=""Help"" action=""bypass -h admin_help"" width=45 height=15 back=""sek.cbui94"" fore=""sek.cbui92""></td></tr></table><br>Reload<table width=240><tr><td><combobox width=160 height=21 var=""cb"" list=admin;announcement;buylist;config;crest;cw;door;htm;item;multisell;npc;npcwalker;script;skill;teleport;zone;></td><td><button value=""Reload"" action=""bypass -h admin_reload $cb"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td></tr></table><br>Zones / Territories<table width=240><tr><td><button value=""Zone Check"" action=""bypass -h admin_zone"" width=75 height=21 back=""L2UI_ch3.Btn1_normalOn"" fore=""L2UI_ch3.Btn1_normal""></td><td><button value=""Maker Check"" action=""bypass -h admin_maker"" width=75 height=21 back=""L2UI_ch3.Btn1_normalOn"" fore=""L2UI_ch3.Btn1_normal""></td><td width=75></td></tr><tr><td><button value=""Territory Goto"" action=""bypass -h admin_terr goto $menu_command"" width=75 height=21 back=""L2UI_ch3.Btn1_normalOn"" fore=""L2UI_ch3.Btn1_normal""></td><td><button value=""Maker Info"" action=""bypass -h admin_maker $menu_command"" width=75 height=21 back=""L2UI_ch3.Btn1_normalOn"" fore=""L2UI_ch3.Btn1_normal""></td><td></td></tr></table><br>GeoEngine<table width=240><tr><td><button value=""Position Info"" action=""bypass -h admin_geo pos"" width=75 height=21 back=""L2UI_ch3.Btn1_normalOn"" fore=""L2UI_ch3.Btn1_normal""></td><td><button value=""Beeline Test"" action=""bypass -h admin_geo move"" width=75 height=21 back=""L2UI_ch3.Btn1_normalOn"" fore=""L2UI_ch3.Btn1_normal""></td><td><button value=""Path Find"" action=""bypass -h admin_path find"" width=75 height=21 back=""L2UI_ch3.Btn1_normalOn"" fore=""L2UI_ch3.Btn1_normal""></td></tr><tr><td><button value=""Path Info"" action=""bypass -h admin_path info"" width=75 height=21 back=""L2UI_ch3.Btn1_normalOn"" fore=""L2UI_ch3.Btn1_normal""></td><td><button value=""LoS Test"" action=""bypass -h admin_geo see"" width=75 height=21 back=""L2UI_ch3.Btn1_normalOn"" fore=""L2UI_ch3.Btn1_normal""></td><td><button value=""Report Geo Bug"" action=""bypass -h admin_geo bug $menu_command"" width=75 height=21 back=""L2UI_ch3.Btn1_normalOn"" fore=""L2UI_ch3.Btn1_normal""></td></tr></table><br>Maintenance / Development<table width=240><tr><td><button value=""Maintenance"" action=""bypass -h admin_server"" width=75 height=21 back=""L2UI_ch3.Btn1_normalOn"" fore=""L2UI_ch3.Btn1_normal""></td><td><button value=""Announce"" action=""bypass -h admin_link announce.htm"" width=75 height=21 back=""L2UI_ch3.Btn1_normalOn"" fore=""L2UI_ch3.Btn1_normal""></td><td><button value=""SysMessage"" action=""bypass -h admin_msg $menu_command"" width=75 height=21 back=""L2UI_ch3.Btn1_normalOn"" fore=""L2UI_ch3.Btn1_normal""></td></tr></table>
        //         </center></body></html>
        //         ", 0);
        //         RefreshContent(0,
        // @"<table width=280 height=45><tr><td width=70 align=center>Search</td><td width=140><edit var=""search"" width=130 height=15></td><td width=70><button value=""Find"" action=""bypass admin_help 1 $search"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td></tr><tr><td></td><td align=center>Found 93 results</td><td></td></tr></table><table width=280 height=41 bgcolor=000000><tr><td width=280 height=34><color=#FFC900>//admin</color> <color=#33cccc>[1-4]</color><br1>Go through the different admin panels.</td></tr></table><img src=""L2UI.SquareGray"" width=280 height=1><table width=280 height=41><tr><td width=280 height=34><color=#FFC900>//buy</color> <color=#33cccc>[id]</color><br1>Open the GM Shop panel, or the associated BuyList.</td></tr></table><img src=""L2UI.SquareGray"" width=280 height=1><table width=280 height=41 bgcolor=000000><tr><td width=280 height=34><color=#FFC900>//camera</color><br1>Toggle the CameraMode.</td></tr></table><img src=""L2UI.SquareGray"" width=280 height=1><table width=280 height=41><tr><td width=280 height=34><color=#FFC900>//gmlist</color><br1>Toggle you from /gmlist results.</td></tr></table><img src=""L2UI.SquareGray"" width=280 height=1><table width=280 height=41 bgcolor=000000><tr><td width=280 height=34><color=#FFC900>//gmoff</color> <color=#33cccc>[duration]</color><br1>Toggle off your GM status, 1min by default.</td></tr></table><img src=""L2UI.SquareGray"" width=280 height=1><table width=280 height=41><tr><td width=280 height=34><color=#FFC900>//help</color> <color=#33cccc>[page]</color><br1>Open this panel.</td></tr></table><img src=""L2UI.SquareGray"" width=280 height=1><table width=280 height=41 bgcolor=000000><tr><td width=280 height=34><color=#FFC900>//link</color> <color=#33cccc>file name</color><br1>Open the proper admin htm.</td></tr></table><img src=""L2UI.SquareGray"" width=280 height=1><table width=280 bgcolor=000000><tr><td FIXWIDTH=22 align=center><img height=2><button action=""bypass admin_help 1 "" back=L2UI_CH3.prev1_down fore=L2UI_CH3.prev1 width=16 height=16></td><td FIXWIDTH=26 align=center></td><td FIXWIDTH=26 align=center></td><td FIXWIDTH=26 align=center></td><td FIXWIDTH=26 align=center></td><td FIXWIDTH=26 align=center><font color=LEVEL>01</font></td><td FIXWIDTH=26 align=center><a action=""bypass admin_help 2 "">02</a></td><td FIXWIDTH=26 align=center><a action=""bypass admin_help 3 "">03</a></td><td FIXWIDTH=26 align=center><a action=""bypass admin_help 4 "">04</a></td><td FIXWIDTH=26 align=center><a action=""bypass admin_help 5 "">05</a></td><td FIXWIDTH=22 align=center><img height=2><button action=""bypass admin_help 14 "" back=L2UI_CH3.next1_down fore=L2UI_CH3.next1 width=16 height=16></td></tr></table><img src=""L2UI.SquareGray"" width=280 height=1>", 0);
        //     RefreshContent(0,
        // @"<title>Main menu</title><center><table width=260><tr><td><button value=""Main"" action=""bypass -h admin_admin"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Game"" action=""bypass -h admin_admin 2"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Effects"" action=""bypass -h admin_admin 3"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Server"" action=""bypass -h admin_admin 4"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td></tr></table><img src=""l2ui.SquareWhite"" width=275 height=1><br><table width=240><tr><td>QuickBox</td><td><edit var=""menu_command"" width=120 height=15></td><td><button value=""Help"" action=""bypass -h admin_help"" width=45 height=15 back=""sek.cbui94"" fore=""sek.cbui92""></td></tr></table><br>Systems<table width=240><tr><td><button value=""Item"" action=""bypass -h admin_item"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""GM Shop"" action=""bypass -h admin_buy"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Teleport"" action=""bypass -h admin_teleport $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Bookmarks"" action=""bypass -h admin_bk $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td></tr><tr><td><button value=""Enchant"" action=""bypass -h admin_enchant $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Petitions"" action=""bypass -h admin_petition"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td></td><td><button value=""Spawnlist"" action=""bypass -h admin_list_spawns $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td></tr></table><br>""Players"" actions<table width=240><tr><td><button value=""Find"" action=""bypass -h admin_find player $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Manage"" action=""bypass -h admin_debug $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Dualbox"" action=""bypass -h admin_find dualbox $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Unban Char."" action=""bypass -h admin_unban player $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td></tr><tr><td><button value=""Tele. To"" action=""bypass -h admin_teleportto $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Recall"" action=""bypass -h admin_recall $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td></td><td><button value=""Unban Acc."" action=""bypass -h admin_unban account $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td></tr></table><br>""Others targets"" actions<table width=240><tr><td><button value=""Kill"" action=""bypass -h admin_kill $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Res"" action=""bypass -h admin_res $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Open"" action=""bypass -h admin_open $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Close"" action=""bypass -h admin_close $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td></tr><tr><td><button value=""Cancel"" action=""bypass -h admin_cancel $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Heal"" action=""bypass -h admin_heal $menu_command"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Full Food"" action=""bypass -h admin_summon food"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Delete"" action=""bypass -h admin_delete"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td></tr></table><br>GM actions<table width=240><tr><td><button value=""List ON/OFF"" action=""bypass -h admin_gmlist"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Invul"" action=""bypass -h admin_invul"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Undying"" action=""bypass -h admin_undying"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td><td><button value=""Hide"" action=""bypass -h admin_hide"" width=65 height=19 back=""L2UI_ch3.smallbutton2_over"" fore=""L2UI_ch3.smallbutton2""></td></tr></table></center>", 0);
        //         RefreshContent(0,
        //         @"<html><body>
        // <center>
        // <table width=280>
        // <tr>
        // <td width=40><button value=""Main"" action=""bypass -h admin_admin"" width=40 height=15 back=""sek.cbui94"" fore=""sek.cbui92""></td>
        // <td width=180 align=center>GMShop</td>
        // <td width=40><button value=""Back"" action=""bypass -h admin_admin"" width=40 height=15 back=""sek.cbui94"" fore=""sek.cbui92""></td>
        // </tr>
        // </table><br>
        // <table>
        // <tr>
        // <td width=120>Attack:</td>
        // <td width=120>Defense:</td>
        // <td width=120>Accesories:</td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_link gmshop/Weapons.htm""><font color=""LEVEL"">Weapons</font></a></td>
        // <td><a action=""bypass -h admin_link gmshop/Armors.htm""><font color=""LEVEL"">Armors</font></a></td>
        // <td><a action=""bypass -h admin_link gmshop/Jewels.htm""><font color=""LEVEL"">Jewels</font></a></td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_buy 2012""><font color=""LEVEL"">Magic Books</font></a></td>
        // <td><a action=""bypass -h admin_item set""><font color=""LEVEL"">Armor sets</font></a></td>
        // <td><a action=""bypass -h admin_buy 9121""><font color=""LEVEL"">Hair</font></a></td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_buy 300527""><font color=""LEVEL"">Fishing Rods</font></a></td>
        // <td><a action=""bypass -h admin_buy 1007""><font color=""LEVEL"">Underwear</font></a></td>
        // <td><a action=""bypass -h admin_buy 1011""><font color=""LEVEL"">Tattoos</font></a></td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_link gmshop/pets.htm""><font color=""LEVEL"">Pets</font></a></td>
        // <td><a action=""bypass -h admin_buy 1006""><font color=""LEVEL"">Cloaks</font></a></td>
        // <td><a action=""bypass -h admin_buy 300533""><font color=""LEVEL"">Event/Echo</font></a></td>
        // </tr>
        // </table><br>
        // <table>
        // <tr>
        // <td width=160>Material:</td>
        // <td width=160>Consumable:</td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_link gmshop/materials.htm""><font color=""LEVEL"">Craft Items</font></a></td>
        // <td><a action=""bypass -h admin_buy 3001""><font color=""LEVEL"">Potions</font></a></td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_link gmshop/recipes.htm""><font color=""LEVEL"">Recipes</font></a></td>
        // <td><a action=""bypass -h admin_link gmshop/Dyes.htm""><font color=""LEVEL"">Dyes</font></a></td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_link gmshop/QuestItems.htm""><font color=""LEVEL"">Quest items</font></a></td>
        // <td><a action=""bypass -h admin_buy 3002""><font color=""LEVEL"">Scrolls</font></a></td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_link gmshop/SpellBooks.htm""><font color=""LEVEL"">SpellBooks</font></a></td>
        // <td><a action=""bypass -h admin_buy 300537""><font color=""LEVEL"">Chests</font></a></td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_buy 300536""><font color=""LEVEL"">Seeds</font></a></td>
        // <td><a action=""bypass -h admin_buy 300529""><font color=""LEVEL"">Tickets</font></a></td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_buy 300532""><font color=""LEVEL"">Fishing Material</font></a></td>
        // <td><a action=""bypass -h admin_link gmshop/Fishing.htm""><font color=""LEVEL"">Fishes</font></a></td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_buy 300534""><font color=""LEVEL"">Crystals/Gemstones</font></a></td>
        // <td><a action=""bypass -h admin_buy 300511""><font color=""LEVEL"">Shots</font></a></td>
        // </tr>
        // <tr>
        // <td></td>
        // <td><a action=""bypass -h admin_buy 300543""><font color=""LEVEL"">Life Stones</font></a></td>
        // </tr>
        // </table>
        // <p><br></p>
        // <table>
        // <tr><td width=160>C5 Items:</td><td width=160></td></tr>
        // <tr>
        // <td><a action=""bypass -h admin_buy 71025""><font color=""LEVEL"">Weapons w/o SA</font></a></td>
        // <td><a action=""bypass -h admin_buy 71021""><font color=""LEVEL"">Armors</font></a></td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_buy 71022""><font color=""LEVEL"">Weapons with SA</a></td>
        // <td><a action=""bypass -h admin_buy 71024""><font color=""LEVEL"">Jewels</font></a></td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_buy 71026""><font color=""LEVEL"">Monsters only weapons</font></a></td>
        // <td><a action=""bypass -h admin_buy 71023""><font color=""LEVEL"">Hair Accesories</font></a></td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_buy 71027""><font color=""LEVEL"">Spellbooks</font></a></td>
        // <td><a action=""bypass -h admin_buy 71029""><font color=""LEVEL"">Tickets</font></td>
        // </tr>
        // <tr>
        // <td><a action=""bypass -h admin_buy 71030""><font color=""LEVEL"">Recipes</font></a></td>
        // <td><a action=""bypass -h admin_buy 71028""><font color=""LEVEL"">Seeds</font></a></td>
        // </tr>
        // </table>
        // <p><br></p>
        // <table>
        // <tr><td width=160><font color=""FFFFFF"">Interlude Items:</font></td><td width=160></td></tr>
        // <tr>
        // <td><a action=""bypass -h admin_buy 71031""><font color=""LEVEL"">Weapons</font></a></td>
        // <td><a action=""bypass -h admin_buy 71032""><font color=""LEVEL"">Hair Accesories</font></a></td>
        // </tr>
        // </table>
        // </center>
        // </body></html>
        // ", 0);
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        AudioManager.Instance.PlayUISound("window_open");
        L2GameUI.Instance.WindowOpened(this);

        ShopWindow.Instance.HideWindow(false);
    }

    public override void HideWindow(bool silent)
    {
        base.HideWindow(silent);

        if (!silent && !_isWindowHidden)
            AudioManager.Instance.PlayUISound("window_close");

        L2GameUI.Instance.WindowClosed(this);
    }

    public void RefreshContent(int npcId, string htmlString, int itemId)
    {
        ShowWindow();

        _content.Clear();

        _inputValues = new Dictionary<string, string>();

        string processedHtml = PreProcessHtml(htmlString, npcId, itemId);
        ProcessHtmlContent(_content, processedHtml);
    }

    private string PreProcessHtml(string html, int npcId, int itemId)
    {
        // Remove newlines and normalize spaces
        string processed = Regex.Replace(html, @"\s{2,}", "");

        if (processed.Contains("<center>"))
        {
            _centerEverything = true;
            _content.style.alignContent = Align.Center;
            _content.style.justifyContent = Justify.Center;
        }
        else
        {
            _centerEverything = false;
            _content.style.alignContent = Align.FlexStart;
            _content.style.justifyContent = Justify.FlexStart;
        }

        processed = processed
            .Replace("<html>", "")
            .Replace("</html>", "")
            .Replace("<body>", "")
            .Replace("</body>", "")
            .Replace("%objectId%", npcId.ToString())
            .Replace("%item%", itemId.ToString())
            .Replace("<center>", "")
            .Replace("</center>", "")
            .Replace("<p>", "<br1>")
            .Replace("</p>", "<br1>");

        processed = ReplaceItemNames(processed);
        processed = ReplaceSysStrings(processed);
        processed = ReplaceFontColors(processed);

        return processed;
    }

    private string ReplaceItemNames(string processed)
    {
        processed = Regex.Replace(processed, @"\&\#(\d+);", match =>
                {
                    // Extract the captured number
                    string number = match.Groups[1].Value;

                    AbstractItem item = ItemTable.Instance.GetItem(int.Parse(number));
                    if (item != null)
                    {
                        return item.ItemName.Name;
                    }
                    else
                    {
                        return "<Unknown item>";
                    }
                });

        return processed;
    }

    private string ReplaceSysStrings(string processed)
    {
        processed = Regex.Replace(processed, @"\&\$(\d+);", match =>
                {
                    // Extract the captured number
                    string number = match.Groups[1].Value;

                    SysStringData sysString = SysStringTable.Instance.GetSysString(int.Parse(number));
                    if (sysString != null)
                    {
                        return sysString.Name;
                    }
                    else
                    {
                        return "<Unknown SysString>";
                    }
                });

        return processed;
    }


    private string ReplaceFontColors(string processed)
    {
        // Process font color tags while preserving Unity rich text format
        processed = Regex.Replace(
            processed,
            @"<font color=(?:""|')?([^""'>]+?)(?:""|')?>(.*?)</font>",
            match =>
            {
                string colorValue = match.Groups[1].Value;
                string content = match.Groups[2].Value;
                return colorValue == "LEVEL"
                    ? $"<color=#FFC900>{content}</color>"
                    : $"<color=#{colorValue}>{content}</color>";
            }
        );

        return processed;
    }

    private void ProcessHtmlContent(VisualElement container, string html)
    {
        // Debug.LogWarning(StringUtils.Base64Encode(html));

        _nodes = ParseHtmlIntoNodes(html);
        var currentTextBuilder = new StringBuilder();

        foreach (HtmlNode node in _nodes)
        {
            if (node.Type == NodeType.Text)
            {
                currentTextBuilder.Append(node.Content);
            }
            else
            {
                // If we have accumulated text, add it as a text element
                if (currentTextBuilder.Length > 0)
                {
                    AddTextElement(container, currentTextBuilder.ToString());
                    currentTextBuilder.Clear();
                }
                ProcessNode(container, node);
            }
        }

        // Add any remaining text
        if (currentTextBuilder.Length > 0)
        {
            AddTextElement(container, currentTextBuilder.ToString());
        }
    }

    private List<HtmlNode> ParseHtmlIntoNodes(string html)
    {
        var nodes = new List<HtmlNode>();
        int currentPos = 0;
        var textBuilder = new StringBuilder();

        while (currentPos < html.Length)
        {
            if (html[currentPos] == '<')
            {
                int tagEnd = html.IndexOf('>', currentPos);
                if (tagEnd == -1) break;

                string tag = html.Substring(currentPos, tagEnd - currentPos + 1);

                if (tag.StartsWith("<title"))
                {
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    int linkEnd = html.IndexOf("</title>", currentPos);
                    if (linkEnd != -1)
                    {
                        string linkTag = html.Substring(currentPos, linkEnd - currentPos);

                        string content = linkTag.Substring(Mathf.Clamp(tagEnd + 1 - currentPos, 0, 7));

                        currentPos = currentPos + (linkEnd - currentPos) + 8;

                        _windowName.text = content;
                    }
                    else currentPos = tagEnd + 1;
                }
                else if (tag.StartsWith("<br1"))
                {
                    // If we have accumulated text, add it as a node
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    nodes.Add(new HtmlNode { Type = NodeType.LineBreakSmall });
                    currentPos = tagEnd + 1;
                }
                else if (tag.StartsWith("<br"))
                {
                    // If we have accumulated text, add it as a node
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    nodes.Add(new HtmlNode { Type = NodeType.LineBreak });
                    currentPos = tagEnd + 1;
                }
                else if (tag.StartsWith("<img"))
                {
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    HtmlNode imgNode = ExtractNode(html, ">", NodeType.Image, tagEnd, ref currentPos);
                    if (imgNode != null)
                    {
                        nodes.Add(imgNode);
                    }
                }
                else if (tag.StartsWith("<button "))
                {
                    // If we have accumulated text, add it as a node
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    HtmlNode buttonNode = ExtractNode(html, ">", NodeType.Button, tagEnd, ref currentPos);
                    if (buttonNode != null)
                    {
                        nodes.Add(buttonNode);
                    }
                }
                else if (tag.StartsWith("<edit"))
                {
                    // If we have accumulated text, add it as a node
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    HtmlNode inputNode = ExtractNode(html, ">", NodeType.InputField, tagEnd, ref currentPos);
                    if (inputNode != null)
                    {
                        nodes.Add(inputNode);
                    }
                }
                else if (tag.StartsWith("<combobox"))
                {
                    // If we have accumulated text, add it as a node
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    HtmlNode inputNode = ExtractNode(html, ">", NodeType.DropList, tagEnd, ref currentPos);
                    if (inputNode != null)
                    {
                        nodes.Add(inputNode);
                    }
                }
                else if (tag.StartsWith("<table"))
                {
                    // If we have accumulated text, add it as a node
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    HtmlNode tableNode = ExtractNode(html, "</table>", NodeType.Table, tagEnd, ref currentPos);
                    if (tableNode != null)
                    {
                        nodes.Add(tableNode);
                    }
                }
                else if (tag.StartsWith("<a "))
                {
                    // If we have accumulated text, add it as a node
                    if (textBuilder.Length > 0)
                    {
                        nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
                        textBuilder.Clear();
                    }

                    int linkEnd = html.IndexOf("</a>", currentPos);
                    if (linkEnd != -1)
                    {
                        string linkTag = html.Substring(currentPos, linkEnd - currentPos);
                        Match actionMatch = Regex.Match(linkTag, @"action=""([^""]+)"".*?>(.*?)$");
                        if (actionMatch.Success)
                        {
                            nodes.Add(new HtmlNode
                            {
                                Type = NodeType.Link,
                                Attributes = new Dictionary<string, string>
                                {
                                    ["action"] = actionMatch.Groups[1].Value.Replace("bypass", "").Replace("-h", "").Trim(),
                                    ["value"] = actionMatch.Groups[2].Value
                                }
                            });
                        }
                        currentPos = linkEnd + 4;
                    }
                    else currentPos = tagEnd + 1;
                }
                else
                {
                    // For <br> and <color> tags, include them in the text content
                    textBuilder.Append(tag);
                    currentPos = tagEnd + 1;
                }
            }
            else
            {
                textBuilder.Append(html[currentPos]);
                currentPos++;
            }
        }

        // Add any remaining text
        if (textBuilder.Length > 0)
        {
            nodes.Add(new HtmlNode { Type = NodeType.Text, Content = textBuilder.ToString() });
        }

        return nodes;
    }

    private HtmlNode ExtractNode(string html, string tag, NodeType nodeType, int tagEnd, ref int currentPos)
    {
        int linkEnd = html.IndexOf(tag, currentPos);
        if (linkEnd != -1)
        {
            string linkTag = html.Substring(currentPos, linkEnd - currentPos);

            HtmlNode node = new HtmlNode
            {
                Type = nodeType,
                Attributes = ExtractAttributes(linkTag.Substring(0, tagEnd - currentPos)),
                Content = linkTag.Substring(Mathf.Clamp(tagEnd + 1 - currentPos, 0, linkTag.Length - 1))
            };

            currentPos = currentPos + (linkEnd - currentPos) + tag.Length;
            return node;
        }

        currentPos = tagEnd + 1;
        return null;
    }

    private Dictionary<string, string> ExtractAttributes(string value)
    {
        Dictionary<string, string> attributes = new Dictionary<string, string>();

        Dictionary<string, string> patterns = new Dictionary<string, string>
        {
            ["action"] = @"action=[""']?([^""'>]+)[""']?",
            ["back"] = @"back=[""']?([^""'>\s]+)[""']?",
            ["fore"] = @"fore=[""']?([^""'>\s]+)[""']?",
            ["width"] = @"width=[""']?([^""'>\s]+)[""']?",
            ["height"] = @"height=[""']?([^""'>\s]+)[""']?",
            ["bgcolor"] = @"bgcolor=[""']?([^""'>\s]+)[""']?",
            ["value"] = @"value=[""']([^""']*)[""']",
            ["src"] = @"src=[""']?([^""'>\s]+)[""']?",
            ["var"] = @"var=[""']?([^""'>\s]+)[""']?",
            ["list"] = @"list=[""']?([^""'>\s]+)[""']?"
        };

        foreach (KeyValuePair<string, string> pattern in patterns)
        {
            Match match = Regex.Match(value, pattern.Value);
            if (match.Success)
            {
                string attributeValue = match.Groups[1].Value;
                if (pattern.Key == "action")
                {
                    attributeValue = attributeValue.Replace("bypass", "").Replace("-h", "").Trim();
                }
                attributes[pattern.Key] = attributeValue;
            }
        }

        return attributes;
    }

    private void ProcessNode(VisualElement container, HtmlNode node)
    {
        if (_centerEverything)
        {
            container.style.alignContent = Align.Center;
            container.style.justifyContent = Justify.Center;
        }

        switch (node.Type)
        {
            case NodeType.Text:
                AddTextElement(container, node.Content);
                break;
            case NodeType.DropList:
                AddDropList(container, node.Attributes);
                break;
            case NodeType.Image:
                AddImageElement(container, node.Attributes);
                break;
            case NodeType.LineBreak:
                AddLineBreak(container, false);
                break;
            case NodeType.LineBreakSmall:
                AddLineBreak(container, true);
                break;
            case NodeType.Table:
                ProcessTable(container, node.Attributes, node.Content);
                break;
            case NodeType.Button:
                AddButton(container, node.Attributes);
                break;
            case NodeType.InputField:
                AddInputField(container, node.Attributes);
                break;
            case NodeType.Link:
                AddHyperlinkElement(container, node.Attributes);
                break;
        }
    }

    private void AddDropList(VisualElement container, Dictionary<string, string> attributes)
    {
        DropdownField dropdown = (DropdownField)_l2dropdown.Instantiate()[0];

        if (attributes.TryGetValue("width", out string width))
        {
            int widthPx = int.Parse(width) + 0;
            dropdown.style.minWidth = Math.Max(40, widthPx - 50);
            dropdown.style.width = Math.Max(40, widthPx - 50);
        }
        if (attributes.TryGetValue("height", out string height))
        {
            int heightPx = int.Parse(height);
            dropdown.style.minHeight = heightPx - 4;
            dropdown.style.height = heightPx - 4;
        }

        if (attributes.TryGetValue("list", out string list))
        {
            if (list != null && list.Length > 0)
            {
                dropdown.choices.AddRange(list.Split(";"));
            }
        }

        dropdown.index = 0;
        if (attributes.TryGetValue("var", out string dropdownIndex))
        {
            _inputValues.TryAdd(dropdownIndex, dropdown.text);
            dropdown.RegisterValueChangedCallback((newVal) =>
            {
                _inputValues[dropdownIndex] = newVal.newValue;

                AudioManager.Instance.PlayUISound("window_open");
            });

            dropdown.AddManipulator(new ButtonClickSoundManipulator(dropdown));
        }

        container.Add(dropdown);
    }

    private void AddLineBreak(VisualElement container, bool small)
    {
        var spacer = _htmlWrapper.Instantiate()[0];
        if (small)
        {
            spacer.style.height = 0;
        }
        else
        {
            spacer.style.height = 8;
        }
        container.Add(spacer);
    }

    private void AddTextElement(VisualElement container, string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        Label label = new Label { text = text };
        label.enableRichText = true;
        label.AddToClassList("l2-color-4");
        label.AddToClassList("html-window-label");

        container.Add(label);
    }

    private void AddHyperlinkElement(VisualElement container, Dictionary<string, string> attributes)
    {
        Button button = (Button)_hyperlink.Instantiate()[0];
        VisualElement buttonLabel = button.Q<Label>("ButtonLabel");
        VisualElement buttonBg = button.Q<VisualElement>("ButtonBg");
        Label label = button.Q<Label>("ButtonLabel");

        if (attributes.TryGetValue("width", out string width))
        {
            button.style.width = int.Parse(width);
        }
        if (attributes.TryGetValue("height", out string height))
        {
            button.style.height = int.Parse(height);
        }
        if (attributes.TryGetValue("action", out string action))
        {
            button.clicked += () => ButtonClicked(action);
        }
        if (attributes.TryGetValue("value", out string value))
        {
            label.text = "<u>" + value + "</u>";
        }
        else
        {
            label.text = "<u>...</u>";
        }

        container.Add(button);
    }

    private void AddButton(VisualElement container, Dictionary<string, string> attributes)
    {
        Button button = (Button)_l2Button.Instantiate()[0];
        Label label = button.Q<Label>("ButtonLabel");

        if (attributes.TryGetValue("width", out string width))
        {
            button.style.minWidth = int.Parse(width) - 5;
            button.style.width = int.Parse(width) - 5;
        }
        if (attributes.TryGetValue("height", out string height))
        {
            button.style.minHeight = int.Parse(height) + 3;
            button.style.height = int.Parse(height) + 3;
        }
        if (attributes.TryGetValue("action", out string action))
        {
            button.clicked += () => ButtonClicked(action);
        }
        if (attributes.TryGetValue("value", out string value))
        {
            label.text = value;
        }
        else
        {
            button.style.width = 20;
            if (attributes.TryGetValue("fore", out string fore))
            {
                if (fore == "L2UI_CH3.next1")
                {
                    label.text = ">";
                }
                else if (fore == "L2UI_CH3.prev1")
                {
                    label.text = "<";
                }
                else
                {
                    label.text = "*";
                }
            }
            else
            {
                label.text = "*";
            }
        }

        button.AddManipulator(new ButtonClickSoundManipulator(button));

        container.Add(button);
    }

    private void AddInputField(VisualElement container, Dictionary<string, string> attributes)
    {
        TextField textField = (TextField)_l2Input.Instantiate()[0];

        if (attributes.TryGetValue("width", out string width))
        {
            int widthPx = int.Parse(width) + 0;
            textField.style.minWidth = Math.Max(40, widthPx - 50);
            textField.style.width = Math.Max(40, widthPx - 50);
        }
        if (attributes.TryGetValue("height", out string height))
        {
            int heightPx = int.Parse(height) + 4;
            textField.style.minHeight = heightPx;
            textField.style.height = heightPx;
        }

        if (attributes.TryGetValue("var", out string dropdownIndex))
        {
            textField.RegisterValueChangedCallback((newVal) =>
            {
                _inputValues[dropdownIndex] = newVal.newValue;
            });
        }

        textField.AddManipulator(new BlinkingCursorManipulator(textField));

        container.Add(textField);
    }

    private void AddImageElement(VisualElement container, Dictionary<string, string> attributes)
    {
        VisualElement imageContainer = new VisualElement();

        Texture2D exampleImage = Resources.Load<Texture2D>("Data/SysTextures/Icon/accessary_magic_ring_i00");

        Image image = new Image();
        image.scaleMode = ScaleMode.StretchToFill;

        if (attributes.TryGetValue("src", out string src))
        {
            Texture2D srcImage = Resources.Load<Texture2D>("Data/UI/Assets/html/" + src.Split(".")[1]);
            if (srcImage != null)
            {
                image.image = srcImage;
            }
            else
            {
                image.image = exampleImage;
            }
        }
        if (attributes.TryGetValue("width", out string width))
        {
            int widthPx = int.Parse(width);
            imageContainer.style.minWidth = widthPx;
            imageContainer.style.maxWidth = widthPx;
            image.style.minWidth = widthPx;
            image.style.maxWidth = widthPx;
        }

        if (attributes.TryGetValue("height", out string height))
        {
            int heigthPx = int.Parse(height);
            imageContainer.style.maxHeight = heigthPx;
            imageContainer.style.minHeight = heigthPx;
            image.style.maxHeight = heigthPx;
            image.style.minHeight = heigthPx;
        }

        imageContainer.Add(image);

        imageContainer.style.flexShrink = new StyleFloat(StyleKeyword.None);

        container.Add(imageContainer);
    }

    private void ProcessTable(VisualElement container, Dictionary<string, string> attributes, string tableHtml)
    {
        VisualElement wrapper = _htmlWrapper.Instantiate()[0];

        VisualElement tableContainer = _htmlTable.Instantiate()[0];

        if (attributes.TryGetValue("height", out string tableHeight))
        {
            tableContainer.style.height = int.Parse(tableHeight);
        }
        if (attributes.TryGetValue("width", out string tableWidth))
        {
            tableContainer.style.width = int.Parse(tableWidth);
        }
        else
        {
            tableContainer.style.width = new Length(100f, LengthUnit.Percent);
        }
        if (attributes.TryGetValue("bgcolor", out string color))
        {
            tableContainer.style.backgroundColor = ColorUtils.HexToColor(color + "FF");
        }
        if (_centerEverything)
        {
            tableContainer.style.alignSelf = Align.Center;
        }

        // Table content
        var rows = Regex.Matches(tableHtml, @"<tr>(.*?)</tr>", RegexOptions.Singleline);
        int colCount = 0;

        foreach (Match row in rows)
        {
            VisualElement rowElement = _htmlRow.Instantiate()[0];

            // Extract attributes individually
            string widthPattern = @"width=(\d+)";
            string heightPattern = @"height=(\d+)";
            string fixWidthPattern = @"FIXWIDTH=(\d+)";
            string alignPattern = @"align=(left|center|right|justify)";
            string contentPattern = @"<td[^>]*>(.*?)</td>";

            var cells = Regex.Matches(row.Groups[1].Value, contentPattern, RegexOptions.Singleline);

            foreach (Match cell in cells)
            {
                if (colCount == 0)
                {
                    colCount = cells.Count;
                }

                VisualElement cellElement = _htmlCell.Instantiate()[0];

                string cellText = cell.Value;
                string firstCell = cellText.Substring(0, cellText.IndexOf(">"));

                // Extract width
                var widthMatch = Regex.Match(firstCell, widthPattern);
                string width = widthMatch.Success ? widthMatch.Groups[1].Value : "";

                // Extract width2
                var fixWidthMatch = Regex.Match(firstCell, fixWidthPattern);
                string width2 = fixWidthMatch.Success ? fixWidthMatch.Groups[1].Value : "";

                // Extract height
                var heightMatch = Regex.Match(firstCell, heightPattern);
                string height = heightMatch.Success ? heightMatch.Groups[1].Value : "";

                // Extract align
                var alignMatch = Regex.Match(firstCell, alignPattern);
                string align = alignMatch.Success ? alignMatch.Groups[1].Value : "";

                // Extract content
                var contentMatch = Regex.Match(cellText, contentPattern);
                string content = contentMatch.Success ? contentMatch.Groups[1].Value : "";

                int customWidth = 0;
                if (width.Length > 0)
                {
                    customWidth = int.Parse(width);// * 0.788f;// - 36;
                }
                else if (width2.Length > 0)
                {
                    customWidth = int.Parse(width2);// * 0.788f;// - 36;
                }

                if (customWidth == 0)
                {
                    cellElement.style.width = new Length(100f / colCount, LengthUnit.Percent);
                }
                else if (customWidth < 120)
                {
                    cellElement.style.width = customWidth;
                    cellElement.style.flexGrow = 0;
                }


                if (height.Length > 0)
                {
                    cellElement.style.height = int.Parse(height);
                }

                switch (align)
                {
                    case "left":
                        cellElement.style.alignItems = Align.FlexStart;
                        break;
                    case "center":
                        cellElement.style.alignItems = Align.Center;
                        break;
                    case "right":
                        cellElement.style.alignItems = Align.FlexEnd;
                        break;
                }

                rowElement.Add(cellElement);

                ProcessHtmlContent(cellElement, content);
            }

            tableContainer.Add(rowElement);
        }

        wrapper.Add(tableContainer);
        container.Add(wrapper);
    }

    private void ButtonClicked(string action)
    {
        Debug.Log(action);
        if (action.Contains("$"))
        {
            string pattern = @"\$\w+";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(action);

            if (matches.Count > 0)
            {
                string id = matches[matches.Count - 1].Value;
                Debug.Log(id[1..]);
                if (_inputValues.TryGetValue(id[1..], out string value))
                {
                    action = action.Replace(id, value);
                }
            }
        }
        Debug.Log(action);

        GameClient.Instance.ClientPacketHandler.RequestBypassToServer(action);
    }

    private enum NodeType
    {
        Text,
        LineBreak,
        LineBreakSmall,
        Image,
        Table,
        Link,
        Button,
        InputField,
        DropList
    }

    [System.Serializable]
    private class HtmlNode
    {
        public NodeType Type { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
    }
}
