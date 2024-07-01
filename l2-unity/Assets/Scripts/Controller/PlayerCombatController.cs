using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour {
    private NetworkTransformShare _networkTransformShare;
    private NetworkCharacterControllerShare _networkCharacterControllerShare;
    [SerializeField] private bool _isAutoAttacking = false;
    [SerializeField] private bool _isRunningToTarget = false;


    public bool AutoAttacking { get { return _isAutoAttacking; } set { _isAutoAttacking = value; } }
    public bool RunningToTarget { get { return _isRunningToTarget; } set { _isRunningToTarget = value; } }


    private static PlayerCombatController _instance;
    public static PlayerCombatController Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }

        TryGetComponent<NetworkTransformShare>(out _networkTransformShare);
        TryGetComponent<NetworkCharacterControllerShare>(out _networkCharacterControllerShare);
    }

    void OnDestroy() {
        _instance = null;
    }

    void Update() {
        if (PlayerController.Instance.CanMove && InputManager.Instance.IsInputPressed(InputType.Move)) {
            _isRunningToTarget = false;
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

    public bool VerifyAttackInput() {
        if (InputManager.Instance.IsInputPressed(InputType.Attack)) {
            if (TargetManager.Instance.HasTarget()) {
                //Should follow target
                //Should request for autoattack once reaching target
                Transform target = TargetManager.Instance.Target.Data.ObjectTransform;
                float attackRange = ((PlayerStats) PlayerEntity.Instance.Stats).AttackRange;
                float distance = Vector3.Distance(transform.position, target.position);
                Debug.Log($"target: {target} distance: {distance} range: {attackRange}");

                if (distance <= attackRange) {
                    OnReachingTarget();
                } else {                  
                    ClickManager.Instance.HideLocator();
                    _isRunningToTarget = true;
                    PathFinderController.Instance.MoveTo(target.position, ((PlayerStats) PlayerEntity.Instance.Stats).AttackRange);
                }

                return true;
            }
        }

        return false;
    }

    // Send a request auto attack when close enough
    public void OnReachingTarget() {
        if(TargetManager.Instance.Target != null && (!_isAutoAttacking || TargetManager.Instance.AttackTarget != TargetManager.Instance.Target)) {
            Debug.LogWarning("On Reaching Target");
            TargetManager.Instance.SetAttackTarget();
            PathFinderController.Instance.ClearPath();
            PlayerController.Instance.ResetDestination();
            PlayerController.Instance.SetCanMove(false);

            //TODO set can move to true if action failed packet returned

            if (_networkTransformShare != null) {
                _networkTransformShare.SharePosition();
            }

            if (_networkCharacterControllerShare != null) {
               // _networkCharacterControllerShare.ShareMoveDirection(Vector3.zero);
            }

            GameClient.Instance.ClientPacketHandler.SendRequestAutoAttack();
        }
    }

    // Autoattack failed (entity probably too far)
    public void OnAutoAttackFailed() {
        Debug.LogWarning("AutoAttack failed");
        PlayerController.Instance.SetCanMove(true);
    }
}
