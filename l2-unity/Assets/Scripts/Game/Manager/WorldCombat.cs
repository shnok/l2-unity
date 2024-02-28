using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldCombat : MonoBehaviour {
    [SerializeField] private GameObject _impactParticle;

    private static WorldCombat _instance;
    public static WorldCombat Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    void OnDestroy() {
        _instance = null;
    }


    public void InflictAttack(Transform target, int damage, int newHp, bool criticalHit) {
        ApplyDamage(target, damage, newHp, criticalHit);
    }

    public void InflictAttack(Transform attacker, Transform target, int damage, int newHp, bool criticalHit) {
        ApplyDamage(target, damage, newHp, criticalHit);

        // Instantiate hit particle
        ParticleImpact(attacker, target);
    }

    private void ApplyDamage(Transform target, int damage, int newHp, bool criticalHit) {
        Entity entity = target.GetComponent<Entity>();
        if (entity != null) {
            // Apply damage to target
            entity.ApplyDamage(damage, newHp, criticalHit);
        }
    }

    public void EntityStartAutoAttacking(Entity entity) {
        entity.StartAutoAttacking();
    }

    public void EntityStopAutoAttacking(Entity entity) {
        entity.StopAutoAttacking();
    }

    private void ParticleImpact(Transform attacker, Transform target) {
        // Calculate the position and rotation based on attacker
        var heading = attacker.position - target.position;
        float angle = Vector3.Angle(heading, target.forward);
        Vector3 cross = Vector3.Cross(heading, target.forward);
        if(cross.y >= 0) angle = -angle;
        Vector3 direction = Quaternion.Euler(0, angle, 0) * target.forward;

        float particleHeight = target.GetComponent<Entity>().Appearance.CollisionHeight * 1.25f;
        GameObject go = (GameObject)Instantiate(
            _impactParticle, 
            target.position + direction * 0.15f + Vector3.up * particleHeight, 
            Quaternion.identity);

        go.transform.LookAt(attacker);
        go.transform.eulerAngles = new Vector3(0, go.transform.eulerAngles.y + 180f, 0);
    }
}
