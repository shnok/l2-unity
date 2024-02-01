using UnityEngine;

[System.Serializable]
public class Entity : MonoBehaviour {

    [SerializeField]
    private NetworkIdentity _identity;
    public NetworkIdentity Identity { get => _identity; set => _identity = value; }


    /* Called when ApplyDamage packet is received */
    public void ApplyDamage(byte attackId, int value) {

    }

    /* Notify server that entity got attacked */
    public void InflictAttack(AttackType attackType) {

    }
}
