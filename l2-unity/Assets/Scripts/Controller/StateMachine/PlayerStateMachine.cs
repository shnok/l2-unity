using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour {
    private static PlayerStateMachine _instance;
    public static PlayerStateMachine Instance { get { return _instance; } }

    private NetworkTransformShare _networkTransformShare;
    private NetworkCharacterControllerShare _networkCharacterControllerShare;
    //[SerializeField] private bool _isAutoAttacking = false;
    //[SerializeField] private bool _isRunningToTarget = false;

    //private PlayerState _currentState;
    //public PlayerState currentState { get { return _currentState; } set { SetPlayerState(value); } }

    [SerializeField] private Intention _intention;
    [SerializeField] private PlayerState _state;
    private bool _thinking;
    [SerializeField] private bool _waitingForServerReply;

    public PlayerState State { get { return _state; } }
    //public bool AutoAttacking { get { return _isAutoAttacking; } set { _isAutoAttacking = value; } }
    //public bool RunningToTarget { get { return _isRunningToTarget; } set { _isRunningToTarget = value; } }


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


        //if (PlayerController.Instance.CanMove && InputManager.Instance.IsInputPressed(InputType.Move)) {
        //    _isRunningToTarget = false;
        //    TargetManager.Instance.ClearAttackTarget();
        //}

        //if (InputManager.Instance.IsInputPressed(InputType.Escape)) {
        //    TargetManager.Instance.ClearTarget();
        //}

        //// for debug purpose
        //if (InputManager.Instance.IsInputPressed(InputType.DebugAttack)) {      
        //    if(TargetManager.Instance.HasTarget()) {
        //        Entity entity = TargetManager.Instance.Target.Data.ObjectTransform.GetComponent<Entity>();
        //        if (entity != null) {
        //            entity.InflictAttack(AttackType.AutoAttack);
        //        }
        //    }
        //}
    }

    public void SetState(PlayerState state) {
        Debug.Log($"[AI] New state: {state}");
        _state = state;
    }

    public void SetWaitingForServerReply(bool value) {
        Debug.LogWarning($"[AI] Waiting for server reply: {value}");
        _waitingForServerReply = value;
    }

    /*
    =========================
    ========= EVENT =========
    =========================
     */
    public void notifyEvent(Event evt) {
        if (evt != Event.THINK) {
            Debug.Log($"[AI] New event: {evt}");
        }

        notifyEvent(evt, null);
    }

    public void notifyEvent(Event evt, Entity go) {
        //        log.debug("[AI] New event: {}", evt);

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
            case Event.AUTO_ATTACK_START:
                onEvtAutoAttackStarted();
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

    }

    private void onEvtArrived() {
        if(_intention != Intention.INTENTION_ATTACK) {
            setIntention(Intention.INTENTION_IDLE);
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

    private void onEvtAutoAttackStarted() {
        // TODO: Lookat entity, set state to attack lock
        
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
        // Listen to inputs ?
        if(InputManager.Instance.IsInputPressed(InputType.Move) || PlayerController.Instance.RunningToDestination) {
            setIntention(Intention.INTENTION_MOVE_TO);
        }

        if (InputManager.Instance.IsInputPressed(InputType.Attack) && TargetManager.Instance.HasTarget()) {
            setIntention(Intention.INTENTION_ATTACK);
        }
    }

    void thinkMoveTo() {
        // Listen to inputs ?
        if (!InputManager.Instance.IsInputPressed(InputType.Move) && !PlayerController.Instance.RunningToDestination && CanMove()) {
            notifyEvent(Event.ARRIVED);
        }

        if (InputManager.Instance.IsInputPressed(InputType.Attack) && TargetManager.Instance.HasTarget()) {
            setIntention(Intention.INTENTION_ATTACK);
        }

        if ((InputManager.Instance.IsInputPressed(InputType.Move) || PlayerController.Instance.RunningToDestination) && !CanMove()) {
            setIntention(Intention.INTENTION_MOVE_TO);
        }
    }

    void thinkAttack() {
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
        Debug.Log($"[AI] New intention: {intention}");

        _intention = intention;

        if ((intention != Intention.INTENTION_FOLLOW) && (intention != Intention.INTENTION_ATTACK)) {
            // stopFollow();
        }

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

            if(target == null) {
                Debug.Log("Target is null, CANCEL event sent");
                notifyEvent(Event.CANCEL);
                return;
            }

            float attackRange = ((PlayerStats)PlayerEntity.Instance.Stats).AttackRange;
            float distance = Vector3.Distance(transform.position, target.position);
            Debug.Log($"target: {target} distance: {distance} range: {attackRange}");

            if (distance <= attackRange && !_waitingForServerReply) {
                notifyEvent(Event.READY_TO_ACT);
            }
        }
    }

    private void onIntentionFollow() {

    }

    private void onIntentionMoveTo(object arg0) {
        TargetManager.Instance.ClearAttackTarget();

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



    public bool VerifyAttackInput() {
        //if (InputManager.Instance.IsInputPressed(InputType.Attack)) {
        //    if (TargetManager.Instance.HasTarget()) {
        //        //Should follow target
        //        //Should request for autoattack once reaching target
        //        Transform target = TargetManager.Instance.Target.Data.ObjectTransform;
        //        float attackRange = ((PlayerStats)PlayerEntity.Instance.Stats).AttackRange;
        //        float distance = Vector3.Distance(transform.position, target.position);
        //        Debug.Log($"target: {target} distance: {distance} range: {attackRange}");

        //        if (distance <= attackRange) {
        //            OnReachingTarget();
        //        } else {
        //            ClickManager.Instance.HideLocator();
        //            _isRunningToTarget = true;
        //            PathFinderController.Instance.MoveTo(target.position, ((PlayerStats)PlayerEntity.Instance.Stats).AttackRange);
        //        }

        //        return true;
        //    }
        //}

        return false;
    }

    // Send a request auto attack when close enough
    public void OnReachingTarget() {
        //if (TargetManager.Instance.Target != null && (!_isAutoAttacking || TargetManager.Instance.AttackTarget != TargetManager.Instance.Target)) {
        //    Debug.LogWarning("On Reaching Target");
        //    TargetManager.Instance.SetAttackTarget();
        //    PathFinderController.Instance.ClearPath();
        //    PlayerController.Instance.ResetDestination();
        //    PlayerController.Instance.SetCanMove(false);

        //    //TODO set can move to true if action failed packet returned

        //    if (_networkTransformShare != null) {
        //        _networkTransformShare.SharePosition();
        //    }

        //    if (_networkCharacterControllerShare != null) {
        //        // _networkCharacterControllerShare.ShareMoveDirection(Vector3.zero);
        //    }

        //    NetworkCharacterControllerShare.Instance.ForceShareMoveDirection();

        //    GameClient.Instance.ClientPacketHandler.SendRequestAutoAttack();
        //}

        Debug.LogWarning("On Reaching Target");
        TargetManager.Instance.SetAttackTarget();
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
        PlayerEntity.Instance.StartAutoAttacking();
        notifyEvent(Event.READY_TO_ACT);
    }

    public void OnAutoAttackStop() {
        SetWaitingForServerReply(false);
        PlayerEntity.Instance.StopAutoAttacking();
        notifyEvent(Event.CANCEL);
    }

    // Autoattack failed (entity probably too far)
    public void OnAutoAttackFailed() {
        SetWaitingForServerReply(false);
        Debug.LogWarning("AutoAttack failed");
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


        Debug.LogWarning("Move failed");
    }
}
