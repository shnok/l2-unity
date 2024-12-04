using UnityEngine;

public class CharCreationState : GameStateBase
{
    public CharCreationState(GameManager stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        L2LoginUI.Instance.ShowCharCreationWindow();

        LoginCameraManager.Instance.SwitchCamera("Login");
    }

    public override void Update()
    {

    }

    public override void HandleEvent(GameEvent evt, object arg0)
    {
        switch (evt)
        {
            case GameEvent.GAME_DISCONNECTED:
                L2LoginUI.Instance.ShowLoginWindow();
                _stateMachine.ChangeState(GameState.LOGIN_SCREEN);
                break;
            case GameEvent.CHAR_LOADED:
            case GameEvent.RETURN:
                L2LoginUI.Instance.ShowCharSelectWindow();
                _stateMachine.ChangeState(GameState.CHAR_SELECT);
                break;
            case GameEvent.CHAR_CREATED:
                _stateMachine.ChangeState(GameState.CHAR_CREATION);
                break;
            default:
                Debug.LogWarning($"[GameStateMachine] Unhandled event {evt} for state {_stateMachine.State}");
                break;
        }
    }
}