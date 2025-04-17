using UnityEngine;

[System.Serializable]
public class EffectEmitter
{
    [SerializeField] private AttachMethod _attachOn;
    [SerializeField] private bool _spawnOnTarget;
    [SerializeField] private bool _relativeToCylinder;
    [SerializeField] private string _effectClass;
    [SerializeField] private string _secondaryEffectClass;
    [SerializeField] private float _scaleSize;
    [SerializeField] private bool _onMultiTarget;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private EtcEffect _etcEffect;
    [SerializeField] private EtcEffectInfo _etcEffectInfo;
    [SerializeField] private bool _pawnLight;
    [SerializeField] private EffectPawnLightParam _pawnLightParam;
    [SerializeField] private bool _arrow;
    [SerializeField] private bool _chargedArrow;

    public AttachMethod AttachOn { get { return _attachOn; } set { _attachOn = value; } }
    public bool SpawnOnTarget { get { return _spawnOnTarget; } set { _spawnOnTarget = value; } }
    public bool RelativeToCylinder { get { return _relativeToCylinder; } set { _relativeToCylinder = value; } }
    public string EffectClass { get { return _effectClass; } set { _effectClass = value; } }
    public string SecondaryEffectClass { get { return _secondaryEffectClass; } set { _secondaryEffectClass = value; } }
    public float ScaleSize { get { return _scaleSize; } set { _scaleSize = value; } }
    public bool OnMultiTarget { get { return _onMultiTarget; } set { _onMultiTarget = value; } }
    public Vector3 Offset { get { return _offset; } set { _offset = value; } }
    public EtcEffect EtcEffect { get { return _etcEffect; } set { _etcEffect = value; } }
    public EtcEffectInfo EtcEffectInfo { get { return _etcEffectInfo; } set { _etcEffectInfo = value; } }
    public bool PawnLight { get { return _pawnLight; } set { _pawnLight = value; } }
    public EffectPawnLightParam PawnLightParam { get { return _pawnLightParam; } set { _pawnLightParam = value; } }
    public bool Arrow { get => _arrow; set => _arrow = value; }
    public bool ChargedArrow { get => _chargedArrow; set => _chargedArrow = value; }

}
