using UnityEngine;

public class LoginAuthedState : GameStateBase
{
    public LoginAuthedState(GameManager stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        L2LoginUI.Instance.ShowLicenseWindow();
    }

    public override void Update()
    {

    }

    public override void HandleEvent(GameEvent evt, object arg0)
    {
        switch (evt)
        {
            case GameEvent.LOGIN_DISCONNECTED:
                L2LoginUI.Instance.ShowLoginWindow();
                _stateMachine.ChangeState(GameState.LOGIN_SCREEN);
                break;
            case GameEvent.SERVER_LIST_RECEIVED:
                L2LoginUI.Instance.ShowServerSelectWindow();
                ServerSelectWindow.Instance.UpdateServerList((ServerListPacket)arg0);
                break;
            case GameEvent.PLAY_ALLOWED:
                GameClient.Instance.Connect();
                break;
            case GameEvent.AUTH_ALLOWED:
                _stateMachine.ChangeState(GameState.CHAR_SELECT);
                break;
            default:
                Debug.LogWarning($"[GameStateMachine] Unhandled event {evt} for state {_stateMachine.State}");
                break;
        }
    }
}