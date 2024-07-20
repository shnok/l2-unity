using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestWindow : L2PopupWindow
{
    public VisualElement minimal_panel;
    private VisualElement boxContent;
    private VisualElement background;
    private VisualElement boxHeader;
    private VisualElement rootWindow;
    private ButtonQuest _button;
    private static QuestWindow _instance;
    public static QuestWindow Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _button = new ButtonQuest(this);
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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Quest/QuestWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);


        yield return new WaitForEndOfFrame();

        rootWindow = GetElementByClass("root-windows");
        boxHeader = GetElementByClass("drag-area");
       // boxContent = GetElementByClass("action_content");
        background = GetElementByClass("background_over");
        _button.RegisterButtonCloseWindow(rootWindow, "btn-close-frame");
       // _button.RegisterClickWindow(boxContent, boxHeader);

        DragManipulator drag = new DragManipulator(boxHeader, _windowEle);
        boxHeader.AddManipulator(drag);
        HideWindow();
    }
}
