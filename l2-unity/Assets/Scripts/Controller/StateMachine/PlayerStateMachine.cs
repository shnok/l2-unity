using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour {
    private static PlayerStateMachine _instance;
    public static PlayerStateMachine Instance { get { return _instance; } }

    private NetworkTransformShare _networkTransformShare;
    private NetworkCharacterControllerShare _networkCharacterControllerShare;

    [SerializeField] private Intention _intention;
    [SerializeField] private PlayerState _state;
    [SerializeField] private Event _lastEvent;
    [SerializeField] private bool _waitingForServerReply;
    private bool _thinking;

    public PlayerState State { get { return _state; } }


    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }

        _waitingForServerReply = false;
        TryGetComponent<NetworkTransformShare>(out _networkTransformShare);
        TryGetComponent<NetworkCharacterControllerShare>(out _networkCharacterControllerShare);
    }

    void OnDestroy() {
        _instance = null;
    }


    void Update() {
        notifyEvent(Event.THINK);
    }

    public void SetState(PlayerState state) {
      //  Debug.Log($"[StateMachine] New state: {state}");
        _state = state;
    }

    public void SetWaitingForServerReply(bool value) {
       // Debug.LogWarning($"[StateMachine] Waiting for server reply: {value}");
        _waitingForServerReply = value;
    }

    /*
    =========================
    ========= EVENT =========
    =========================
     */
    public void notifyEvent(Event evt) {
        if (evt != Event.THINK) {
            _lastEvent = evt;
        //    Debug.Log($"[StateMachine] New event: {evt}");
        }

        notifyEvent(evt, null);
    }

    public void notifyEvent(Event evt, Entity go) {
        switch (evt) {
            case Event.THINK:
                onEvtThink();
                break;
            case Event.DEAD:
                onEvtDead();
                break;
            case Event.ARRIVED:
                onEvtArrived();
                break;
            case Event.ATTACKED:
                onEvtAttacked(go);
                break;
            case Event.READY_TO_ACT:
                onEvtReadyToAct();
                break;
            case Event.CANCEL:
                onEvtCancel();
                break;
        }
    }

    private void onEvtThink() {
        if (_thinking) {
            return;
        }

        _thinking = true;

        if (_intention == Intention.INTENTION_IDLE) {
            thinkIdle();
        }

        if (_intention == Intention.INTENTION_MOVE_TO) {
            thinkMoveTo();
        }

        if (_intention == Intention.INTENTION_ATTACK) {
            thinkAttack();
        }

        _thinking = false;
    }

    private void onEvtDead() {
        SetState(PlayerState.DEAD);
    }

    private void onEvtArrived() {
        if(_intention != Intention.INTENTION_ATTACK) {
            if(TargetManager.Instance.HasAttackTarget()) {
                // arrived to target position
                setIntention(Intention.INTENTION_ATTACK);
            } else {
                // regular player movement
                setIntention(Intention.INTENTION_IDLE);
            }
        }
    }

    private void onEvtAttacked(Entity attacker) {

    }

    private void onEvtReadyToAct() {
        if (_intention == Intention.INTENTION_ATTACK) {
            if (_waitingForServerReply) {
                SetWaitingForServerReply(false);
                SetState(PlayerState.ATTACKING);
            } else {
                if (TargetManager.Instance.Target == null) {
                    notifyEvent(Event.CANCEL);
                    return;
                }

                OnReachingTarget();

                SetWaitingForServerReply(true);
            }
        } else if (_intention == Intention.INTENTION_MOVE_TO) {
            SetState(PlayerState.RUNNING);
        }
    }

    private void onEvtCancel() {
        if (_intention == Intention.INTENTION_ATTACK) {
            setIntention(Intention.INTENTION_IDLE);
        }
    }

    /*
    =========================
    ========= THINK =========
    =========================
     */
    void thinkIdle() {
        // Does the player want to move ?
        if (InputManager.Instance.IsInputPressed(InputType.Move) || PlayerController.Instance.RunningToDestination) {
            setIntention(Intention.INTENTION_MOVE_TO);
        }

        // If has a target and attack key pressed
        if (InputManager.Instance.IsInputPressed(InputType.Attack) && TargetManager.Instance.HasTarget()) {
            setIntention(Intention.INTENTION_ATTACK);
        }
    }

    void thinkMoveTo() {
        // Arrived to destination
        if (!InputManager.Instance.IsInputPressed(InputType.Move) && !PlayerController.Instance.RunningToDestination && CanMove()) {
            notifyEvent(Event.ARRIVED);
        }

        // If move input is pressed while running to target
        if (TargetManager.Instance.HasAttackTarget() && InputManager.Instance.IsInputPressed(InputType.Move)) {
            // Cancel follow target
            TargetManager.Instance.ClearAttackTarget();
        }

        // If has a target and attack key pressed
        if (InputManager.Instance.IsInputPressed(InputType.Attack) && TargetManager.Instance.HasTarget()) {
            setIntention(Intention.INTENTION_ATTACK);
        }

        // If the player wants to move but cant
        if ((InputManager.Instance.IsInputPressed(InputType.Move) || PlayerController.Instance.RunningToDestination) && !CanMove()) {
            setIntention(Intention.INTENTION_MOVE_TO);
        }
    }

    void thinkAttack() {
        // Does the player want to move ?
        if (InputManager.Instance.IsInputPressed(InputType.Move) || PlayerController.Instance.RunningToDestination) {
            setIntention(Intention.INTENTION_MOVE_TO);
        }
    }


    /*
    =========================
    ======= INTENTION =======
    =========================
    */
    public void setIntention(Intention intention) {
        setIntention(intention, null);
    }

    public void setIntention(Intention intention, object arg0) {
       // Debug.Log($"[StateMachine] New intention: {intention}");

        _intention = intention;

        switch (intention) {
            case Intention.INTENTION_IDLE:
                onIntentionIdle();
                break;
            case Intention.INTENTION_MOVE_TO:
                onIntentionMoveTo(arg0);
                break;
            case Intention.INTENTION_ATTACK:
                onIntentionAttack((Entity) arg0);
                break;
            case Intention.INTENTION_FOLLOW:
                onIntentionFollow();
                break;
        }
    }

    private void onIntentionAttack(Entity entity) {
        if (CanMove()) {

            Transform target = TargetManager.Instance.Target.Data.ObjectTransform;

            if (target == null) {
                Debug.Log("Target is null, CANCEL event sent");
                notifyEvent(Event.CANCEL);
                return;
            }

            TargetManager.Instance.SetAttackTarget();

            float attackRange = ((PlayerStats)PlayerEntity.Instance.Stats).AttackRange;
            float distance = Vector3.Distance(transform.position, target.position);
            Debug.Log($"target: {target} distance: {distance} range: {attackRange}");

            // Is close enough? Is player already waiting for server reply?
            if (distance <= attackRange * 0.9f && !_waitingForServerReply) {
                notifyEvent(Event.READY_TO_ACT);
            } else {
                // Move to target with a 10% error margin
                PathFinderController.Instance.MoveTo(target.position, ((PlayerStats)PlayerEntity.Instance.Stats).AttackRange * 0.9f);
            }
        }
    }

    private void onIntentionFollow() {

    }

    private void onIntentionMoveTo(object arg0) {
        if (arg0 != null) {
            PathFinderController.Instance.MoveTo((Vector3)arg0);
        }

        if (CanMove()) {
            SetState(PlayerState.RUNNING);
        } else if(!_waitingForServerReply) {
            SetWaitingForServerReply(true);
            GameClient.Instance.ClientPacketHandler.UpdateMoveDirection(Vector3.zero);
        } else {
            PlayerController.Instance.StopMoving();
        }
    }

    private void onIntentionIdle() {
        SetState(PlayerState.IDLE);
    }

    public bool CanMove() {
        return (_state == PlayerState.IDLE
            || _state == PlayerState.RUNNING
            || _state == PlayerState.WALKING);
    }


    public void OnReachingTarget() {
      //  Debug.Log("On Reaching Target");
        PathFinderController.Instance.ClearPath();
        PlayerController.Instance.ResetDestination();

        if (_networkTransformShare != null) {
            _networkTransformShare.SharePosition();
        }

        if (_networkCharacterControllerShare != null) {
            // _networkCharacterControllerShare.ShareMoveDirection(Vector3.zero);
        }

        NetworkCharacterControllerShare.Instance.ForceShareMoveDirection();

        GameClient.Instance.ClientPacketHandler.SendRequestAutoAttack();
    }

    public void OnAutoAttackStart() {
        SetWaitingForServerReply(false);
        if(PlayerEntity.Instance.StartAutoAttacking()) {
            PlayerController.Instance.StartLookAt(TargetManager.Instance.AttackTarget.Data.ObjectTransform);
        }
        notifyEvent(Event.READY_TO_ACT);
    }

    public void OnAutoAttackStop() {
        SetWaitingForServerReply(false);
        PlayerEntity.Instance.StopAutoAttacking();
        PlayerController.Instance.StopLookAt();
        notifyEvent(Event.CANCEL);
    }

    // Autoattack failed (entity probably too far)
    public void OnAutoAttackFailed() {
        SetWaitingForServerReply(false);
        PlayerEntity.Instance.StopAutoAttacking(); 
        notifyEvent(Event.CANCEL);
        // PlayerController.Instance.SetCanMove(true);
    }

    public void OnMoveAllowed() {
        SetWaitingForServerReply(false);
        notifyEvent(Event.READY_TO_ACT);
    }

    public void OnMoveFailed() {
        SetWaitingForServerReply(false);
        if(CanMove()) {
            Debug.LogWarning("Player move failed but can move !");
            SetState(PlayerState.ATTACKING);
        }
    }
}
