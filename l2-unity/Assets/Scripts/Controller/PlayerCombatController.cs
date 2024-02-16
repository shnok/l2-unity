using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    private static PlayerCombatController _instance;
    public static PlayerCombatController Instance { get { return _instance; } }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    void Update() {
        if(InputManager.Instance.IsInputPressed(InputType.DebugAttack)) {
            // for debug purpose
            if(TargetManager.Instance.HasTarget()) {
                Entity entity = TargetManager.Instance.GetTargetData().Data.ObjectTransform.GetComponent<Entity>();
                if (entity != null) {
                    entity.InflictAttack(AttackType.AutoAttack);
                }
            }
        }
    }

    public void VerifyAttackInput() {
        if (InputManager.Instance.IsInputPressed(InputType.Attack)) {
            if (TargetManager.Instance.HasTarget()) {
                //Should follow target
                //Should request for autoattack once reaching target
                Transform target = TargetManager.Instance.GetTargetData().Data.ObjectTransform;
                float attackRange = ((PlayerStatus)PlayerEntity.Instance.Status).AttackRange;
                float distance = Vector3.Distance(transform.position, target.position);
                Debug.Log($"target: {target} distance: {distance} range: {attackRange}");
                if (distance <= attackRange) {
                    ClientPacketHandler.Instance.SendRequestAutoAttack();
                } else {

                }
            }
        }
    }

}
