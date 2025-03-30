using Animancer;
using UnityEngine;

public class EntityReferenceHolder : MonoBehaviour
{
    [SerializeField] protected Entity _entity;
    [SerializeField] protected Gear _gear;
    [SerializeField] protected Combat _combat;
    [SerializeField] protected NewBaseAnimationController _newAnimationController;
    [SerializeField] protected AnimancerComponent _animancer;
    [SerializeField] protected BaseAnimationAudioHandler _audioHandler;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected Transform _clickArea;

    public NewBaseAnimationController NewAnimationController { get { return _newAnimationController; } }
    public AnimancerComponent Animancer { get { return _animancer; } }
    public Gear Gear { get { return _gear; } }
    public Combat Combat { get { return _combat; } }
    public BaseAnimationAudioHandler AudioHandler { get { return _audioHandler; } }
    public Entity Entity { get { return _entity; } }
    public Animator Animator { get { return _animator; } }
    public Transform ClickArea { get { return _clickArea; } }
}