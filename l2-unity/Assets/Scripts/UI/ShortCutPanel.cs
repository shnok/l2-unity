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
using static UnityEditor.Rendering.FilterWindow;
using static UnityEngine.Rendering.DebugUI;




public class ShortCutPanel : MonoBehaviour
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



    private string[] arrImgNextButton = new string[6]; 
    //-1 current panel
    private int showPanelIndex = 0;

    // [SerializeField] private float _statusWindowMinWidth = 700.0f;
    //[SerializeField] private float _statusWindowMaxWidth = 800.0f;


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
            shortCutButton = new ShortCutButton(this);
            InitArrImgNumbers();
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
            _shortCutWindowsTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/ShortCutPanel");
            CretaeDemoInfo();
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
        return statusWindowDragArea.worldBound.position;
    }

    public void setDrugChildren(VisualElement[] children)
    {
        drag.setChildren(children);
    }

    public VisualElement GetShortCutPanelElements()
    {
        return shortCutPanelElements;
    }
    public IEnumerator BuildWindow(VisualElement root)
    {
        shortCutPanelElements = _shortCutWindowsTemplate.Instantiate()[0];


        MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(shortCutPanelElements);
        shortCutPanelElements.AddManipulator(mouseOverDetection);

        statusWindowDragArea = shortCutPanelElements.Q<VisualElement>(null, "drag-area-shortcut");
        drag = new DragManipulatorsChildren(statusWindowDragArea, shortCutPanelElements);
        statusWindowDragArea.AddManipulator(drag);


        rootGroupBox = shortCutPanelElements.Q<VisualElement>(null, "short-cut");

        var imageIndex = shortCutPanelElements.Q<VisualElement>(null, "ImageIndex");
        

        //Working code for overlay
        var shortcutImage1 = shortCutPanelElements.Q<VisualElement>(null, "row0");

        buttonSlider = shortCutPanelElements.Q<VisualElement>(null, "button-slider");



        ClickPanelManipulation panel = new ClickPanelManipulation(shortcutImage1, IconOverlay.Instance);
        shortcutImage1.AddManipulator(panel);



        ClickSliderShortCutManipulator slider = new ClickSliderShortCutManipulator(ShortCutPanelMinimal.Instance, drag);
        buttonSlider.AddManipulator(slider);


        shortCutButton.SetImageNumber(imageIndex, showPanelIndex);
        //set root cell
        SetImage(shortCutPanelElements);

        shortCutButton.RegisterButtonCallBackNext(imageIndex , "button_next");
        shortCutButton.RegisterButtonCallBackPreview(imageIndex , "button_preview");


        root.Add(rootGroupBox);



        yield return new WaitForEndOfFrame();

    }

    public void ReplaceLeftSliderToRightSlider()
    {
        buttonSlider.RemoveFromClassList("button-slider");
        buttonSlider.AddToClassList("button-slider-right");
    }

    public void ReplaceRightSliderToLeftSlider()
    {
        buttonSlider.RemoveFromClassList("button-slider-right");
        buttonSlider.AddToClassList("button-slider");
    }
    
    public void NextPanelToRootPanel()
    {
        SetImageNext(rootGroupBox , showPanelIndex);
    }

    private void SetImageNext(VisualElement rootGroupBox , int activePanel)
    {
        for (int cell = 0; cell <= sizeCell; cell++)
        {
            var row = rootGroupBox.Q(className: "row" + cell);
            var border = GetBorderRow(rootGroupBox, cell);
            SetImageNext(border, row, cell , activePanel);
        }
    }


    private void SetImage(VisualElement rootGroupBox)
    {
        for (int cell = 0; cell <= sizeCell; cell++)
        {
            var row = rootGroupBox.Q(className: "row" + cell);
            var border = GetBorderRow(rootGroupBox, cell);
            SetRows(border, row, cell);
        }
    }


    private void SetRows(VisualElement border, VisualElement row, int id_path)
    {
        SetImage(border, row, id_path);
    }

    private void VisibleBorder(VisualElement border, bool show)
    {
        if (border != null) border.visible = show;
    }

    private VisualElement GetBorderRow(VisualElement shortCutMinimal, int index)
    {
        return shortCutMinimal.Q(className: "border_row" + index);
    }

    private void SetImage(VisualElement border, VisualElement row, int id_path)
    {
        string path = arrayRowsPanels.GetRowImgPath(id_path);
        Texture2D imgSource1 = Resources.Load<Texture2D>(path);
        if (imgSource1 != null)
        {
            VisibleBorder(border, true);
            row.style.backgroundImage = new StyleBackground(imgSource1);
        }
    }

    private void SetImageNext(VisualElement border, VisualElement row, int id_path , int activePanel)
    {
        // 0 root panel
        if(activePanel == 0)
        {
            SetImage(shortCutPanelElements);
        }
        else
        {
            //children minimal panels
            ShortCutChildrenModel[] childrenArrayPanels = ShortCutPanelMinimal.Instance.GetArrayRowsPanels();
            activePanel = activePanel - 1;
            if (activePanel <= childrenArrayPanels.Length - 1)
            {
                string path = childrenArrayPanels[activePanel].GetRowImgPath(id_path);
                Texture2D imgSource1 = Resources.Load<Texture2D>(path);
                if (imgSource1 != null)
                {
                    VisibleBorder(border, true);
                    row.style.backgroundImage = new StyleBackground(imgSource1);
                }
                else
                {
                    VisibleBorder(border, false);
                    row.style.backgroundImage = new StyleBackground();
                }
            }

        }
       
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