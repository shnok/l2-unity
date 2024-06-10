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
using static UnityEngine.Rendering.DebugUI;




public class ShortCutPanel : MonoBehaviour
{
    private VisualTreeAsset _shortCutWindowsTemplate;
    private VisualElement ve;
    private List<ShortCutPanelMinimal> _minmalPanels;
    private VisualElement shortCutPanel;
    private VisualTreeAsset _shortCutWIndowsMinimalTemplate;
    private VisualElement statusWindowDragArea;
    private DragManipulatorsChildren drag;
    private int sizeCell = 11;
    public ShortCutChildrenModel arrayRowsPanels;
    private string[] arrImgNextButton = { "Data/UI/ShortCut/button/numbers/shortcut_f01" , 
        "Data/UI/ShortCut/button/numbers/shortcut_f02" , 
        "Data/UI/ShortCut/button/numbers/shortcut_f03" , 
        "Data/UI/ShortCut/button/numbers/shortcut_f04" , 
        "Data/UI/ShortCut/button/numbers/shortcut_f05" , 
        "Data/UI/ShortCut/button/numbers/shortcut_f06", 
        "Data/UI/ShortCut/button/numbers/shortcut_f07" };
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
        }
        else
        {
            Destroy(this);
        }
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
    public IEnumerator BuildWindow(VisualElement root)
    {
        shortCutPanel = _shortCutWindowsTemplate.Instantiate()[0];


        MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(shortCutPanel);
        shortCutPanel.AddManipulator(mouseOverDetection);

        statusWindowDragArea = shortCutPanel.Q<VisualElement>(null, "drag-area-shortcut");
        drag = new DragManipulatorsChildren(statusWindowDragArea, shortCutPanel);
        statusWindowDragArea.AddManipulator(drag);


        var rootGroupBox = shortCutPanel.Q<VisualElement>(null, "short-cut");

        var imageIndex = shortCutPanel.Q<VisualElement>(null, "ImageIndex");
        

        //Working code for overlay
        var shortcutImage1 = shortCutPanel.Q<VisualElement>(null, "row0");

        var buttonSlider = shortCutPanel.Q<VisualElement>(null, "button-slider");
      
       // var buttonNext = shortCutPanel.Q<UnityEngine.UIElements.Button>("button_next");
        //var buttonNext = shortCutPanel.Q<UnityEngine.UIElements.Button>("button_preview");


        ClickPanelManipulation panel = new ClickPanelManipulation(shortcutImage1, IconOverlay.Instance);
        shortcutImage1.AddManipulator(panel);



        ClickSliderShortCutManipulator slider = new ClickSliderShortCutManipulator(ShortCutPanelMinimal.Instance, drag);
        buttonSlider.AddManipulator(slider);


        SetImageNumber(imageIndex, showPanelIndex);
        SetImage(shortCutPanel);
        RegisterButtonCallBackNext(imageIndex , "button_next");
        RegisterButtonCallBackPreview(imageIndex , "button_preview");
        // float x_root = root.worldBound.max.x;
        //float y_root = root.worldBound.max.y;
        //InitPosition(x_root, y_root, rootGroupBox);

        root.Add(rootGroupBox);



        yield return new WaitForEndOfFrame();

    }

    private void RegisterButtonCallBackNext( VisualElement imageIndex , string buttonId)
    {
        var btn = shortCutPanel.Q<UnityEngine.UIElements.Button>(buttonId);
        if (btn == null)
        {
            Debug.LogError(buttonId + " can't be found.");
            return;
        }

        btn.RegisterCallback<MouseDownEvent>(evt => {

            ++showPanelIndex;

            if (showPanelIndex >= arrImgNextButton.Length)
            {
                showPanelIndex = 0;
                SetImageNumber(imageIndex, showPanelIndex);
            }
            else
            {
                SetImageNumber(imageIndex, showPanelIndex);
            }
            
            

        }, TrickleDown.TrickleDown);
    }

    private void RegisterButtonCallBackPreview(VisualElement imageIndex, string buttonId)
    {
        var btn = shortCutPanel.Q<UnityEngine.UIElements.Button>(buttonId);
        if (btn == null)
        {
            Debug.LogError(buttonId + " can't be found.");
            return;
        }

        btn.RegisterCallback<MouseDownEvent>(evt => {

            --showPanelIndex;
            if (showPanelIndex < 0)
            {
                showPanelIndex = arrImgNextButton.Length - 1;
                SetImageNumber(imageIndex, showPanelIndex);
            }
            else
            {
                SetImageNumber(imageIndex, showPanelIndex);
            }
            
            
        }, TrickleDown.TrickleDown);
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


    public Texture2D GetImage(string path)
    {
        return  Resources.Load<Texture2D>(path);
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


    public void SetImageNumber(VisualElement imageElement , int index)
    {
            if(index >= arrImgNextButton.Length)
            {
                SetImage(imageElement, GetImage(arrImgNextButton[0]));
            }
            else
            {
                SetImage(imageElement, GetImage(arrImgNextButton[index]));
            }
    }

    private void SetImage(VisualElement imageElement , Texture2D imgSource1)
    {
        if (imgSource1) imageElement.style.backgroundImage = new StyleBackground(imgSource1);
    }

    private void InitPosition(float x_root, float y_root, VisualElement rootGroupBox)
    {
        rootGroupBox.transform.position = new Vector2(x_root - 44, 25);
    }

}