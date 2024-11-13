using UnityEngine;
using UnityEngine.UIElements;

public class Nameplate
{
    public static Color FINAL_KARMA_COLOR = new Color(1f, 0, 0, 1f);
    public static Color DEFAULT_NAME_COLOR = new Color(1f, 1f, 1f, 1f);
    public static Color FLAG_COLOR = new Color(1f, 0f, 1f, 1f);

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
    private Color _previousServerNameColorValue;
    private int _lastFlag = 0;
    private int _previousKarmaAmount = 0;
    private bool _isStyleVisible;
    private bool _blink;
    private float _lastBlinkTime;

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
        _visible = true;
    }

    public void ManageColors()
    {
        if (_previousServerTitleColor != _entity.Appearance.ServerTitleColor)
        {
            // Debug.LogWarning($"Title color changed: Old:{_previousServerTitleColor} New:{_entity.Appearance.ServerTitleColor}");
            _previousServerTitleColor = _entity.Appearance.ServerTitleColor;

            if (_previousServerTitleColor != 0)
            {
                _nameplateEntityTitle.style.color = ColorUtils.IntegerToColor(_previousServerTitleColor);
            }
        }

        //PVP FLAG: 1 - Purple
        //PVP FLAG: 2 - Blinking
        //Karma 0-300 slowly going from white to deep red 300 being the maximum lerp value

        if (_entity.Stats.Karma > 0)
        {
            if (_entity.Stats.Karma != _previousKarmaAmount)
            {
                _previousKarmaAmount = _entity.Stats.Karma;

                float lerpRatio = Mathf.Clamp(_entity.Stats.Karma / 300f, 0.25f, 1f);
                _nameplateEntityName.style.color = Color.Lerp(DEFAULT_NAME_COLOR, FINAL_KARMA_COLOR, lerpRatio);
            }
        }
        else if (_entity.Identity.PvpFlag == 1)
        {
            _lastFlag = _entity.Identity.PvpFlag;
            _nameplateEntityName.style.color = FLAG_COLOR;
        }
        else if (_entity.Identity.PvpFlag == 2)
        {
            _lastFlag = _entity.Identity.PvpFlag;
            if (Time.time - _lastBlinkTime > 0.5f)
            {
                _blink = !_blink;

                _nameplateEntityName.style.color = _blink ? FLAG_COLOR : _previousServerNameColorValue;
                _lastBlinkTime = Time.time;
            }
        }
        else
        {
            if (_previousServerNameColor != _entity.Appearance.ServerNameColor || _entity.Stats.Karma != _previousKarmaAmount || _entity.Identity.PvpFlag != _lastFlag)
            {
                _lastFlag = 0;
                _previousKarmaAmount = 0;

                // Debug.LogWarning($"Name color changed: Old:{_previousServerNameColor} New:{_entity.Appearance.ServerNameColor}");
                _previousServerNameColor = _entity.Appearance.ServerNameColor;

                if (_previousServerNameColor != 0)
                {
                    _previousServerNameColorValue = ColorUtils.IntegerToColor(_previousServerNameColor);
                    _nameplateEntityName.style.color = _previousServerNameColorValue;
                }
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
