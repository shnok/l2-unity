using UnityEngine;

public class StartingGameState : GameStateBase
{
    public StartingGameState(GameManager stateMachine) : base(stateMachine) { }


    public override void Enter(object arg0)
    {
        _stateMachine.LoadTables();
        SceneLoader.Instance.LoadMenu();

        _stateMachine.StartLoading();
    }

    public override void Update()
    {

    }

    public override void HandleEvent(GameEvent evt)
    {
        switch (evt)
        {
            case GameEvent.LOADING_COMPLETE:
                _stateMachine.ChangeState(GameState.LOGIN_SCREEN);
                break;
            case GameEvent.CONNECT_ALLOWED:
                _stateMachine.ChangeState(GameState.LOGIN_CONNECTED);
                break;
            default:
                Debug.LogWarning($"[GameStateMachine] Unhandled event {evt} for state {_stateMachine.State}");
                break;
        }
    }
}