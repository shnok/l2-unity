using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterInfo : MonoBehaviour
{

    private VisualTreeAsset _testUITemplate;
    public VisualElement minimal_panel;


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

    private static CharacterInfo _instance;
    public static CharacterInfo Instance
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
        var windowsFrame = testUI.Q<VisualElement>(null, "drag-area");

        //MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(testUI);
        //testUI.AddManipulator(mouseOverDetection);

        DragManipulator drag = new DragManipulator(windowsFrame, testUI);
        windowsFrame.AddManipulator(drag);

        root.Add(testUI);
        yield return new WaitForEndOfFrame();
    }


    private void OnDestroy()
    {
        _instance = null;
    }


}
