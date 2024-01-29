#if (UNITY_EDITOR) 
using UnityEngine;

[System.Serializable]
public class AmbientSound {
    public string name;
    public string ambientSoundName;
    public string ambientSoundType;
    public float ambientSoundStartTime;
    public int ambientRandom;
    public float soundRadius;
    public int soundVolume;
    public int soundPitch;
    public Vector3 position;
    public string groupName;

    public override string ToString() {
        return $"Name: {name}, ambientSoundName: {ambientSoundName}, " +
            $"ambientSoundType: {ambientSoundType}, " +
            $"ambientSoundStartTime: {ambientSoundStartTime}, " +
            $"ambientRandom: {ambientRandom}" +
            $"soundRadius: {soundRadius}" +
            $"soundVolume: {soundVolume}" +
            $"soundPitch: {soundPitch}" +
            $"ambientRandom: {ambientRandom}" +
            $"position: {position}" +
            $"groupName: {groupName}";
    }
}
#endif