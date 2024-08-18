using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PassiveSkillsHide : AbstractSkills
{
    private SkillLearn _skillLearn;
    public PassiveSkillsHide(SkillLearn _skillLearn)
    {
        this._skillLearn = _skillLearn;
    }

    public void clickDfAbiliti(UnityEngine.UIElements.Button btn, VisualElement _activeTab_debilitatingContent, int[] _arrDfPassiveSelect)
    {
        if (_arrDfPassiveSelect[0] == 0)
        {
            ChangeDfBox(btn, fillBackgroundDf[0]);
            _arrDfPassiveSelect[0] = 1;
            HideSkillbar(true, _activeTab_debilitatingContent , _skillLearn);
        }
        else
        {
            ChangeDfBox(btn, fillBackgroundDf[1]);
            _arrDfPassiveSelect[0] = 0;
            HideSkillbar(false, _activeTab_debilitatingContent ,  _skillLearn);
        }
    }

    public void clickDfSubject(UnityEngine.UIElements.Button btn, VisualElement _activeTab_debilitatingContent, int[] _arrDfPassiveSelect)
    {
        if (_arrDfPassiveSelect[1] == 0)
        {
            ChangeDfBox(btn, fillBackgroundDf[0]);
            _arrDfPassiveSelect[1] = 1;
            HideSkillbar(true, _activeTab_debilitatingContent, _skillLearn);
        }
        else
        {
            ChangeDfBox(btn, fillBackgroundDf[1]);
            _arrDfPassiveSelect[1] = 0;
            HideSkillbar(false, _activeTab_debilitatingContent, _skillLearn);
        }
    }


}
