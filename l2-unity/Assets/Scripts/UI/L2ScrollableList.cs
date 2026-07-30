using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    private int _currentSelectedItemId; // currently support only 1 selected

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
        Debug.Log("Adding skill to index: " + index);

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
        RemoveItem(_content.Children().ElementAt(index), index);
        _items[index] = _items[^1];
        Array.Resize(ref _items, _items.Length - 1);
        _content.RemoveAt(index);
    }

    public void RemoveSelectedFromList()
    {
        RemoveItem(_content.Children().ElementAt(_currentSelectedItemId), _currentSelectedItemId);
        _items[_currentSelectedItemId] = _items[^1];
        Array.Resize(ref _items, _items.Length - 1);
        _content.RemoveAt(_currentSelectedItemId);
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
            UnSelect();
        }
        SetSelected(index);
        _currentSelectedItemId = index;
    }

    private void SetSelected(int index)
    {
        VisualElement el = _content.ElementAt(index);
        el.AddToClassList("selected");
    }

    public void UnSelect()
    {
        VisualElement el = _content.ElementAt(_currentSelectedItemId);
        el.RemoveFromClassList("selected");
        _content.style.display = DisplayStyle.Flex;
    }

    public void ToggleShowHide()
    {
        _container.style.display = _container.style.display.value == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void Show()
    {
        _container.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        _container.style.display = DisplayStyle.None;
    }
}