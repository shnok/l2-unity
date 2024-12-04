using UnityEngine;

public class InGameState : GameStateBase
{
    public InGameState(GameManager stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        GameClient.Instance.ClientPacketHandler.SendLoadWorld();
    }

    public override void Update()
    {

    }

    public override void HandleEvent(GameEvent evt, object arg0)
    {
        switch (evt)
        {
            case GameEvent.RESTART_ALLOWED:
                _stateMachine.ChangeState(GameState.RESTARTING);
                break;
            case GameEvent.CHAR_LOADED:
                _stateMachine.StopLoading();
                break;
            case GameEvent.GAME_DISCONNECTED:
                _stateMachine.ChangeState(GameState.DISONNECTING);
                break;
            default:
                Debug.LogWarning($"[GameStateMachine] Unhandled event {evt} for state {_stateMachine.State}");
                break;
        }
    }
}