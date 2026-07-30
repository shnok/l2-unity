using System;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] private bool _enableLogs;

    private static PlayerStateMachine _instance;
    public static PlayerStateMachine Instance => _instance;

    [SerializeField] private PlayerState _currentState;
    [SerializeField] private Intention _currentIntention;
    public Intention Intention { get { return _currentIntention; } }
    public PlayerState State { get { return _currentState; } }
    [SerializeField] private bool _waitingForServerReply;
    private float _waitingForServerReplyTimestamp;
    private const float STATE_TIMEOUT_SEC = 1f;

    public bool WaitingForServerReply { get { return _waitingForServerReply; } }
    public bool LogsEnabled { get { return _enableLogs; } }

    private StateBase _stateInstance;
    private IntentionBase _intentionInstance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        InitializeState();
        InitializeIntention();

        this.enabled = false;
    }

    private void Start()
    {
        _waitingForServerReply = false;
    }

    public void SetWaitingForServerReply(bool value)
    {
        if (_enableLogs) Debug.Log($"[StateMachine] Waiting for server reply: {value}");
        if (value == true)
        {
            _waitingForServerReplyTimestamp = Time.time;
        }

        _waitingForServerReply = value;
    }

    private void Update()
    {
        _stateInstance?.Update();
        _intentionInstance?.Update();
        WatchDog();
    }

    private void WatchDog()
    {
        if (_waitingForServerReply)
        {
            if (Time.time - _waitingForServerReplyTimestamp > STATE_TIMEOUT_SEC)
            {
                Debug.LogError($"[StateMachine] Waiting for server response timeout. State:{_currentState} Intention:{_currentIntention}");
                _waitingForServerReply = false;
            }
        }
    }

    public void ChangeState(PlayerState newState)
    {
        ChangeState(newState, null);
    }

    public void ChangeState(PlayerState newState, object arg0)
    {
        if (_enableLogs) Debug.Log("[StateMachine][STATE] " + newState);
        _stateInstance?.Exit();
        _currentState = newState;
        InitializeState();
        _stateInstance?.Enter(arg0);
    }

    public void ChangeIntention(Intention intention)
    {
        ChangeIntention(intention, null);
    }

    public void ChangeIntention(Intention newIntention, object arg0)
    {
        if (_waitingForServerReply)
        {
            Debug.LogWarning("Was waiting for another reply!");
            //TODO: Set this new intention in a "NextIntention" in temporary variable
            return;
        }

        if (_enableLogs) Debug.Log("[StateMachine][INTENTION] " + newIntention);
        _intentionInstance?.Exit();
        _currentIntention = newIntention;
        InitializeIntention();
        _intentionInstance?.Enter(arg0);
    }

    private void InitializeState()
    {
        _stateInstance = _currentState switch
        {
            PlayerState.IDLE => new IdleState(this),
            PlayerState.MOVING => new MovingState(this),
            PlayerState.ATTACKING => new AttackingState(this),
            PlayerState.DEAD => new DeadState(this),
            PlayerState.SITTING => new SittingState(this),
            PlayerState.SIT_WAIT => new SitWaitState(this),
            PlayerState.STANDING => new StandingState(this),
            PlayerState.SKILL => new SkillState(this),
            PlayerState.JUMPING => new JumpingState(this),
            _ => throw new ArgumentException("Invalid state")
        };
    }

    private void InitializeIntention()
    {
        _intentionInstance = _currentIntention switch
        {
            Intention.INTENTION_IDLE => new IdleIntention(this),
            Intention.INTENTION_MOVE_TO => new MoveToIntention(this),
            Intention.INTENTION_ATTACK => new AttackIntention(this),
            Intention.INTENTION_INTERACT => new InteractIntention(this),
            Intention.INTENTION_FOLLOW => new FollowIntention(this),
            Intention.INTENTION_SIT => new SitIntention(this),
            Intention.INTENTION_STAND => new StandIntention(this),
            Intention.INTENTION_MOVE => new MoveIntention(this),
            Intention.INTENTION_SKILL => new SkillIntention(this),
            Intention.INTENTION_JUMP => new JumpIntention(this),
            _ => throw new ArgumentException("Invalid intention")
        };
    }

    public bool CanMove()
    {
        return IsInMovableState() && !_waitingForServerReply;
    }

    public bool IsInMovableState()
    {
        return _currentState == PlayerState.IDLE || _currentState == PlayerState.MOVING || _currentState == PlayerState.JUMPING;
    }

    public void NotifyEvent(Event evt)
    {
        NotifyEvent(evt, null);
    }

    public void NotifyEvent(Event evt, object arg0)
    {
        if (_enableLogs) Debug.Log("[StateMachine][EVENT] " + evt);
        _stateInstance?.HandleEvent(evt, arg0);
    }

    public void OnActionAllowed()
    {
        if (_enableLogs) Debug.Log("[StateMachine] Action allowed");
        SetWaitingForServerReply(false);
        NotifyEvent(Event.ACTION_ALLOWED);
    }

    public void OnAttackAllowed()
    {
        if (_enableLogs) Debug.Log("[StateMachine] Attack allowed");
        SetWaitingForServerReply(false);
        NotifyEvent(Event.ATTACK_ALLOWED);
    }

    public void OnSkillAllowed(int hitTime)
    {
        if (_enableLogs) Debug.Log("[StateMachine] Skill allowed");
        SetWaitingForServerReply(false);
        NotifyEvent(Event.SKILL_ALLOWED, hitTime);
    }

    public void OnMagicSkillCanceled()
    {
        if (_enableLogs) Debug.Log("[StateMachine] Skill allowed");
        SetWaitingForServerReply(false);
        NotifyEvent(Event.CANCEL);
    }

    public void OnActionDenied()
    {
        if (_enableLogs) Debug.Log("[StateMachine] Action denied");
        SetWaitingForServerReply(false);
        NotifyEvent(Event.ACTION_DENIED);
    }

    public void OnStopAutoAttack()
    {
        if (_enableLogs) Debug.Log("[StateMachine] Stop autoattack");
        NotifyEvent(Event.CANCEL);
    }

    public void OnSkillNotAllowed(int skillId)
    {
        if (_enableLogs) Debug.Log("[StateMachine] Skill denied");
        SetWaitingForServerReply(false);
        NotifyEvent(Event.SKILL_DENIED);
    }
}


// OLD SYSTEM FOR REFERENCE

// public class PlayerStateMachine : MonoBehaviour
// {
//     private static PlayerStateMachine _instance;
//     public static PlayerStateMachine Instance { get { return _instance; } }

//     private NetworkTransformShare _networkTransformShare;
//     private NetworkCharacterControllerShare _networkCharacterControllerShare;

//     [SerializeField] private Intention _intention;
//     [SerializeField] private PlayerState _state;
//     [SerializeField] private Event _lastEvent;
//     [SerializeField] private bool _waitingForServerReply;
//     private bool _thinking;

//     public PlayerState State { get { return _state; } }


//     private void Awake()
//     {
//         if (_instance == null)
//         {
//             _instance = this;
//         }
//         else
//         {
//             Destroy(this);
//         }

//         _waitingForServerReply = false;
//         TryGetComponent<NetworkTransformShare>(out _networkTransformShare);
//         TryGetComponent<NetworkCharacterControllerShare>(out _networkCharacterControllerShare);
//     }

//     void OnDestroy()
//     {
//         _instance = null;
//     }


//     void Update()
//     {
//         notifyEvent(Event.THINK);
//     }

//     public void SetState(PlayerState state)
//     {
//         //  Debug.Log($"[StateMachine] New state: {state}");
//         _state = state;
//     }

//     public void SetWaitingForServerReply(bool value)
//     {
//         // Debug.LogWarning($"[StateMachine] Waiting for server reply: {value}");
//         _waitingForServerReply = value;
//     }

//     #region Events
//     /*
//     =========================
//     ========= EVENT =========
//     =========================
//      */
//     public void notifyEvent(Event evt)
//     {
//         if (evt != Event.THINK)
//         {
//             _lastEvent = evt;
//             //    Debug.Log($"[StateMachine] New event: {evt}");
//         }

//         notifyEvent(evt, null);
//     }

//     public void notifyEvent(Event evt, Entity go)
//     {
//         switch (evt)
//         {
//             case Event.THINK:
//                 onEvtThink();
//                 break;
//             case Event.DEAD:
//                 onEvtDead();
//                 break;
//             case Event.ARRIVED:
//                 onEvtArrived();
//                 break;
//             case Event.ATTACKED:
//                 onEvtAttacked(go);
//                 break;
//             case Event.READY_TO_ACT:
//                 onEvtReadyToAct();
//                 break;
//             case Event.CANCEL:
//                 onEvtCancel();
//                 break;
//             case Event.TARGET_REACHED:
//                 OnEvtTargetReached();
//                 break;
//         }
//     }

//     private void onEvtThink()
//     {
//         if (_thinking)
//         {
//             return;
//         }

//         _thinking = true;

//         if (_intention == Intention.INTENTION_IDLE)
//         {
//             thinkIdle();
//         }

//         if (_intention == Intention.INTENTION_MOVE_TO)
//         {
//             thinkMoveTo();
//         }

//         if (_intention == Intention.INTENTION_ATTACK)
//         {
//             thinkAttack();
//         }

//         _thinking = false;
//     }

//     private void onEvtDead()
//     {
//         SetState(PlayerState.DEAD);
//     }

//     private void onEvtArrived()
//     {
//         if (_intention != Intention.INTENTION_ATTACK)
//         {
//             if (TargetManager.Instance.HasAttackTarget())
//             {
//                 // arrived to target position
//                 setIntention(Intention.INTENTION_ATTACK);
//             }
//             else
//             {
//                 // regular player movement
//                 setIntention(Intention.INTENTION_IDLE);
//             }
//         }
//     }

//     private void onEvtAttacked(Entity attacker)
//     {

//     }

//     private void onEvtReadyToAct()
//     {
//         if (_intention == Intention.INTENTION_ATTACK)
//         {
//             if (_waitingForServerReply)
//             {
//                 SetWaitingForServerReply(false);
//                 SetState(PlayerState.ATTACKING);
//             }
//             else
//             {
//                 if (TargetManager.Instance.Target == null)
//                 {
//                     notifyEvent(Event.CANCEL);
//                     return;
//                 }


//                 notifyEvent(Event.TARGET_REACHED);
//             }
//         }
//         else if (_intention == Intention.INTENTION_MOVE_TO)
//         {
//             SetState(PlayerState.RUNNING);
//         }
//     }


//     public void OnEvtTargetReached()
//     {
//         //  Debug.Log("On Reaching Target");
//         PathFinderController.Instance.ClearPath();
//         PlayerController.Instance.ResetDestination();

//         if (_networkTransformShare != null)
//         {
//             _networkTransformShare.SharePosition();
//         }

//         if (_networkCharacterControllerShare != null)
//         {
//             // _networkCharacterControllerShare.ShareMoveDirection(Vector3.zero);
//         }

//         NetworkCharacterControllerShare.Instance.ForceShareMoveDirection();

//         GameClient.Instance.ClientPacketHandler.SendRequestAutoAttack();

//         SetWaitingForServerReply(true);
//     }

//     private void onEvtCancel()
//     {
//         if (_intention == Intention.INTENTION_ATTACK)
//         {
//             setIntention(Intention.INTENTION_IDLE);
//         }
//     }
//     #endregion

//     #region Actions
//     /*
//     =========================
//     ========= THINK =========
//     =========================
//      */
//     void thinkIdle()
//     {
//         if (_intention != Intention.INTENTION_IDLE)
//         {
//             return;
//         }

//         // Does the player want to move ?
//         if (InputManager.Instance.Move || PlayerController.Instance.RunningToDestination)
//         {
//             setIntention(Intention.INTENTION_MOVE_TO);
//         }
//     }

//     void thinkMoveTo()
//     {
//         if (_intention != Intention.INTENTION_MOVE_TO)
//         {
//             return;
//         }

//         // Arrived to destination
//         if (!InputManager.Instance.Move && !PlayerController.Instance.RunningToDestination && CanMove())
//         {
//             notifyEvent(Event.ARRIVED);
//         }

//         if (_intention != Intention.INTENTION_MOVE_TO)
//         {
//             return;
//         }

//         // If move input is pressed while running to target
//         if (TargetManager.Instance.HasAttackTarget() && InputManager.Instance.Move)
//         {
//             // Cancel follow target
//             TargetManager.Instance.ClearAttackTarget();
//         }

//         if (_intention != Intention.INTENTION_MOVE_TO)
//         {
//             return;
//         }

//         // If the player wants to move but cant
//         if ((InputManager.Instance.Move || PlayerController.Instance.RunningToDestination) && !CanMove())
//         {
//             setIntention(Intention.INTENTION_MOVE_TO);
//         }
//     }

//     void thinkAttack()
//     {
//         if (_intention != Intention.INTENTION_ATTACK)
//         {
//             return;
//         }

//         // Does the player want to move ?
//         if (InputManager.Instance.Move || PlayerController.Instance.RunningToDestination)
//         {
//             setIntention(Intention.INTENTION_MOVE_TO);
//         }
//     }
//     #endregion

//     #region Intentions
//     /*
//     =========================
//     ======= INTENTION =======
//     =========================
//     */
//     public void setIntention(Intention intention)
//     {
//         setIntention(intention, null);
//     }

//     public void setIntention(Intention intention, object arg0)
//     {
//         // Debug.Log($"[StateMachine] New intention: {intention}");

//         _intention = intention;

//         switch (intention)
//         {
//             case Intention.INTENTION_IDLE:
//                 onIntentionIdle();
//                 break;
//             case Intention.INTENTION_MOVE_TO:
//                 onIntentionMoveTo(arg0);
//                 break;
//             case Intention.INTENTION_ATTACK:
//                 onIntentionAttack((Entity)arg0);
//                 break;
//             case Intention.INTENTION_FOLLOW:
//                 onIntentionFollow();
//                 break;
//         }
//     }

//     private void onIntentionAttack(Entity entity)
//     {
//         if (CanMove())
//         {

//             Transform target = TargetManager.Instance.Target.Data.ObjectTransform;

//             if (target == null)
//             {
//                 Debug.Log("Target is null, CANCEL event sent");
//                 notifyEvent(Event.CANCEL);
//                 return;
//             }

//             TargetManager.Instance.SetAttackTarget();

//             float attackRange = ((PlayerStats)PlayerEntity.Instance.Stats).AttackRange;
//             float distance = Vector3.Distance(transform.position, target.position);
//             Debug.Log($"target: {target} distance: {distance} range: {attackRange}");

//             // Is close enough? Is player already waiting for server reply?
//             if (distance <= attackRange * 0.9f && !_waitingForServerReply)
//             {
//                 notifyEvent(Event.READY_TO_ACT);
//             }
//             else
//             {
//                 // Move to target with a 10% error margin
//                 PathFinderController.Instance.MoveTo(target.position, ((PlayerStats)PlayerEntity.Instance.Stats).AttackRange * 0.9f);
//             }
//         }
//     }

//     private void onIntentionFollow()
//     {

//     }

//     private void onIntentionMoveTo(object arg0)
//     {
//         if (arg0 != null)
//         {
//             PathFinderController.Instance.MoveTo((Vector3)arg0);
//         }

//         if (CanMove())
//         {
//             SetState(PlayerState.RUNNING);
//         }
//         else if (!_waitingForServerReply)
//         {
//             SetWaitingForServerReply(true);
//             GameClient.Instance.ClientPacketHandler.UpdateMoveDirection(Vector3.zero);
//         }
//         else
//         {
//             PlayerController.Instance.StopMoving();
//         }
//     }

//     private void onIntentionIdle()
//     {
//         SetState(PlayerState.IDLE);
//     }
//     #endregion

//     public bool CanMove()
//     {
//         return (_state == PlayerState.IDLE
//             || _state == PlayerState.RUNNING
//             || _state == PlayerState.WALKING);
//     }


//     public void OnAutoAttackStart()
//     {
//         SetWaitingForServerReply(false);
//         if (PlayerEntity.Instance.StartAutoAttacking())
//         {
//             PlayerController.Instance.StartLookAt(TargetManager.Instance.AttackTarget.Data.ObjectTransform);
//         }
//         notifyEvent(Event.READY_TO_ACT);
//     }

//     public void OnAutoAttackStop()
//     {
//         SetWaitingForServerReply(false);
//         PlayerEntity.Instance.StopAutoAttacking();
//         PlayerController.Instance.StopLookAt();
//         notifyEvent(Event.CANCEL);
//     }

//     // Autoattack failed (entity probably too far)
//     public void OnAutoAttackFailed()
//     {
//         SetWaitingForServerReply(false);
//         PlayerEntity.Instance.StopAutoAttacking();
//         notifyEvent(Event.CANCEL);
//         // PlayerController.Instance.SetCanMove(true);
//     }

//     public void OnMoveAllowed()
//     {
//         SetWaitingForServerReply(false);
//         notifyEvent(Event.READY_TO_ACT);
//     }

//     public void OnMoveFailed()
//     {
//         SetWaitingForServerReply(false);
//         if (CanMove())
//         {
//             Debug.LogWarning("Player move failed but can move !");
//             SetState(PlayerState.ATTACKING);
//         }
//     }
// }
