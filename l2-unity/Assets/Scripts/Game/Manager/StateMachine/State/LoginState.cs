using UnityEngine;

public class LoginState : GameStateBase
{
    public LoginState(GameManager stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        if (!_stateMachine.Loading)
        {
            LoginCameraManager.Instance.SwitchCamera("Login");
            L2LoginUI.Instance.ShowLoginWindow();
        }
    }

    public override void Update()
    {

    }

    public override void HandleEvent(GameEvent evt, object arg0)
    {
        switch (evt)
        {
            case GameEvent.LOADING_COMPLETE:
                _stateMachine.StopLoading();
                LoginCameraManager.Instance.SwitchCamera("Login");
                LoginWindow.Instance.ShowWindow();
                LoginWindow.Instance.ShowLogo();
                PawnCreator.Instance.SpawnAllPawns();
                break;
            case GameEvent.CONNECT_ALLOWED:
                _stateMachine.ChangeState(GameState.LOGIN_CONNECTED);
                break;
            default:
                Debug.LogWarning($"[GameStateMachine] Unhandled event {evt} for state {_stateMachine.State}");
                break;
        }
    }

    private void ResetLoginState()
    {
    }
}