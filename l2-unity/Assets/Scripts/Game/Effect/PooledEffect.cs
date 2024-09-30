using UnityEngine;

public class PooledEffect
{
    public string EffectClass { get; set; }
    public float StartTime { get; set; }
    public float EffectDurationSec { get; set; }
    public float MaximumInactiveTimeSec { get; set; }
    public GameObject GameObject { get; set; }
}