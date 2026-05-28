using UnityEngine;
using static CharSelectedPacket;

public class EnteringWorldState : GameStateBase
{
    public EnteringWorldState(GameManager stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        MusicManager.Instance.Clear();
        _stateMachine.StartLoading();

        SceneLoader.Instance.LoadGame();
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
                GameObject.Destroy(L2LoginUI.Instance.gameObject);

                PlayerInfo playerInfo = GameClient.Instance.PlayerInfo;

                WorldClock.Instance.SynchronizeClock(playerInfo.CurrentGameTime);

                // Only spawn player once we loaded the world
                WorldSpawner.Instance.OnReceivePlayerInfo(playerInfo.Identity, playerInfo.Status, playerInfo.Stats, playerInfo.Appearance, playerInfo.EntityActionInfo);

                PlayerStateMachine.Instance.enabled = true;

                _stateMachine.ChangeState(GameState.IN_GAME, false);
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