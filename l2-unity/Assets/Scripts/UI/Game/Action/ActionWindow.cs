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
    private List<ActionSlot> _slots;

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

        _slots = new List<ActionSlot>();

        int position = 0;
        for (int i = 0; i < SLOTS_PER_ROW * 4; i++)
        {
            AddSlot(position++, _basicContainer);
        }

        for (int i = 0; i < SLOTS_PER_ROW * 2; i++)
        {
            AddSlot(position++, _partyContainer);
        }

        for (int i = 0; i < SLOTS_PER_ROW * 2; i++)
        {
            AddSlot(position++, _tokenContainer);
        }

        for (int i = 0; i < SLOTS_PER_ROW * 3; i++)
        {
            AddSlot(position++, _socialContainer);
        }

        _slots[0].AssignAction(ActionType.Sit);
        _slots[1].AssignAction(ActionType.WalkRun);
        _slots[2].AssignAction(ActionType.Attack);
        _slots[3].AssignAction(ActionType.NextTarget);
        _slots[4].AssignAction(ActionType.Pickup);
        _slots[5].AssignAction(ActionType.Assist);
    }

    private void AddSlot(int position, VisualElement container)
    {
        VisualElement slotElement = _slotTemplate.Instantiate()[0];
        container.Add(slotElement);

        ActionSlot slot = new ActionSlot(slotElement, position, L2Slot.SlotType.Action);
        _slots.Add(slot);
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

    /*
//icon action001| id = 0 (Sit)
        var sit = GetElementByClass("image0");
        sit.style.backgroundImage = IconManager.Instance.LoadTextureByName("action001");
        listForToolTips.Add(sit);
        ActionNameTable.Instance.GetAciton(0).Icon = "action001";
        //icon action002| id = 1 (walk)
        var walk = GetElementByClass("image1");
        walk.style.backgroundImage = IconManager.Instance.LoadTextureByName("action002");
        listForToolTips.Add(walk);
        ActionNameTable.Instance.GetAciton(1).Icon = "action002";

        //icon action003| id = 2 (Attack)
        var attack = GetElementByClass("image2");
        attack.style.backgroundImage = IconManager.Instance.LoadTextureByName("action003");
        listForToolTips.Add(attack);
        ActionNameTable.Instance.GetAciton(2).Icon = "action003";

        //icon action005| id = 3 (trade)
        var trade = GetElementByClass("image3");
        trade.style.backgroundImage = IconManager.Instance.LoadTextureByName("action005");
        listForToolTips.Add(trade);
        ActionNameTable.Instance.GetAciton(3).Icon = "action005";

        //icon action007| id = 4 (next_target)
        var next_target = GetElementByClass("image4");
        next_target.style.backgroundImage = IconManager.Instance.LoadTextureByName("action007");
        listForToolTips.Add(next_target);
        ActionNameTable.Instance.GetAciton(4).Icon = "action007";

        //icon action008| id = 5 (pick_up)
        var pick_up = GetElementByClass("image5");
        pick_up.style.backgroundImage = IconManager.Instance.LoadTextureByName("action008");
        listForToolTips.Add(pick_up);
        ActionNameTable.Instance.GetAciton(5).Icon = "action008";

        //icon action010| id = 6 (assist)
        var assist = GetElementByClass("image6");
        assist.style.backgroundImage = IconManager.Instance.LoadTextureByName("action010");
        listForToolTips.Add(assist);
        ActionNameTable.Instance.GetAciton(6).Icon = "action010";

        //icon action018| id = 10 (private_store sell)
        var private_store_sell = GetElementByClass("image10");
        private_store_sell.style.backgroundImage = IconManager.Instance.LoadTextureByName("action018");
        listForToolTips.Add(private_store_sell);
        ActionNameTable.Instance.GetAciton(10).Icon = "action018";

        //icon action028| id = 28 (private_store buy)
        var private_store_buy = GetElementByClass("image28");
        private_store_buy.style.backgroundImage = IconManager.Instance.LoadTextureByName("action028");
        listForToolTips.Add(private_store_buy);
        ActionNameTable.Instance.GetAciton(28).Icon = "action028";

        //icon action040| id = 40 (recommend)
        var recommend = GetElementByClass("image40");
        recommend.style.backgroundImage = IconManager.Instance.LoadTextureByName("action040");
        listForToolTips.Add(recommend);
        ActionNameTable.Instance.GetAciton(40).Icon = "action040";

        //icon action044| id = 55 (recording)
        var recording = GetElementByClass("image55");
        recording.style.backgroundImage = IconManager.Instance.LoadTextureByName("action044");
        listForToolTips.Add(recording);
        ActionNameTable.Instance.GetAciton(55).Icon = "action044";

        //icon action046| id = 57 (find_store)
        var find_store = GetElementByClass("image57");
        find_store.style.backgroundImage = IconManager.Instance.LoadTextureByName("action046");
        listForToolTips.Add(find_store);
        ActionNameTable.Instance.GetAciton(57).Icon = "action046";

        //icon action047| id = 58 (duel)
        var duel = GetElementByClass("image58");
        duel.style.backgroundImage = IconManager.Instance.LoadTextureByName("action047");
        listForToolTips.Add(duel);
        ActionNameTable.Instance.GetAciton(58).Icon = "action047";

        //icon action048| id = 59 (duel_withdrawal)
        var duel_withdrawal = GetElementByClass("image59");
        duel_withdrawal.style.backgroundImage = IconManager.Instance.LoadTextureByName("action048");
        listForToolTips.Add(duel_withdrawal);
        ActionNameTable.Instance.GetAciton(59).Icon = "action048";

        //icon action050| id = 61 (package_sale)
        var package_sale = GetElementByClass("image61");
        package_sale.style.backgroundImage = IconManager.Instance.LoadTextureByName("action050");
        listForToolTips.Add(package_sale);
        ActionNameTable.Instance.GetAciton(61).Icon = "action050";

        //icon action053| id = 64 (my_teleports)
        var my_teleports = GetElementByClass("image64");
        my_teleports.style.backgroundImage = IconManager.Instance.LoadTextureByName("action053");
        listForToolTips.Add(my_teleports);
        ActionNameTable.Instance.GetAciton(64).Icon = "action053";

        //icon Action054| id = 65 (bot_report)
        var bot_report = GetElementByClass("image65");
        bot_report.style.backgroundImage = IconManager.Instance.LoadTextureByName("action054");
        listForToolTips.Add(bot_report);
        ActionNameTable.Instance.GetAciton(65).Icon = "action054";

        //icon action065| id = 76 (invite_friends)
        var invite_friends = GetElementByClass("image76");
        invite_friends.style.backgroundImage = IconManager.Instance.LoadTextureByName("action065");
        listForToolTips.Add(invite_friends);
        ActionNameTable.Instance.GetAciton(76).Icon = "action065";

        //invite_friends action066| id = 77 (start_end_recording)
        var start_end_recording = GetElementByClass("image77");
        start_end_recording.style.backgroundImage = IconManager.Instance.LoadTextureByName("action066");
        listForToolTips.Add(start_end_recording);
        ActionNameTable.Instance.GetAciton(77).Icon = "action066";

        //icon action080| id = 92 (previous_target)
        var previous_target = GetElementByClass("image92");
        previous_target.style.backgroundImage = IconManager.Instance.LoadTextureByName("action080");
        listForToolTips.Add(previous_target);
        ActionNameTable.Instance.GetAciton(92).Icon = "action080";

        //icon action011| id = 7 (rm_invite)
        var rm_invite = GetElementByClass("image7");
        rm_invite.style.backgroundImage = IconManager.Instance.LoadTextureByName("action011");
        listForToolTips.Add(rm_invite);
        ActionNameTable.Instance.GetAciton(7).Icon = "action011";

        //icon action012| id = 8 (leave_arty)
        var leave_arty = GetElementByClass("image8");
        leave_arty.style.backgroundImage = IconManager.Instance.LoadTextureByName("action012");
        listForToolTips.Add(leave_arty);
        ActionNameTable.Instance.GetAciton(8).Icon = "action012";

        //icon action013| id = 9 (dismiss_party_member)
        var dismiss_party_member = GetElementByClass("image9");
        dismiss_party_member.style.backgroundImage = IconManager.Instance.LoadTextureByName("action013");
        listForToolTips.Add(dismiss_party_member);
        ActionNameTable.Instance.GetAciton(9).Icon = "action013";

        //icon action019| id = 11 (party_matching)
        var party_matching = GetElementByClass("image11");
        party_matching.style.backgroundImage = IconManager.Instance.LoadTextureByName("action019");
        listForToolTips.Add(party_matching);
        ActionNameTable.Instance.GetAciton(11).Icon = "action019";

        //icon action041| id = 50 (change_party_leader)
        var change_party_leader = GetElementByClass("image50");
        change_party_leader.style.backgroundImage = IconManager.Instance.LoadTextureByName("action041");
        listForToolTips.Add(change_party_leader);
        ActionNameTable.Instance.GetAciton(50).Icon = "action041";

        //icon action045| id = 56 (command_channel_invite)
        var command_channel_invite = GetElementByClass("image56");
        command_channel_invite.style.backgroundImage = IconManager.Instance.LoadTextureByName("action045");
        listForToolTips.Add(command_channel_invite);
        ActionNameTable.Instance.GetAciton(56).Icon = "action045";

        //icon action067| id = 78 (use_of_token_1)
        var use_of_token_1 = GetElementByClass("image78");
        use_of_token_1.style.backgroundImage = IconManager.Instance.LoadTextureByName("action067");
        listForToolTips.Add(use_of_token_1);
        ActionNameTable.Instance.GetAciton(78).Icon = "action067";

        //icon action068| id = 79 (use_of_token_2)
        var use_of_token_2 = GetElementByClass("image79");
        use_of_token_2.style.backgroundImage = IconManager.Instance.LoadTextureByName("action068");
        listForToolTips.Add(use_of_token_2);
        ActionNameTable.Instance.GetAciton(79).Icon = "action068";

        //icon action069| id = 80 (use_of_token_3)
        var use_of_token_3 = GetElementByClass("image80");
        use_of_token_3.style.backgroundImage = IconManager.Instance.LoadTextureByName("action069");
        listForToolTips.Add(use_of_token_3);
        ActionNameTable.Instance.GetAciton(80).Icon = "action069";

        //icon action070| id = 81 (use_of_token_4)
        var use_of_token_4 = GetElementByClass("image81");
        use_of_token_4.style.backgroundImage = IconManager.Instance.LoadTextureByName("action070");
        listForToolTips.Add(use_of_token_4);
        ActionNameTable.Instance.GetAciton(81).Icon = "action070";

        //icon action071| id = 82 (target_by_token 1)
        var target_by_token_1 = GetElementByClass("image82");
        target_by_token_1.style.backgroundImage = IconManager.Instance.LoadTextureByName("action071");
        listForToolTips.Add(target_by_token_1);
        ActionNameTable.Instance.GetAciton(82).Icon = "action071";

        //icon action072| id = 83 (target_by_token 2)
        var target_by_token_2 = GetElementByClass("image83");
        target_by_token_2.style.backgroundImage = IconManager.Instance.LoadTextureByName("action072");
        listForToolTips.Add(target_by_token_2);
        ActionNameTable.Instance.GetAciton(83).Icon = "action072";

        //icon action073| id = 84 (target_by_token 3)
        var target_by_token_3 = GetElementByClass("image84");
        target_by_token_3.style.backgroundImage = IconManager.Instance.LoadTextureByName("action073");
        listForToolTips.Add(target_by_token_3);
        ActionNameTable.Instance.GetAciton(84).Icon = "action073";

        //icon action074| id = 85 (target_by_token 4)
        var target_by_token_4 = GetElementByClass("image85");
        target_by_token_4.style.backgroundImage = IconManager.Instance.LoadTextureByName("action074");
        listForToolTips.Add(target_by_token_4);
        ActionNameTable.Instance.GetAciton(85).Icon = "action074";

        //icon action015| id = 12 (announcements)
        var announcements = GetElementByClass("image12");
        announcements.style.backgroundImage = IconManager.Instance.LoadTextureByName("action015");
        listForToolTips.Add(announcements);
        ActionNameTable.Instance.GetAciton(12).Icon = "action015";

        //icon action016| id = 13 (victory)
        var victory = GetElementByClass("image13");
        victory.style.backgroundImage = IconManager.Instance.LoadTextureByName("action016");
        listForToolTips.Add(victory);
        ActionNameTable.Instance.GetAciton(13).Icon = "action016";

        //icon action017| id = 14 (advance)
        var advance = GetElementByClass("image14");
        advance.style.backgroundImage = IconManager.Instance.LoadTextureByName("action017");
        listForToolTips.Add(advance);
        ActionNameTable.Instance.GetAciton(14).Icon = "action017";

        //icon action020| id = 24(yes)
        var yes = GetElementByClass("image24");
        yes.style.backgroundImage = IconManager.Instance.LoadTextureByName("action020");
        listForToolTips.Add(yes);
        ActionNameTable.Instance.GetAciton(24).Icon = "action020";

        //icon action021| id = 25(no)
        var no = GetElementByClass("image25");
        no.style.backgroundImage = IconManager.Instance.LoadTextureByName("action021");
        listForToolTips.Add(no);
        ActionNameTable.Instance.GetAciton(25).Icon = "action021";

        //icon action022| id = 26(bow)
        var bow = GetElementByClass("image26");
        bow.style.backgroundImage = IconManager.Instance.LoadTextureByName("action022");
        listForToolTips.Add(bow);
        ActionNameTable.Instance.GetAciton(26).Icon = "action022";

        //icon action029| id = 29(unaware)
        var unaware = GetElementByClass("image29");
        unaware.style.backgroundImage = IconManager.Instance.LoadTextureByName("action029");
        listForToolTips.Add(unaware);
        ActionNameTable.Instance.GetAciton(29).Icon = "action029";

        //icon action030| id = 30(standby)
        var standby = GetElementByClass("image30");
        standby.style.backgroundImage = IconManager.Instance.LoadTextureByName("action030");
        listForToolTips.Add(standby);
        ActionNameTable.Instance.GetAciton(30).Icon = "action030";

        //icon action031| id = 31(laugh)
        var laugh = GetElementByClass("image31");
        laugh.style.backgroundImage = IconManager.Instance.LoadTextureByName("action031");
        listForToolTips.Add(laugh);
        ActionNameTable.Instance.GetAciton(31).Icon = "action031";

        //icon action032| id = 33(applaud)
        var applaud = GetElementByClass("image33");
        applaud.style.backgroundImage = IconManager.Instance.LoadTextureByName("action032");
        listForToolTips.Add(applaud);
        ActionNameTable.Instance.GetAciton(33).Icon = "action032";

        //icon action033| id = 34(dance)
        var dance = GetElementByClass("image34");
        dance.style.backgroundImage = IconManager.Instance.LoadTextureByName("action033");
        listForToolTips.Add(dance);
        ActionNameTable.Instance.GetAciton(34).Icon = "action033";

        //icon action034| id = 35(sorrow)
        var sorrow = GetElementByClass("image35");
        sorrow.style.backgroundImage = IconManager.Instance.LoadTextureByName("action034");
        listForToolTips.Add(sorrow);
        ActionNameTable.Instance.GetAciton(35).Icon = "action034";

        //icon action051| id = 62(charm)
        var charm = GetElementByClass("image62");
        charm.style.backgroundImage = IconManager.Instance.LoadTextureByName("action051");
        listForToolTips.Add(charm);
        ActionNameTable.Instance.GetAciton(62).Icon = "action051";

        //icon action055| id = 66(shyness)
        var shyness = GetElementByClass("image66");
        shyness.style.backgroundImage = IconManager.Instance.LoadTextureByName("action055");
        listForToolTips.Add(shyness);
        ActionNameTable.Instance.GetAciton(66).Icon = "action055";

        //icon action060| id = 71(exchange_bows)
        var exchange_bows = GetElementByClass("image71");
        exchange_bows.style.backgroundImage = IconManager.Instance.LoadTextureByName("action060");
        listForToolTips.Add(exchange_bows);
        ActionNameTable.Instance.GetAciton(61).Icon = "action060";

        //icon action061| id = 72(high_five)
        var high_five = GetElementByClass("image72");
        high_five.style.backgroundImage = IconManager.Instance.LoadTextureByName("action061");
        listForToolTips.Add(high_five);
        ActionNameTable.Instance.GetAciton(72).Icon = "action061";

        //icon action062| id = 73(couple_dance)
        var couple_dance = GetElementByClass("image73");
        couple_dance.style.backgroundImage = IconManager.Instance.LoadTextureByName("action062");
        listForToolTips.Add(couple_dance);
        ActionNameTable.Instance.GetAciton(73).Icon = "action062";

        //icon action077| id = 87(propose_emote)
        var propose_emote = GetElementByClass("image87");
        propose_emote.style.backgroundImage = IconManager.Instance.LoadTextureByName("action077");
        listForToolTips.Add(propose_emote);
        ActionNameTable.Instance.GetAciton(87).Icon = "action077";

        //icon action078| id = 88(provoke_emote)
        var provoke_emote = GetElementByClass("image88");
        provoke_emote.style.backgroundImage = IconManager.Instance.LoadTextureByName("action078");
        listForToolTips.Add(provoke_emote);
        ActionNameTable.Instance.GetAciton(88).Icon = "action078";
    */
}