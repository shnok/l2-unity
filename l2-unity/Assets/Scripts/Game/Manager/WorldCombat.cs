using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldCombat : MonoBehaviour {
    [SerializeField] private GameObject _impactParticle;

    private static WorldCombat _instance;
    public static WorldCombat Instance { get { return _instance; } }

    void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    // Inflict attack to the target
    public void Attack(Transform target, AttackType attackType) {
        target.GetComponent<Entity>().InflictAttack(attackType);
    }

    // Apply the damage to target and spawn extra effects
    public void InflictAttack(Transform attacker, Transform target, AttackType attackId, int value, bool criticalHit) {

        ApplyDamage(target, attackId, value, criticalHit);

        // Instantiate hit particle
        ParticleImpact(attacker, target);

        // Instantiate damage text
    }

    private void ApplyDamage(Transform target, AttackType attackId, int damage, bool criticalHit) {
        Entity entity = target.GetComponent<Entity>();
        if (entity != null) {
            // Apply damage to target
            entity.ApplyDamage(attackId, damage, criticalHit);
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

        float particleHeight = target.GetComponent<Entity>().Identity.CollisionHeight * 1.25f;
        GameObject go = (GameObject)Instantiate(
            _impactParticle, 
            target.position + direction * 0.3f + Vector3.up * particleHeight, 
            Quaternion.identity);

        go.transform.LookAt(attacker);
        go.transform.eulerAngles = new Vector3(0, go.transform.eulerAngles.y + 180f, 0);
    }
}
