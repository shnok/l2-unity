using UnityEngine;

[System.Serializable]
public class Entity : MonoBehaviour {
    [SerializeField] private NetworkIdentity _identity;
    [SerializeField] private Status _status;
    [SerializeField] private int _target;

    protected NetworkAnimationReceive _networkAnimationReceive;
    private NetworkTransformReceive _networkTransformReceive;
    private NetworkCharacterControllerReceive _networkCharacterControllerReceive;

    public Status Status { get => _status; set => _status = value; }
    public NetworkIdentity Identity { get => _identity; set => _identity = value; }
    public int Target { get => _target; set => _target = value; }

    public void Awake() {
        Initialize();
    }

    protected virtual void Initialize() {
        TryGetComponent(out _networkAnimationReceive);
        TryGetComponent(out _networkTransformReceive);
        TryGetComponent(out _networkCharacterControllerReceive);
    }

    /* Called when ApplyDamage packet is received */
    public void ApplyDamage(AttackType attackType, int value, bool criticalHit) {
        if(_status.Hp <= 0) {
            Debug.LogWarning("Trying to apply damage to a dead entity");
            return;
        }

        _status.Hp = Mathf.Max(Status.Hp - value, 0);

        OnHit(criticalHit);

        if(_status.Hp == 0) {
            OnDeath();
        }
    }

    /* Notify server that entity got attacked */
    public void InflictAttack(AttackType attackType) {
        ClientPacketHandler.Instance.InflictAttack(_identity.Id, attackType);
    }

    protected virtual void OnDeath() {
        if(_networkAnimationReceive != null) {
            _networkAnimationReceive.enabled = false;
        }
        if (_networkTransformReceive != null) {
            _networkTransformReceive.enabled = false;
        }
        if (_networkCharacterControllerReceive != null) {
            _networkCharacterControllerReceive.enabled = false;
        }
    }

    protected virtual void OnHit(bool criticalHit) {
        // TODO: Add armor type for more hit sounds
        AudioManager.Instance.PlayHitSound(criticalHit, transform.position);
    }

    public virtual void OnStopMoving() {}

    public virtual void OnStartMoving(bool walking) {}
}
