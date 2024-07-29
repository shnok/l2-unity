using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolTipManager : MonoBehaviour
{

    private static ToolTipManager _instance;

    public static ToolTipManager Instance { get { return _instance; } }

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

    public void RegisterCallbackSkills(List<VisualElement> list)
    {
        ToolTipSkill.Instance.RegisterCallback(list);
    }

    public void RegisterCallbackActions(List<VisualElement> list)
    {
        ToolTipAction.Instance.RegisterCallback(list);
    }

  
}
