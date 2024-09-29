#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

public class L2Emitter
{
    public string effectName;
    public string objectName;
    public string name;
    public bool useColorScale;
    public List<ColorScale> colorScales;
    public Range3D colorMultiplierRange;
    public float opacity;
    public bool fadeOut;
    public float fadeOutStartTime;
    public bool fadeIn;
    public float fadeInEndTime;
    public int maxParticles;
    public Vector3 startLocationOffset;
    public Range3D startLocationRange;
    public bool spinParticles;
    public Range3D spinsPerSecondRange;
    public Range3D startSpinRange;
    public Range3D startSizeRange;
    public string texture;
    public int TextureUSubdivisions;
    public int TextureVSubdivisions;
    public bool UseRandomSubdivision;
    public int SubdivisionStart;
    public int SubdivisionEnd;
    public string staticMesh;
    public bool useSizeScale;
    public bool uniformSize;
    public List<SizeScale> sizeScales;
    public Range3D startVelocityRange;
    public Range lifetimeRange;
    public string drawStyle;
    public string getVelocityDirectionFrom;
    public Range3D velocityLossRange;
    public string StartLocationShape;
    public Range sphereRadiusRange;
    public Range3D startLocationPolarRange;
    public string useDirectionAs;
    public float sizeScaleRepeats;
    public Vector3 projectionNormal;
    public float drawScale;
    public Vector3 acceleration;
    public Range initialDelayRange;
}
#endif