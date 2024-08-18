using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class AbstractSkills
{
    protected string[] fillBackgroundDf = { "Data/UI/Window/Skills/QuestWndPlusBtn_v2", "Data/UI/Window/Skills/Button_DF_Skills_Down_v3" };
    protected void ChangeDfBox(Button btn, string texture)
    {
        IEnumerable<VisualElement> children = btn.Children();
        var e = children.First();
        e.style.display = DisplayStyle.Flex;
        Texture2D iconDfNoraml = LoadTextureDF(texture);
        setBackgroundDf(btn, iconDfNoraml);
    }
    protected void HideSkillbar(bool hide, VisualElement content , SkillLearn _skillLearn)
    {
        var skillBar = GetSkillBar(content);
        _skillLearn.HideElement(hide, skillBar);
    }

    protected void setBackgroundDf(UnityEngine.UIElements.Button btn, Texture2D iconDfNoraml)
    {
        btn.style.backgroundImage = new StyleBackground(iconDfNoraml);
    }

    protected Texture2D LoadTextureDF(string path)
    {
        return Resources.Load<Texture2D>(path);
    }

    protected VisualElement GetSkillBar(VisualElement content)
    {
        var childreb = content.Children();
        int i = 0;
        foreach (VisualElement item in childreb)
        {
            if (i > 0)
            {
                return item;
            }
            i++;
        }

        return null;
    }
}
