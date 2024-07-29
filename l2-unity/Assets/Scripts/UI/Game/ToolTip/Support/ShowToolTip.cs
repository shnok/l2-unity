using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShowToolTip
{
    IToolTips _toolTip;
    public ShowToolTip(IToolTips toolTipManager)
    {
        this._toolTip = toolTipManager;
    }
    public void Show(VisualElement ve)
    {
        var vector2 = new Vector2(ve.worldBound.position.x, ve.worldBound.position.y);
        float original =  vector2.y;
        float dfig = original + ve.worldBound.width;
        var vector3 = new Vector2(vector2.x, dfig);
        _toolTip.NewPosition(vector3 , ve.worldBound.width);
    }

    public void Hide(VisualElement ve)
    {
        _toolTip.ResetPosition(new Vector2(0,0));
    }

}
