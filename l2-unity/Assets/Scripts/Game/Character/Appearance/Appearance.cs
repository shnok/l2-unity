using UnityEngine;

[System.Serializable]
public class Appearance
{
    [SerializeField] private float _collisonHeight;
    [SerializeField] private float _collisionRadius;
    [SerializeField] private int _lhand;
    [SerializeField] private int _rhand;
    [SerializeField] private int _serverNameColor;
    [SerializeField] private int _serverTitleColor;
    // [SerializeField] private Color _defaultTitleColor; //= "9CE8A9FF"; // default color
    public float CollisionHeight { get { return _collisonHeight; } set { _collisonHeight = value; } }
    public float CollisionRadius { get { return _collisionRadius; } set { _collisionRadius = value; } }
    public int ServerTitleColor { get => _serverTitleColor; set => _serverTitleColor = value; }
    public int ServerNameColor { get => _serverNameColor; set => _serverNameColor = value; }
    public int LHand { get { return _lhand; } set { _lhand = value; } }
    public int RHand { get { return _rhand; } set { _rhand = value; } }

    public virtual void UpdateAppearance(Appearance appearance)
    {
        _serverTitleColor = appearance.ServerTitleColor;
        _serverNameColor = appearance.ServerNameColor;
        _collisonHeight = appearance.CollisionHeight;
        _collisionRadius = appearance.CollisionRadius;
        _lhand = appearance.LHand;
        _rhand = appearance.RHand;
    }

    public bool ShouldUpdateWeapons(Appearance newAppearance)
    {
        return LHand != newAppearance.LHand || RHand != newAppearance.RHand;
    }

    public bool ShouldUpdateColSize(Appearance newAppearance)
    {
        return CollisionHeight == 0 || CollisionRadius == 0;
    }
}
