using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class L2Tab
{
    [SerializeField] string _tabName = "Tab";
    [SerializeField] protected bool _autoscroll = true;
    protected VisualElement _tabContainer;
    private VisualElement _tabHeader;
    protected VisualElement _windowEle;
    public string TabName { get { return _tabName; } }
    public VisualElement TabContainer { get { return _tabContainer; } }
    public VisualElement TabHeader { get { return _tabHeader; } }
    private L2TabView _tabView;

    public virtual void Initialize(L2TabView tabView, VisualElement tabContainer, VisualElement tabHeader)
    {
        _tabContainer = tabContainer;
        _tabHeader = tabHeader;
        _tabView = tabView;

        InitTabHeader();
    }

    private void InitTabHeader()
    {
        _tabHeader?.AddManipulator(new ButtonClickSoundManipulator(_tabHeader));
        _tabHeader?.RegisterCallback<MouseDownEvent>(evt =>
        {
            OnTabHeaderClicked();
        }, TrickleDown.TrickleDown);
    }

    protected virtual void OnTabHeaderClicked()
    {
        _tabView.SwitchTab(this);
    }

    public virtual void SelectSlot(int slot) { }
}