using Assets.Scripts.UI;
using Assets.Scripts.UI.Manipulators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;



public class ShortCutPanel : MonoBehaviour
{
    private VisualTreeAsset _shortCutWindowsTemplate;
    private VisualElement ve;
    private List<ShortCutPanelMinimal> _minmalPanels;
    private VisualTreeAsset _shortCutWIndowsMinimalTemplate;
    private VisualElement statusWindowDragArea;
    private DragManipulatorsChildren drag;

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

    private void OnDestroy()
    {
        _instance = null;
    }

    void Start()
    {
        if (_shortCutWindowsTemplate == null)
        {
            _shortCutWindowsTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/ShortCutPanel");
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
        var shortCutPanel = _shortCutWindowsTemplate.Instantiate()[0];


        MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(shortCutPanel);
        shortCutPanel.AddManipulator(mouseOverDetection);
        
        statusWindowDragArea = shortCutPanel.Q<VisualElement>(null, "drag-area-shortcut");
        drag = new DragManipulatorsChildren(statusWindowDragArea, shortCutPanel);
        statusWindowDragArea.AddManipulator(drag);


        var rootGroupBox = shortCutPanel.Q<VisualElement>(null, "short-cut");
        var shortcutImage1 = shortCutPanel.Q<VisualElement>(null, "shortcutimage");

        var buttonSlider = shortCutPanel.Q<VisualElement>(null, "button-slider");
        

        ClickPanelManipulation panel = new ClickPanelManipulation(shortcutImage1, IconOverlay.Instance);
        shortcutImage1.AddManipulator(panel);



        ClickSliderShortCutManipulator slider = new ClickSliderShortCutManipulator(shortcutImage1, ShortCutPanelMinimal.Instance);
        buttonSlider.AddManipulator(slider);

       // float x_root = root.worldBound.max.x;
        //float y_root = root.worldBound.max.y;
        //InitPosition(x_root, y_root, rootGroupBox);

        root.Add(shortCutPanel);

       

        yield return new WaitForEndOfFrame();

    }

    private void InitPosition(float x_root, float y_root, VisualElement rootGroupBox)
    {
        rootGroupBox.transform.position = new Vector2(x_root - 44, 25);
    }

}