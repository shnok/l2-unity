using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ActiveSkillsHide : AbstractSkills
{

    
    private SkillLearn _skillLearn;
    public ActiveSkillsHide(SkillLearn _skillLearn)
    {
        this._skillLearn = _skillLearn;
    }

    public void clickDfPhysical(UnityEngine.UIElements.Button btn , VisualElement _activeTab_physicalContent , int[] _arrDfSelect)
    {
        if (_arrDfSelect[0] == 0)
        {
            ChangeDfBox(btn, fillBackgroundDf[0]);
            _arrDfSelect[0] = 1;
            HideSkillbar(true, _activeTab_physicalContent , _skillLearn);
        }
        else
        {
            ChangeDfBox(btn, fillBackgroundDf[1]);
            _arrDfSelect[0] = 0;
            HideSkillbar(false, _activeTab_physicalContent, _skillLearn);
        }
    }


    public void clickDfMagic(UnityEngine.UIElements.Button btn , VisualElement _activeTab_magicContent, int[] _arrDfSelect)
    {
        if (_arrDfSelect[1] == 0)
        {
            ChangeDfBox(btn, fillBackgroundDf[0]);
            _arrDfSelect[1] = 1;
            HideSkillbar(true, _activeTab_magicContent, _skillLearn);
        }
        else
        {
            ChangeDfBox(btn, fillBackgroundDf[1]);
            _arrDfSelect[1] = 0;
            HideSkillbar(false, _activeTab_magicContent, _skillLearn);
        }
    }

    public void clickDfEnhancing(UnityEngine.UIElements.Button btn, VisualElement _activeTab_magicContent, int[] _arrDfSelect)
    {
        if (_arrDfSelect[2] == 0)
        {
            ChangeDfBox(btn, fillBackgroundDf[0]);
            _arrDfSelect[2] = 1;
            HideSkillbar(true, _activeTab_magicContent, _skillLearn);
        }
        else
        {
            ChangeDfBox(btn, fillBackgroundDf[1]);
            _arrDfSelect[2] = 0;
            HideSkillbar(false, _activeTab_magicContent, _skillLearn);
        }
    }

    public void clickDfDebilitating(UnityEngine.UIElements.Button btn, VisualElement _activeTab_debilitatingContent, int[] _arrDfSelect)
    {
        if (_arrDfSelect[3] == 0)
        {
            ChangeDfBox(btn, fillBackgroundDf[0]);
            _arrDfSelect[3] = 1;
            HideSkillbar(true, _activeTab_debilitatingContent, _skillLearn);
        }
        else
        {
            ChangeDfBox(btn, fillBackgroundDf[1]);
            _arrDfSelect[3] = 0;
            HideSkillbar(false, _activeTab_debilitatingContent, _skillLearn);
        }
    }

   
    public void clickDfClan(UnityEngine.UIElements.Button btn, VisualElement _activeTab_ClanContent, int[] _arrDfSelect)
    {
        if (_arrDfSelect[4] == 0)
        {
            ChangeDfBox(btn, fillBackgroundDf[0]);
            _arrDfSelect[4] = 1;
            HideSkillBarClan(true, _activeTab_ClanContent , "SkillBar0");
            HideSkillBarClan(true, _activeTab_ClanContent, "SkillBar1");
        }
        else
        {
            ChangeDfBox(btn, fillBackgroundDf[1]);
            _arrDfSelect[4] = 0;
            HideSkillBarClan(false, _activeTab_ClanContent, "SkillBar0");
            HideSkillBarClan(false, _activeTab_ClanContent, "SkillBar1");

        }
    }



   

    private void HideSkillBarClan(bool hide, VisualElement content , string skillBarName)
    {
        var skillBar = GetSkillBarByName(content, skillBarName);
        if(skillBar != null) _skillLearn.HideElement(hide, skillBar);
    }

   

    private VisualElement GetSkillBarByName(VisualElement content , string skillBarName)
    {
        var childreb = content.Children();

        foreach (VisualElement item in childreb)
        {
            if (item.name.Equals(skillBarName))
            {
                return item;
            }
        }

        return null;
    }



   
}
