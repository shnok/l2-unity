using TMPro;
using UnityEngine;

public class Nameplate3D : MonoBehaviour
{
    [SerializeField] protected bool _visible;
    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private RectTransform _bar;
    [SerializeField] private RectTransform _barTrail;
    [Header("Sizing")]
    [SerializeField] private float _initalBarWidth;
    [SerializeField] private float _currentBarWidth;
    [SerializeField] private float _barTrailWidth;
    [Header("Owner")]
    [SerializeField] protected Entity _target;
    [SerializeField] protected NetworkIdentity _identity;

    public bool Visible { get => _visible; set => _visible = value; }
    public Entity Target { get => _target; }

    void Awake()
    {
        if (_name == null || _title == null)
        {
            Debug.LogWarning("Nameplates TextMesh is not set.");
            gameObject.SetActive(false);
        }

        _currentBarWidth = _initalBarWidth;
        _barTrailWidth = _initalBarWidth;
    }

    public virtual void SetTarget(Entity target)
    {
        _target = target;
        UpdateIdentity(target.Identity);
    }

    private void UpdateIdentity(NetworkIdentity identity)
    {
        _identity = identity;

        if (_target == PlayerEntity.Instance)
        {
            _name.text = "";
            _title.text = "";
            return;
        }

        _name.text = identity.Name;
        _title.text = identity.Title;
    }

    public void UpdateStatus(Status status, Stats stats)
    {
        float hpPercent = 0f;
        if (stats.MaxHp > 0)
            hpPercent = (float)status.Hp / (float)stats.MaxHp;

        hpPercent = Mathf.Clamp01(hpPercent); // 0..1

        float tmp = _initalBarWidth * hpPercent;
        if (tmp > _currentBarWidth)
            _barTrailWidth = tmp;
        _currentBarWidth = tmp;

        _bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _currentBarWidth);
    }

    void Update()
    {
        if (_currentBarWidth < _barTrailWidth)
        {
            _barTrailWidth -= Time.deltaTime * 1.5f;
            _barTrail.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _barTrailWidth);
        }
    }

    protected virtual void UpdateTitleColor(Color color)
    {
        _title.color = color;
    }

    protected virtual void UpdateNameColor(Color color)
    {
        _name.color = color;
    }


    public virtual void ManageColors() { }

    public virtual void Show() { }

    public virtual void Hide() { }
}