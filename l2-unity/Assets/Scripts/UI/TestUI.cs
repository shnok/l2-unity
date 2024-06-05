using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestUI : MonoBehaviour
{
    private VisualTreeAsset _testUITemplate;
    public VisualElement minimal_panel;


    public void Start()
    {
        if (_testUITemplate == null)
        {
            _testUITemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/TestUI");
        }

        if (_testUITemplate == null)
        {
            Debug.LogError("Could not load status window template.");
        }
    }

    private static TestUI _instance;
    public static TestUI Instance
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

        var testUI = _testUITemplate.Instantiate()[0];
        minimal_panel = testUI.Q(className: "testui-panel");

        root.Add(testUI);

    }

 
 
    private void OnDestroy()
    {
        _instance = null;
    }
}
