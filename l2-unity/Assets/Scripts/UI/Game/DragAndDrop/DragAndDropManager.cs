using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class DragAndDropManager 
{
    private string _activeCell;
    private bool _isDrag;
    bool isTest = false;
    private Dictionary<string, Background> allElements = new Dictionary<string, Background>();
    private static DragAndDropManager _instance;
    
    public static DragAndDropManager getInstance()
    {
        if (_instance == null)
            _instance = new DragAndDropManager();
        return _instance;
    }
    
    public bool IsDrag()
    {
        return _isDrag;
    }
    public void RegisterList(List<VisualElement> list)
    {
        list.ForEach(item =>
        {
            DragAndDropManipulator dad = new DragAndDropManipulator(item , this);
            RegisterOver(item);
            if (!isTest)
            {
                isTest = true;
                //test data
                allElements.Add(item.name, IconManager.Instance.LoadTextureByName("skill0914"));
            }
            else
            {
                //test data
                allElements.Add(item.name, IconManager.Instance.LoadTextureByName("skill0271"));
            }
           
        });
    }

    private void RegisterOver(VisualElement item)
    {
        item.RegisterCallback<MouseOverEvent>(evt =>
        {
            VisualElement ve = (VisualElement)evt.currentTarget;
            if (ve != null)
            {
                if (!string.IsNullOrEmpty(_activeCell))
                {
                    if (!_activeCell.Equals(ve.name) & _isDrag)
                    {
                        addImage(ve, _activeCell);
                        _isDrag = false;
                        _activeCell = "";
                    }
                }                
            }
        }, TrickleDown.TrickleDown);
    }

    private void addImage(VisualElement item , string activeName)
    {
        if (allElements.ContainsKey(activeName))
        {
            var s = allElements[activeName];
            item.style.backgroundImage = s;
        }
    }

    public void SetActive(string activeName)
    {
        this._activeCell = activeName;
        _isDrag = true;
    }

    public Background GetActiveBackground()
    {
        if(string.IsNullOrEmpty(_activeCell)) return IconManager.Instance.LoadTextureByName("");

        if (allElements.ContainsKey(_activeCell)){
            return allElements[_activeCell];
        }
        return IconManager.Instance.LoadTextureByName(_activeCell);
    }
}
