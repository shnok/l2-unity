using UnityEngine;

[CreateAssetMenu(fileName = "Hunanoid", menuName = "Shnok/Animations/Humanoid")]
public class L2HumanoidAnimationContainer : ScriptableObject
{
    [SerializeField]
    private L2HumanoidAnimation[] _animations;

    public L2HumanoidAnimation[] Animations { get => _animations; }
}
