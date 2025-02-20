using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class L2Tab : L2Scrollable
{
    [SerializeField] string _tabName = "Tab";
    [SerializeField] protected bool _autoscroll = true;
    private VisualElement _tabContainer;
    private VisualElement _tabHeader;
    protected VisualElement _windowEle;
    public string TabName { get { return _tabName; } }
    public VisualElement TabContainer { get { return _tabContainer; } }
    public VisualElement TabHeader { get { return _tabHeader; } }

    public virtual void Initialize(VisualElement windowEle, VisualElement tabContainer, VisualElement tabHeader, bool hasScrollView)
    {
        if (hasScrollView)
        {
            base.Initialize(tabContainer, true);
        }

        _windowEle = windowEle;
        _tabContainer = tabContainer;
        _tabHeader = tabHeader;

        InitTabHeader();
    }

    public virtual void Initialize(VisualElement windowEle, VisualElement tabContainer, VisualElement tabHeader)
    {
        Initialize(windowEle, tabContainer, tabHeader, false);
    }

    private void InitTabHeader()
    {
        _tabHeader?.AddManipulator(new ButtonClickSoundManipulator(_tabHeader));
        _tabHeader?.RegisterCallback<MouseDownEvent>(evt =>
        {
            OnSwitchTab();
        }, TrickleDown.TrickleDown);
    }


    protected virtual void OnSwitchTab() { }

    public virtual void SelectSlot(int slot) { }
}