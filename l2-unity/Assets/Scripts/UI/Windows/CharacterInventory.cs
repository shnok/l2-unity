using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterInventory : MonoBehaviour
{

    private VisualTreeAsset _testUITemplate;
    public VisualElement minimal_panel;
    private ButtonInventory _buttonInventory;
    private VisualElement boxHeader;
    private VisualElement boxContent;
    private VisualElement background;
    private VisualElement rootWindow;
    private bool isHide;


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

    private static CharacterInventory _instance;
    public static CharacterInventory Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _buttonInventory = new ButtonInventory(this);
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
         boxHeader = testUI.Q<VisualElement>(className:"drag-area");
         boxContent = testUI.Q<VisualElement>(className: "inventory_content");
         background = testUI.Q<VisualElement>(className: "background_over");


         rootWindow = testUI.Q<VisualElement>(className: "root_windows");

        //MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(testUI);
        //testUI.AddManipulator(mouseOverDetection);

        DragManipulator drag = new DragManipulator(boxHeader, testUI);
        boxHeader.AddManipulator(drag);
        
        _buttonInventory.RegisterButtonCloseWindow(rootWindow, "btn-close-frame");


        HideElements(true);
        root.Add(testUI);
        yield return new WaitForEndOfFrame();
    }

    public void HideElements(bool is_hide)
    {
        HideElements(is_hide, rootWindow);
    }

    public void HideElements(bool is_hide, VisualElement rootWindows)
    {
        if (is_hide)
        {
           isHide = is_hide;
           boxHeader.style.display = DisplayStyle.None;
           boxContent.style.display = DisplayStyle.None;
           background.style.display = DisplayStyle.None;
           rootWindows.style.display = DisplayStyle.None;
        }
        else
        {
            isHide = is_hide;
            boxHeader.style.display = DisplayStyle.Flex;
            boxContent.style.display = DisplayStyle.Flex;
            background.style.display = DisplayStyle.Flex;
            rootWindows.style.display = DisplayStyle.Flex;
            //rootWindows.style.display = DisplayStyle.Flex;
        }
    }

    public bool isHideWindow()
    {
        return isHide;
    }
    private void OnDestroy()
    {
        _instance = null;
    }


}
