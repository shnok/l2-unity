using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterInfoWindow : MonoBehaviour
{

    private VisualTreeAsset _testUITemplate;
    public VisualElement minimal_panel;
    private bool isHide;
    private VisualElement rootWindow;
    private VisualElement content;
    private ButtonCharacter _buttonCharacter;
    private VisualElement _templateDefaultWindows;
    private MouseOverDetectionManipulator _mouseOverDetection;


    public void Start()
    {
        if (_testUITemplate == null)
        {
            _testUITemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Windows/CharacterInfo");
        }

        if (_testUITemplate == null)
        {
            Debug.LogError("Could not load status window template.");
        }
    }

    private static CharacterInfoWindow _instance;
    public static CharacterInfoWindow Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _buttonCharacter = new ButtonCharacter(this);
        }
        else
        {
            Destroy(this);
        }
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


       
         rootWindow = testUI.Q<VisualElement>(className: "windows");

         var dragArea = testUI.Q<VisualElement>(className: "drag-area");
         content = testUI.Q<VisualElement>(className: "content");
        _buttonCharacter.RegisterButtonCloseWindow(rootWindow, "btn-close-frame");
        _buttonCharacter.RegisterClickWindow(rootWindow, content, dragArea);

        _mouseOverDetection = new MouseOverDetectionManipulator(rootWindow);
        testUI.AddManipulator(_mouseOverDetection);

        DragManipulator drag = new DragManipulator(dragArea, testUI);
        dragArea.AddManipulator(drag);
        HideElements(true);

        root.Add(testUI);
        yield return new WaitForEndOfFrame();
    }

    public void BringFront()
    {
        rootWindow.parent.BringToFront();
        content.parent.BringToFront();
    }

    public void SendToBack()
    {
        rootWindow.parent.SendToBack();
        content.parent.SendToBack();
    }

    public void HideElements(bool is_hide)
    {
        HideElements(is_hide, rootWindow);
    }

    public void HideElements(bool is_hide, VisualElement rootWindow) {
        if (is_hide) {
            _mouseOverDetection.Disable();
            isHide = is_hide;
            content.style.display = DisplayStyle.None;
            rootWindow.style.display = DisplayStyle.None;
            SendToBack();
        } else {
            _mouseOverDetection.Enable();
            isHide = is_hide;
            content.style.display = DisplayStyle.Flex;
            rootWindow.style.display = DisplayStyle.Flex;
            BringFront();
        }
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
