using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour {
    private NetworkTransformShare _networkTransformShare;
    private NetworkCharacterControllerShare _networkCharacterControllerShare;
    [SerializeField] private bool _isAutoAttacking = false;


    public bool AutoAttacking { get { return _isAutoAttacking; } set { _isAutoAttacking = value; } }


    private static PlayerCombatController _instance;
    public static PlayerCombatController Instance { get { return _instance; } }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }

        TryGetComponent<NetworkTransformShare>(out _networkTransformShare);
        TryGetComponent<NetworkCharacterControllerShare>(out _networkCharacterControllerShare);
    }

    void Update() {
        if (PlayerController.Instance.CanMove && InputManager.Instance.IsInputPressed(InputType.Move)) {
            TargetManager.Instance.ClearAttackTarget();
        }

        if (InputManager.Instance.IsInputPressed(InputType.Escape)) {
            TargetManager.Instance.ClearTarget();
        }

        // for debug purpose
        if (InputManager.Instance.IsInputPressed(InputType.DebugAttack)) {      
            if(TargetManager.Instance.HasTarget()) {
                Entity entity = TargetManager.Instance.Target.Data.ObjectTransform.GetComponent<Entity>();
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
                Transform target = TargetManager.Instance.Target.Data.ObjectTransform;
                float attackRange = ((PlayerStatus)PlayerEntity.Instance.Status).AttackRange;
                float distance = Vector3.Distance(transform.position, target.position);
                Debug.Log($"target: {target} distance: {distance} range: {attackRange}");

                TargetManager.Instance.SetAttackTarget();

                if (distance <= attackRange) {
                    OnReachingTarget();
                } else {                  
                    ClickManager.Instance.HideLocator();
                    PathFinderController.Instance.MoveTo(target.position, ((PlayerStatus) PlayerEntity.Instance.Status).AttackRange);
                }
            }
        }
    }

    public void OnReachingTarget() {
        if(TargetManager.Instance.AttackTarget != null && !_isAutoAttacking) {
            Debug.LogWarning("On Reaching Target");
            PathFinderController.Instance.ClearPath();
            PlayerController.Instance.ResetDestination();
            PlayerController.Instance.SetCanMove(false);
            //TODO set can move to true if action failed packet returned

            if (_networkTransformShare != null) {
                _networkTransformShare.SharePosition();
            }

            if (_networkCharacterControllerShare != null) {
                _networkCharacterControllerShare.ShareMoveDirection(Vector3.zero);
            }

            ClientPacketHandler.Instance.SendRequestAutoAttack();
        }
    }
}
