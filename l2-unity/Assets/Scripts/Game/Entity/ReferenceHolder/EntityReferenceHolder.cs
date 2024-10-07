using UnityEngine;

public class EntityReferenceHolder : MonoBehaviour
{
    [SerializeField] protected Entity _entity;
    [SerializeField] protected Gear _gear;
    [SerializeField] protected Combat _combat;
    [SerializeField] protected BaseAnimationController _animationController;
    [SerializeField] protected BaseAnimationAudioHandler _audioHandler;
    [SerializeField] protected Animator _animator;

    public BaseAnimationController AnimationController { get { return _animationController; } }
    public Gear Gear { get { return _gear; } }
    public Combat Combat { get { return _combat; } }
    public BaseAnimationAudioHandler AudioHandler { get { return _audioHandler; } }
    public Entity Entity { get { return _entity; } }
    public Animator Animator { get { return _animator; } }
}