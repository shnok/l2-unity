using UnityEngine;

public class LoginConnectedState : GameStateBase
{
    public LoginConnectedState(GameManager stateMachine) : base(stateMachine) { }


    public override void Update()
    {

    }

    public override void HandleEvent(GameEvent evt)
    {
        switch (evt)
        {
            case GameEvent.AUTH_ALLOWED:
                _stateMachine.ChangeState(GameState.LOGIN_AUTHED);
                break;
            case GameEvent.DISCONNECTED:
                _stateMachine.ChangeState(GameState.LOGIN_SCREEN);
                break;
            default:
                Debug.LogWarning($"[GameStateMachine] Unhandled event {evt} for state {_stateMachine.State}");
                break;
        }
    }
}