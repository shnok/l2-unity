using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonSkillLearn
{
    private SkillLearn _skill;
    public ButtonSkillLearn(SkillLearn skill)
    {
        this._skill = skill;
    }


    public void RegisterButtonCloseWindow(VisualElement rootWindows, string buttonId)
    {
        var btn = rootWindows.Q<Button>(className: buttonId);
        if (btn == null) { Debug.LogError(buttonId + " can't be found."); return; }
        btn.RegisterCallback<MouseUpEvent>(evt => _skill.HideWindow(), TrickleDown.TrickleDown);
    }

    public void RegisterClickCloseButton(Button button)
    {


        if (button == null)
        {
            Debug.LogError(button + " can't be found.");
            return;
        }

        button.RegisterCallback<MouseDownEvent>(evt => {
           // AudioManager.Instance.PlayUISound("click_01");
            _skill.HideWindow();
        }, TrickleDown.TrickleDown);
    }

    public void RegisterClickWindow(VisualElement contentId, VisualElement headerId)
    {

        if (contentId == null | headerId == null)
        {
            Debug.LogError(contentId + " can't be found.");
            return;
        }

        contentId.RegisterCallback<MouseDownEvent>(evt => {
            _skill.BringToFront();

        }, TrickleDown.TrickleDown);

        headerId.RegisterCallback<MouseDownEvent>(evt => {

            _skill.BringToFront();
        }, TrickleDown.TrickleDown);
    }

    public void RegisterClickAction(VisualElement tabElement)
    {


        if (tabElement == null)
        {
            Debug.LogError(tabElement + " can't be found.");
            return;
        }

        tabElement.RegisterCallback<MouseDownEvent>(evt => {
            _skill.ChangeMenuSelect(0);
            AudioManager.Instance.PlayUISound("click_01");
        }, TrickleDown.TrickleDown);
    }

    public void RegisterClickPassive(VisualElement tabElement)
    {


        if (tabElement == null)
        {
            Debug.LogError(tabElement + " can't be found.");
            return;
        }

        tabElement.RegisterCallback<MouseDownEvent>(evt => {
            _skill.ChangeMenuSelect(1);
            AudioManager.Instance.PlayUISound("click_01");
        }, TrickleDown.TrickleDown);
    }

    public void RegisterClickLearn(VisualElement tabElement)
    {
        if (tabElement == null)
        {
            Debug.LogError(tabElement + " can't be found.");
            return;
        }

        tabElement.RegisterCallback<MouseDownEvent>(evt => {
            _skill.ChangeMenuSelect(2);
            AudioManager.Instance.PlayUISound("click_01");
        }, TrickleDown.TrickleDown);
    }

    public void RegisterClickButtonPhysical(VisualElement rootEleent)
    {

        var btn = rootEleent.Q<Button>("DF_Button");
        btn.RegisterCallback<ClickEvent>((evt) =>
        {
            _skill.clickDfPhysical(btn);
        });
    }

    public void RegisterClickButtonMagic(VisualElement rootEleent)
    {

        var btn = rootEleent.Q<Button>("DF_Button_Magic");
        btn.RegisterCallback<ClickEvent>((evt) =>
        {
            _skill.clickDfMagic(btn);
        });
    }

    public void RegisterClickButtonEnhancing(VisualElement rootEleent)
    {

        var btn = rootEleent.Q<Button>("DF_Button_Enhancing");
        btn.RegisterCallback<ClickEvent>((evt) =>
        {
            _skill.clickDfEnhancing(btn);
        });
    }

    public void RegisterClickButtonDebilitating(VisualElement rootEleent)
    {

        var btn = rootEleent.Q<Button>("DF_Button_Debilitating");
        btn.RegisterCallback<ClickEvent>((evt) =>
        {
            _skill.clickDfDebilitating(btn);
        });
    }

    public void RegisterClickButtonClan(VisualElement rootEleent)
    {

        //var btn = rootEleent.Q<Button>("DF_Button_Clan");
       // btn.RegisterCallback<ClickEvent>((evt) =>
        //{
        //    _skill.clickDfClan(btn);
       // });
    }

    public void RegisterClickButtonAbility(VisualElement rootEleent)
    {

        var btn = rootEleent.Q<Button>("DF_Button_Ability");
        btn.RegisterCallback<ClickEvent>((evt) =>
        {
            _skill.clickDfAbility(btn);
        });
    }

    public void RegisterClickButtonSubject(VisualElement rootEleent)
    {

        var btn = rootEleent.Q<Button>("DF_Button_Subject");
        btn.RegisterCallback<ClickEvent>((evt) =>
        {
            _skill.clickDfSubject(btn);
        });
    }
}
