using UnityEngine;
using static CharSelectedPacket;

public class TeleportingState : GameStateBase
{
    public TeleportingState(GameManager stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        //TODO: Black screen here!

        World.Instance.DestroyWorld();

        NameplatesManagerGame.Instance.ClearNameplates();

        CameraController.Instance.enabled = false;
        PlayerStateMachine.Instance.enabled = false;

        //TODO: Check if need to reload world map here!

        PlayerInfo playerInfo = GameClient.Instance.PlayerInfo;

        // Only spawn player once we loaded the world
        WorldSpawner.Instance.OnReceivePlayerInfo(playerInfo.Identity, playerInfo.Status, playerInfo.Stats, playerInfo.Appearance, playerInfo.EntityActionInfo);

        PlayerStateMachine.Instance.enabled = true;

        _stateMachine.ChangeState(GameState.IN_GAME, true);
    }

    public override void Update()
    {

    }

    public override void HandleEvent(GameEvent evt, object arg0)
    {
        switch (evt)
        {
            default:
                Debug.LogWarning($"[GameStateMachine] Unhandled event {evt} for state {_stateMachine.State}");
                break;
        }
    }
}