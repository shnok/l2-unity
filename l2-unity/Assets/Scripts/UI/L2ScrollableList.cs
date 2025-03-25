using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

[System.Serializable]
public class L2ScrollableList<T> : L2Scrollable
{
    private VisualElement _content;
    private T[] _items;
    public T[] Items => _items;
    private ActionOut<int, VisualElement> _bindItem;
    public Action<VisualElement, int> RemoveItem { private get; set; }
    public delegate void ActionOut<in T1, T2>(T1 arg1, out T2 arg2);

    private bool _alternatingRowColor;
    private bool _isAlternatedRow;
    private int _currentSelectedItemId;

    public virtual void Initialize(VisualElement container, IEnumerable<T> items, ActionOut<int, VisualElement> bindItem, bool alternatingRowColor)
    {
        base.Initialize(container, true);
        _container = container;
        _content = _container.Q<VisualElement>("L2ListView");
        _currentSelectedItemId = -1;
        _items = items.ToArray();
        _bindItem = bindItem;
        _alternatingRowColor = alternatingRowColor;
        RefreshList();
    }

    public void RefreshList()
    {
        _content.Clear();
        for (int i = 0; i < _items.Length; i++)
        {
            AddToList(i);
        }
        SelectDefaultSlot();
    }
    
    public void AddToList(int index)
    {
        _bindItem(index, out VisualElement newListItem);
        newListItem.AddToClassList("l2-list-view-item");
        if (_alternatingRowColor)
        {
            if (_isAlternatedRow)
            {
                newListItem.AddToClassList("alternated");
            }
            _isAlternatedRow = !_isAlternatedRow;
        }

        _content.Add(newListItem);
    }

    public void RemoveFromList(int index)
    {
        RemoveItem(_content.Children().ElementAt(index), _currentSelectedItemId);
        _items[index] = _items[^1];
        Array.Resize(ref _items, _items.Length - 1);
        _content.RemoveAt(index);
    }

    private void SelectDefaultSlot()
    {
        if (_currentSelectedItemId != -1)
        {
            SelectItem(_currentSelectedItemId);
        }
    }
    
    public void SelectItem(int index)
    {
        if (_currentSelectedItemId != -1)
        {
            UnSelect(index);
        }
        SetSelected(index);
        _currentSelectedItemId = index;
    }

    public void SetSelected(int index)
    {
        VisualElement el = _content.ElementAt(index);
        el.AddToClassList("selected");
    }

    public void UnSelect(int index)
    {
        VisualElement el = _content.ElementAt(index);
        el.RemoveFromClassList("selected");
        _content.style.display = DisplayStyle.Flex;
    }

    public void ToggleShowHide()
    {
        _container.style.display = _container.style.display.value == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
    }
}