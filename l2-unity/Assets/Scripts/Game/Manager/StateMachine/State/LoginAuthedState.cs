using UnityEngine;

public class LoginAuthedState : GameStateBase
{
    public LoginAuthedState(GameManager stateMachine) : base(stateMachine) { }


    public override void Update()
    {

    }

    public override void HandleEvent(GameEvent evt)
    {
        switch (evt)
        {
            case GameEvent.DISCONNECTED:
                _stateMachine.ChangeState(GameState.LOGIN_AUTHED);
                break;
            default:
                Debug.LogWarning($"[GameStateMachine] Unhandled event {evt} for state {_stateMachine.State}");
                break;
        }
    }
}