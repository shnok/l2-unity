#if FALSE
using UnityEngine;
using UnityEngine.UIElements;

public abstract class AbstractShortCutReplace 
{


    public void SetImageNext(VisualElement border, VisualElement row, int id_path, int activePanel)
    {
        // 0 root panel
        if (activePanel != 0)
        {
            //children minimal panels
            ShortCutChildrenModel[] childrenArrayPanels = ShortCutPanelMinimal.Instance.GetArrayRowsPanels();
            activePanel = activePanel - 1;
            if (activePanel <= childrenArrayPanels.Length - 1)
            {
                string path = childrenArrayPanels[activePanel].GetRowImgPath(id_path);
                Texture2D imgSource1 = Resources.Load<Texture2D>(path);
                if (imgSource1 != null)
                {
                    VisibleBorder(border, true);
                    row.style.backgroundImage = new StyleBackground(imgSource1);
                }
                else
                {
                    VisibleBorder(border, false);
                    row.style.backgroundImage = new StyleBackground();
                }
            }
        }

    }

    public void SetRows(ShortCutChildrenModel childrenModel, VisualElement border, VisualElement row, int id_path)
    {
        SetImage(childrenModel, border, row, id_path);
    }

    private void SetImage(ShortCutChildrenModel childrenModel, VisualElement border, VisualElement row, int id_path)
    {
        string path = childrenModel.GetRowImgPath(id_path);
        Texture2D imgSource1 = Resources.Load<Texture2D>(path);
        if (imgSource1 != null)
        {
            VisibleBorder(border, true);
            row.style.backgroundImage = new StyleBackground(imgSource1);
        }
    }

    public void VisibleBorder(VisualElement border, bool show)
    {
        if (border != null) border.visible = show;
    }

    public VisualElement GetBorderRow(VisualElement shortCutMinimal, int index)
    {
        return shortCutMinimal.Q(className: "border_row" + index);
    }
}
#endif
