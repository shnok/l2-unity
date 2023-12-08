using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldCombat : MonoBehaviour {
    public GameObject impactParticle;

    private static WorldCombat _instance;

    void Awake() {
        if(_instance == null)
            _instance = this;
    }

    public static WorldCombat GetInstance() {
        return _instance;
    }

    public void Attack(Transform target, AttackType attackType) {
        target.GetComponent<Entity>().InflictAttack(attackType);
    }

    public void ApplyDamage(Transform attacker, Transform target, byte attackId, int value) {
        // Apply damage to target
        target.GetComponent<Entity>().ApplyDamage(attackId, value);

        // Instantiate hit particle
        ParticleImpact(attacker, target);

        // Instantiate damage text
    }


    private void ParticleImpact(Transform attacker, Transform target) {
        var heading = attacker.position - target.position;
        float angle = Vector3.Angle(heading, target.forward);
        Vector3 cross = Vector3.Cross(heading, target.forward);
        if(cross.y >= 0) angle = -angle;
        Vector3 direction = Quaternion.Euler(0, angle, 0) * target.forward;
        GameObject go = (GameObject)Instantiate(impactParticle, target.position + direction * 0.3f + Vector3.up * 0.4f, Quaternion.identity);
        go.transform.LookAt(attacker);
        go.transform.eulerAngles = new Vector3(0, go.transform.eulerAngles.y, 0);
    }
}
