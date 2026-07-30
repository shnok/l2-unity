using UnityEngine;

public class RestartingState : GameStateBase
{
    public RestartingState(GameManager stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        MusicManager.Instance.Clear();
        _stateMachine.StartLoading();

        SceneLoader.Instance.LoadMenu();
    }

    public override void Update()
    {

    }

    public override void HandleEvent(GameEvent evt, object arg0)
    {
        switch (evt)
        {
            case GameEvent.UI_LOADED:
            case GameEvent.WORLD_LOADED:
                base.HandleEvent(evt, arg0);
                break;
            case GameEvent.LOADING_COMPLETE:
                _stateMachine.StopLoading();
                _stateMachine.ChangeState(GameState.CHAR_SELECT);
                break;
            case GameEvent.GAME_DISCONNECTED:
                L2LoginUI.Instance.ShowLoginWindow();
                _stateMachine.ChangeState(GameState.LOGIN_SCREEN);
                break;
            default:
                Debug.LogWarning($"[GameStateMachine] Unhandled event {evt} for state {_stateMachine.State}");
                break;
        }
    }
}