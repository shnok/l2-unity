using UnityEngine;

[CreateAssetMenu(fileName = "Hunanoid", menuName = "Shnok/Animations")]
public class L2Animations : ScriptableObject
{
    [SerializeField]
    private AnimationClip[] animationClips;

    public AnimationClip[] AnimationClips { get => animationClips; }
}
