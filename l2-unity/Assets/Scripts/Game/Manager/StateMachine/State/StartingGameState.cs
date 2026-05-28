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

    public override void HandleEvent(GameEvent evt, object arg0)
    {
        switch (evt)
        {
            case GameEvent.UI_LOADED:
            case GameEvent.WORLD_LOADED:
                base.HandleEvent(evt, arg0);
                break;
            case GameEvent.LOADING_COMPLETE:
                LoginWindow.Instance.ShowWindow();
                LoginWindow.Instance.ShowLogo();
                CharacterCreator.Instance.SpawnAllPawns();

                _stateMachine.ChangeState(GameState.LOGIN_SCREEN);
                break;
            case GameEvent.CONNECTION_ALLOWED:
                _stateMachine.ChangeState(GameState.LOGIN_CONNECTED);
                break;
            default:
                // Debug.LogWarning($"[GameStateMachine] Unhandled event {evt} for state {_stateMachine.State}");
                break;
        }
    }
}