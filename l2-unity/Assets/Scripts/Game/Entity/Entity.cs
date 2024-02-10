using UnityEngine;

[System.Serializable]
public class Entity : MonoBehaviour {
    [SerializeField] private NetworkIdentity _identity;
    [SerializeField] private Status _status;
    [SerializeField] private int _target;

    public Status Status { get => _status; set => _status = value; }
    public NetworkIdentity Identity { get => _identity; set => _identity = value; }
    public int Target { get => _target; set => _target = value; }

    /* Called when ApplyDamage packet is received */
    public void ApplyDamage(byte attackId, int value) {

    }

    /* Notify server that entity got attacked */
    public void InflictAttack(AttackType attackType) {

    }
}
