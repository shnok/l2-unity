using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Shnok/Animations/Monster")]
public class L2MonsterAnimationContainer : ScriptableObject
{
    [SerializeField]
    private L2MonsterAnimation[] _animations;

    public L2MonsterAnimation[] Animations { get => _animations; }
}