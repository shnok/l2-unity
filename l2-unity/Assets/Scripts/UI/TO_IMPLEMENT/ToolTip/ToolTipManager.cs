#if FALSE
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

    public void RegisterCallbackActiveSkills(Dictionary<int, VisualElement> dict  , SkillLearn skillWindow)
    {
        ToolTipSkill.Instance.RegisterCallbackActive(dict, skillWindow);
    }

    public void RegisterCallbackPassiveSkills(Dictionary<int, VisualElement> dict, SkillLearn skillWindow)
    {
        ToolTipSkill.Instance.RegisterCallbackPassive(dict, skillWindow);
    }

    public void RegisterCallbackActions(List<VisualElement> list)
    {
        ToolTipAction.Instance.RegisterCallback(list);
    }

  
}
#endif
