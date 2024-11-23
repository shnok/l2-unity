using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATGStandaloneGrassFadeParams : MonoBehaviour
{
    
    public float _cullDistance = 100.0f;
    public float _cullFadeLength = 20.0f;

    [Space(2)]
    public float _detailFadeStart = 60.0f;
    public float _detailFadeLength = 20.0f;

    [Space(2)]
    public float _vertexFadeStart = 10.0f;
    public float _vertexFadeLength = 10.0f;

    [Space(2)]
    public float _shadowGrassFadeStart = 50.0f;
    public float _shadowGrassFadeLength = 20.0f;

    [Space(2)]
    public float _shadowFoliageFadeStart = 70.0f;
    public float _shadowFoliageFadeLength = 20.0f;

    private static readonly int GrassFadePropsPID = Shader.PropertyToID("_AtgGrassFadeProps");
    private static readonly int GrassShadowFadePropsPID = Shader.PropertyToID("_AtgGrassShadowFadeProps");
    private static readonly int VertexFadePropsPID = Shader.PropertyToID("_AtgVertexFadeProps");


    public void SetFadeDistances()
    {

        var GrassFadeProperties = new Vector4(
            _cullDistance * _cullDistance,
            1.0f / (_cullFadeLength * _cullFadeLength * ((_cullDistance / _cullDistance) * 2.0f)),
            _detailFadeStart * _detailFadeStart,
            1.0f / (_detailFadeLength * _detailFadeLength)
        );
        Shader.SetGlobalVector(GrassFadePropsPID, GrassFadeProperties);

        var GrassShadowFadeProps = new Vector4(
            _shadowGrassFadeStart * _shadowGrassFadeStart,
            1.0f / (_shadowGrassFadeLength * _shadowGrassFadeLength),
            _shadowFoliageFadeStart * _shadowFoliageFadeStart,
            1.0f / (_shadowFoliageFadeLength * _shadowFoliageFadeLength)
        );
        Shader.SetGlobalVector(GrassShadowFadePropsPID, GrassShadowFadeProps);

        var VertexFadeProperties = new Vector2(
            _vertexFadeStart * _vertexFadeStart,
            1.0f / (_vertexFadeLength * _vertexFadeLength)
        );
        Shader.SetGlobalVector(VertexFadePropsPID, VertexFadeProperties);
    }

    void OnEnable()
    {
        SetFadeDistances();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        SetFadeDistances();
    }
#endif

}
