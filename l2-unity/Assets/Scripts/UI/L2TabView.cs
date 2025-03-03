using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class L2TabView
{
    private VisualElement _tabViewElement;
    private L2Tab[] _tabs;
    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private L2Tab _activeTab;
    public L2Tab ActiveTab { get => _activeTab; }

    public void Initialize(VisualElement tabViewElement, L2Tab[] tabs, VisualTreeAsset tabTemplate, VisualTreeAsset tabHeaderTemplate, bool reverseOrder)
    {
        _tabViewElement = tabViewElement;
        _tabs = tabs;
        _tabTemplate = tabTemplate;
        _tabHeaderTemplate = tabHeaderTemplate;

        CreateTabs(reverseOrder);
    }

    private void CreateTabs(bool reverseOrder)
    {
        VisualElement tabHeaderContainer = _tabViewElement.Q<VisualElement>("tab-header-container");
        if (tabHeaderContainer == null)
        {
            Debug.LogError("tab-header-container is null");
        }

        VisualElement tabContainer = _tabViewElement.Q<VisualElement>("tab-content-container");
        if (tabContainer == null)
        {
            Debug.LogError("tab-content-container");
        }

        for (int i = 0; i < _tabs.Length; i++)
        {
            L2Tab tab = reverseOrder ? _tabs[_tabs.Length - 1 - i] : _tabs[i];

            VisualElement tabElement = _tabTemplate.CloneTree()[0];
            // tabElement.name = _tabs[i].TabName;
            tabElement.name = tab.TabName;
            tabElement.AddToClassList("unselected-tab");

            VisualElement tabHeaderElement = _tabHeaderTemplate.CloneTree()[0];
            tabHeaderElement.name = tab.TabName;
            tabHeaderElement.Q<Label>().text = tab.TabName;

            tabHeaderContainer.Add(tabHeaderElement);
            tabContainer.Add(tabElement);

            tab.Initialize(this, tabElement, tabHeaderElement);
        }

        if (_tabs.Length > 0)
        {
            SwitchTab(_tabs[0]);
        }
    }

    public bool SwitchTab(L2Tab switchTo)
    {
        if (_activeTab != switchTo)
        {
            if (_activeTab != null)
            {
                _activeTab.TabContainer.AddToClassList("unselected-tab");
                _activeTab.TabHeader.RemoveFromClassList("active");
            }

            switchTo.TabContainer.RemoveFromClassList("unselected-tab");
            switchTo.TabHeader.AddToClassList("active");

            AudioManager.Instance.PlayUISound("window_open");

            _activeTab = switchTo;
            return true;
        }

        return false;
    }

    public void ShowTab(int tabIndex)
    {
        _tabs[tabIndex].TabHeader.RemoveFromClassList("hidden");
    }

    public void HideTab(int tabIndex)
    {
        _tabs[tabIndex].TabHeader.AddToClassList("hidden");
    }
}