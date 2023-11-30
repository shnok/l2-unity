using UnityEngine;

[System.Serializable]
public abstract class Entity : MonoBehaviour {

    [SerializeField]
    private NetworkIdentity identity;

    public NetworkIdentity Identity { get => identity; set => identity = value; }

    /* Called when ApplyDamage packet is received */
    public abstract void ApplyDamage(byte attackId, int value);

    /* Notify server that entity got attacked */
    public abstract void InflictAttack(AttackType attackType);
}
