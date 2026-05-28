using System.Collections;
using UnityEngine;
using static CharSelectedPacket;

public class TeleportingState : GameStateBase
{
    public TeleportingState(GameManager stateMachine) : base(stateMachine) { }

    bool teleportEnded;
    float teleportStarTime;
    float teleportEndTime;

    public override void Enter(object arg0)
    {
        teleportEnded = false;
        teleportStarTime = Time.time;

        //TODO: Black screen here!
        GameManager.Instance.StartLoading();

        World.Instance.DestroyWorld();

        NameplatesManagerGame.Instance.ClearNameplates();

        CameraController.Instance.enabled = false;
        PlayerStateMachine.Instance.enabled = false;

        NpcHtmlWindow.Instance.HideWindow(false);

        //TODO: Check if need to reload world map here!

        PlayerInfo playerInfo = GameClient.Instance.PlayerInfo;

        // Only spawn player once we loaded the world
        WorldSpawner.Instance.OnReceivePlayerInfo(playerInfo.Identity, playerInfo.Status, playerInfo.Stats, playerInfo.Appearance, playerInfo.EntityActionInfo);

        CameraController.Instance.SetTarget(PlayerEntity.Instance.gameObject);

        PlayerStateMachine.Instance.enabled = true;

        teleportEnded = true;
        teleportEndTime = Time.time;

        Debug.Log($"Teleport duration: {teleportEndTime - teleportStarTime}.");

        _stateMachine.ChangeState(GameState.IN_GAME, true);
    }

    public override void Update()
    {
        if (teleportEnded && Time.time - teleportEndTime > 1f)
        {
            teleportEnded = false;
            GameManager.Instance.StopLoading();
        }
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