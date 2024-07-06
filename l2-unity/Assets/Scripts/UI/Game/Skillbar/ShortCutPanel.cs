using Assets.Scripts.UI;
using Assets.Scripts.UI.Manipulators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;





public class ShortCutPanel : MonoBehaviour , IShortCutButton
{
    private VisualTreeAsset _shortCutWindowsTemplate;
    private VisualElement shortCutPanelElements;
    private VisualTreeAsset _shortCutWIndowsMinimalTemplate;
    private VisualElement statusWindowDragArea;
    private DragManipulatorsChildren drag;
    private int sizeCell = 11;
    public ShortCutChildrenModel arrayRowsPanels;
    private ShortCutButton shortCutButton;
    private VisualElement rootGroupBox;
    private VisualElement buttonSlider;
    private ShortCutReplacePanel replacePanel;
    private bool isVertical;
    private ClickSliderShortCutManipulator slider;



    private string[] arrImgNextButton = new string[6]; 
 
    private int showPanelIndex = 0;

    private static ShortCutPanel _instance;
    public static ShortCutPanel Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            CretaeDemoInfo();
            shortCutButton = new ShortCutButton(this , sizeCell);
            replacePanel = new ShortCutReplacePanel(sizeCell , arrayRowsPanels);
            InitArrImgNumbers();
            isVertical = true;
        }
        else
        {
            Destroy(this);
        }
    }

    private void InitArrImgNumbers()
    {
        arrImgNextButton[0] = "Data/UI/ShortCut/button/numbers/shortcut_f01";
        arrImgNextButton[1] = "Data/UI/ShortCut/button/numbers/shortcut_f02";
        arrImgNextButton[2] = "Data/UI/ShortCut/button/numbers/shortcut_f03";
        arrImgNextButton[3] = "Data/UI/ShortCut/button/numbers/shortcut_f04";
        arrImgNextButton[4] = "Data/UI/ShortCut/button/numbers/shortcut_f05";
        arrImgNextButton[5] = "Data/UI/ShortCut/button/numbers/shortcut_f06";
       // arrImgNextButton[6] = "Data/UI/ShortCut/button/numbers/shortcut_f07";
    }

    public void  SetPositionVerical(bool vertical)
    {
        this.isVertical = vertical;
    }

    public bool IsVertical()
    {
        return this.isVertical;
    }
    public string[] GetArrImgNextButton()
    {
        return arrImgNextButton; 
    }

    public int GetShowPanelIndex()
    {
        return showPanelIndex;
    }

    public void SetPanelIndex(int index)
    {
        showPanelIndex = index;
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    void Start()
    {
        if (_shortCutWindowsTemplate == null)
        {
            _shortCutWindowsTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Game/Skillbar/SkillbarWindow");
           
        }
        if (_shortCutWindowsTemplate == null)
        {
            Debug.LogError("Could not load status window template.");
        }
    }

    public void AddWindow(VisualElement root)
    {
        if (_shortCutWindowsTemplate == null & _shortCutWIndowsMinimalTemplate)
        {
            return;
        }

        //BuildWindow(root);
        StartCoroutine(BuildWindow(root));
    }

    public Vector2 GetPositionRoot()
    {
        if (statusWindowDragArea == null) return new Vector2(0, 0);
        if(isVertical) return statusWindowDragArea.worldBound.position;
        if (!isVertical)
        {
            return new Vector2(statusWindowDragArea.worldBound.position.x + 235, statusWindowDragArea.worldBound.position.y - 235);
        }

        return new Vector2(0, 0);
    }

    public VisualElement GetRootVisiaulElement()
    {
        return statusWindowDragArea;
    }

    public void SetDrugChildren(VisualElement[] children)
    {
        drag.SetChildren(children);
    }

    public DragManipulatorsChildren GetDrag()
    {
        return drag;
    }

    public VisualElement GetShortCutPanelElements()
    {
        return shortCutPanelElements;
    }
    public IEnumerator BuildWindow(VisualElement root)
    {
        shortCutPanelElements = _shortCutWindowsTemplate.Instantiate()[0];
        replacePanel.SetRootPanel(shortCutPanelElements);

        MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(shortCutPanelElements);
        shortCutPanelElements.AddManipulator(mouseOverDetection);

        statusWindowDragArea = shortCutPanelElements.Q<VisualElement>(null, "drag-area-shortcut");

        drag = new DragManipulatorsChildren(statusWindowDragArea, shortCutPanelElements);
        statusWindowDragArea.AddManipulator(drag);


        rootGroupBox = shortCutPanelElements.Q(className:"short-cut");

        var imageIndex = shortCutPanelElements.Q<VisualElement>(null, "ImageIndex");
        

        //Working code for overlay
        var shortcutImage1 = shortCutPanelElements.Q<VisualElement>(null, "row0");

        buttonSlider = shortCutPanelElements.Q<VisualElement>(null, "button-slider");




        ClickPanelManipulation panel = new ClickPanelManipulation(shortcutImage1, IconOverlay.Instance);
        shortcutImage1.AddManipulator(panel);



        slider = new ClickSliderShortCutManipulator(ShortCutPanelMinimal.Instance, drag , isVertical);
        buttonSlider.AddManipulator(slider);




        shortCutButton.SetImageNumber(this , imageIndex, showPanelIndex);
        //set root cell
        replacePanel.SetImage(shortCutPanelElements);

        shortCutButton.RegisterButtonCallBackNext(imageIndex , "button_next");
        shortCutButton.RegisterButtonCallBackPreview(imageIndex , "button_preview");
        shortCutButton.RegisterButtonCallBackRotate(rootGroupBox , slider , "visualRotate");
        root.Add(shortCutPanelElements);
        yield return new WaitForEndOfFrame();

    }

    public ClickSliderShortCutManipulator GetActiveManipulatorSlider()
    {
        return slider;
    }

    public VisualElement GetSliderVisualElement()
    {
        return buttonSlider;
    }

    public void ReplaceLeftSliderToRightSlider()
    {
        if (isVertical)
        {
            if (buttonSlider.ClassListContains("button-slider")){
                buttonSlider.RemoveFromClassList("button-slider");
                buttonSlider.AddToClassList("button-slider-right");
            }
        }
        else
        {
            var buttonSliderHorizontal = rootGroupBox.Q<VisualElement>(className: "slide-hor-arrow");

            if (buttonSliderHorizontal != null)
            {
                buttonSliderHorizontal.RemoveFromClassList("slide-hor-arrow");
                buttonSliderHorizontal.AddToClassList("button-slider-left");
            }
        }
    
       
    }

    public void ReplaceRightSliderToLeftSlider()
    {
        if (isVertical)
        {
            if (buttonSlider.ClassListContains("button-slider-right"))
            {
                buttonSlider.RemoveFromClassList("button-slider-right");
                buttonSlider.AddToClassList("button-slider");
            }
        }
        else
        {

            var buttonSliderHorizontal = rootGroupBox.Q<VisualElement>(className: "button-slider-left");

            if (buttonSliderHorizontal != null)
            {
                buttonSliderHorizontal.RemoveFromClassList("button-slider-left");
                buttonSliderHorizontal.AddToClassList("slide-hor-arrow");
            }
        }
     
          
    }

    //rootGroupBox - use this class not use argumets
    public void NextPanelToRootPanel(VisualElement disable, int disablepanel)
    {
        replacePanel.SetImageNext(sizeCell, rootGroupBox, showPanelIndex);
    }



  
    public ShortCutChildrenModel GetShortCutChildrenModel()
    {
        return arrayRowsPanels;
    }



    private void CretaeDemoInfo()
    {
        for (int i = 0; i < sizeCell; i++)
        {

            int[] row = { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
            string[] img = { "Data/UI/ShortCut/demo_skills/skill0915",
                "Data/UI/ShortCut/demo_skills/skill0914",
                "Data/UI/ShortCut/demo_skills/skill0914",
                "Data/UI/ShortCut/demo_skills/skill0914",
                "Data/UI/ShortCut/demo_skills/skill0914",
                "Data/UI/ShortCut/demo_skills/skill0914",
                "Data/UI/ShortCut/demo_skills/skill0915",
                "Data/UI/ShortCut/demo_skills/skill0915",
                "Data/UI/ShortCut/demo_skills/skill0914",
                "Data/UI/ShortCut/demo_skills/skill0914",
                "Data/UI/ShortCut/demo_skills/skill0914",
                "Data/UI/ShortCut/demo_skills/skill0914" };
            arrayRowsPanels = new ShortCutChildrenModel(row, img);
        }
    }


}