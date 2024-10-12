using UnityEngine;

[System.Serializable]
public class EffectEmitter
{
    [SerializeField] private AttachMethod _attachOn;
    [SerializeField] private bool _spawnOnTarget;
    [SerializeField] private bool _relativeToCylinder;
    [SerializeField] private string _effectClass;
    [SerializeField] private float _scaleSize;
    [SerializeField] private bool _onMultiTarget;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private EtcEffect _etcEffect;
    [SerializeField] private EtcEffectInfo _etcEffectInfo;
    [SerializeField] private bool _pawnLight;
    [SerializeField] private EffectPawnLightParam _pawnLightParam;

    public AttachMethod AttachOn { get { return _attachOn; } set { _attachOn = value; } }
    public bool SpawnOnTarget { get { return _spawnOnTarget; } set { _spawnOnTarget = value; } }
    public bool RelativeToCylinder { get { return _relativeToCylinder; } set { _relativeToCylinder = value; } }
    public string EffectClass { get { return _effectClass; } set { _effectClass = value; } }
    public float ScaleSize { get { return _scaleSize; } set { _scaleSize = value; } }
    public bool OnMultiTarget { get { return _onMultiTarget; } set { _onMultiTarget = value; } }
    public Vector3 Offset { get { return _offset; } set { _offset = value; } }
    public EtcEffect EtcEffect { get { return _etcEffect; } set { _etcEffect = value; } }
    public EtcEffectInfo EtcEffectInfo { get { return _etcEffectInfo; } set { _etcEffectInfo = value; } }
    public bool PawnLight { get { return _pawnLight; } set { _pawnLight = value; } }
    public EffectPawnLightParam PawnLightParam { get { return _pawnLightParam; } set { _pawnLightParam = value; } }

}
