using System.Collections.Generic;
using UnityEngine;

public class L2Emitter
{
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
    public List<SizeScale> sizeScales;
    public Range3D startVelocityRange;
    public Range lifetimeRange;
}