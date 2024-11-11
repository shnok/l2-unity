using UnityEngine;
using UnityEngine.UIElements;

public class Nameplate
{
    protected VisualElement _nameplateEle;
    private VisualElement _leftBubbleEle;
    private VisualElement _rightBubbleEle;
    private Label _nameplateEntityName;
    private Label _nameplateEntityTitle;

    [SerializeField] private float _nameplateOffsetHeight;
    [SerializeField] private Transform _target;
    [SerializeField] private bool _visible;
    [SerializeField] private Entity _entity;

    private int _previousServerTitleColor = -1;
    private int _previousServerNameColor = -1;
    private int _previousKarmaAmount = 0;
    private int _flagTimestamp = 0;
    private bool _isStyleVisible;

    public VisualElement NameplateEle { get { return _nameplateEle; } set { _nameplateEle = value; } }
    public bool Visible { get { return _visible; } set { _visible = value; } }
    public Transform Target { get { return _target; } }
    public float NameplateOffsetHeight { get { return _nameplateOffsetHeight; } set { _nameplateOffsetHeight = value; } }
    public Entity Entity { get { return _entity; } }

    // Default
    public Nameplate(VisualElement visualElement, Label entityName, Label entityTitle, Entity entity)
    {
        _nameplateEle = visualElement;
        _nameplateEntityName = entityName;
        _nameplateEntityTitle = entityTitle;
        _target = entity.transform;
        _entity = entity;
        _nameplateEntityName.text = entity.Identity.Name;
        _nameplateEntityTitle.text = entity.Identity.Title;
        _nameplateEntityTitle.style.color = entity.Identity.TitleColor;
        _visible = true;
    }

    public void ManageColors()
    {
        if (_previousServerNameColor != _entity.Identity.ServerNameColor)
        {

            Debug.LogWarning($"Name color changed: Old:{_previousServerNameColor} New:{_entity.Identity.ServerNameColor}");
            _previousServerNameColor = _entity.Identity.ServerNameColor;

            if (_previousServerNameColor == 0)
            {
                // keep default value
                // _nameplateEntityName.style.color = _entity.Identity.TitleColor;
            }
            else
            {
                _nameplateEntityName.style.color = ColorUtils.IntegerToColor(_previousServerNameColor);
            }
        }

        if (_previousServerTitleColor != _entity.Identity.ServerTitleColor)
        {
            Debug.LogWarning($"Title color changed: Old:{_previousServerTitleColor} New:{_entity.Identity.ServerTitleColor}");
            _previousServerTitleColor = _entity.Identity.ServerTitleColor;

            if (_previousServerTitleColor == 0)
            {
                if (_entity.Identity.EntityType == EntityType.Monster || _entity.Identity.EntityType == EntityType.NPC)
                {
                    _nameplateEntityTitle.style.color = _entity.Identity.TitleColor;
                }
            }
            else
            {
                _nameplateEntityTitle.style.color = ColorUtils.IntegerToColor(_previousServerTitleColor);
            }
        }
    }

    public void SetStyle(string className)
    {
        if (_leftBubbleEle != null)
        {
            SetClassName(_leftBubbleEle, className);
        }
        else
        {
            _leftBubbleEle = _nameplateEle.Q<VisualElement>("TargetBubbleLeft");
        }
        if (_rightBubbleEle != null)
        {
            SetClassName(_rightBubbleEle, className);
        }
        else
        {
            _rightBubbleEle = _nameplateEle.Q<VisualElement>("TargetBubbleRight");
        }
    }

    public void RemoveStyle(string className)
    {
        if (_leftBubbleEle != null)
        {
            RemoveClassName(_leftBubbleEle, className);
        }
        else
        {
            _leftBubbleEle = _nameplateEle.Q<VisualElement>("TargetBubbleLeft");
        }
        if (_rightBubbleEle != null)
        {
            RemoveClassName(_rightBubbleEle, className);
        }
        else
        {
            _rightBubbleEle = _nameplateEle.Q<VisualElement>("TargetBubbleRight");
        }
    }

    protected void SetClassName(VisualElement element, string className)
    {
        if (!element.ClassListContains(className))
        {
            element.AddToClassList(className);
        }
    }

    protected void RemoveClassName(VisualElement element, string className)
    {
        if (element.ClassListContains(className))
        {
            element.RemoveFromClassList(className);
        }
    }

    public void Show()
    {
        if (!_isStyleVisible)
        {
            RemoveClassName(_nameplateEle, "hidden");
            _isStyleVisible = true;
        }
    }

    public void Hide()
    {
        if (_isStyleVisible)
        {
            SetClassName(_nameplateEle, "hidden");
            _isStyleVisible = false;
        }
    }
}
