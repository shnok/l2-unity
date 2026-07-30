using UnityEngine;

public class PlayerSpawner : EntitySpawnStrategy<PlayerAppearance, PlayerStats, PlayerStatus>
{
    public PlayerSpawner(EventProcessor eventProcessor)
        : base(eventProcessor)
    {
    }

    #region Spawn
    protected override void SpawnEntity(NetworkIdentity identity, PlayerStatus status,
        PlayerStats stats, PlayerAppearance appearance, EntityActionInfo actionInfo)
    {
        identity.SetPosY(World.Instance.GetGroundHeight(identity.Position));
        identity.EntityType = EntityType.Player;

        CharacterRace race = (CharacterRace)appearance.Race;

        CharacterModelType raceId = CharacterModelTypeParser.ParseRace(race, appearance.Sex, identity.IsMage);

        GameObject go = CharacterBuilder.Instance.BuildCharacterBase(raceId, appearance, identity.EntityType);
        float rotation = VectorUtils.ConvertRotToUnity(identity.Heading);
        go.transform.eulerAngles = new Vector3(go.transform.eulerAngles.x, rotation, go.transform.eulerAngles.z);
        go.transform.position = identity.Position;
        go.transform.name = "_Player";

        PlayerEntity player = go.GetComponent<PlayerEntity>();
        player.AnimationController.Initialize();
        go.GetComponent<NetworkTransformShare>().enabled = true;
        go.GetComponent<PlayerController>().enabled = true;
        go.GetComponent<PlayerController>().Initialize();

        go.SetActive(true);
        player.Stats = new PlayerStats();
        player.Status = new PlayerStatus();

        player.Gear.Initialize(player.Identity.Id);

        UpdateEntityAsync(identity, status, stats, appearance, actionInfo);

        player.Initialize();

        CameraController.Instance.enabled = true;
        CameraController.Instance.SetTarget(go);

        PlayerStateMachine.Instance.ChangeState(PlayerState.IDLE);

        WorldSpawner.Instance.AddObject(identity.Id, player);

        Debug.Log($"Spawn player {race} - {raceId}");

    }
    #endregion

    #region Update
    protected override void UpdateEntityAsync(NetworkIdentity identity, PlayerStatus status, PlayerStats stats,
    PlayerAppearance appearance, EntityActionInfo actionInfo)
    {
        PlayerEntity.Instance.Identity.UpdateIdentity(identity);
        ((PlayerStatus)PlayerEntity.Instance.Status).UpdateStatus(status);

        UpdateStatsAndAppearance(PlayerEntity.Instance, stats, appearance, actionInfo);

        CharacterInfoWindow.Instance.UpdateValues();
        InventoryWindow.Instance.RefreshWeight();

        GameManager.Instance.NotifyEvent(GameEvent.CHAR_LOADED);

        NetworkTransformShare.Instance.SharePosition();
    }

    protected override void UpdateEntitySync(Entity entity, NetworkIdentity identity, PlayerStatus status, PlayerStats stats, PlayerAppearance appearance, EntityActionInfo actionInfo)
    {

    }

    private void UpdateStatsAndAppearance(PlayerEntity entity, PlayerStats stats,
        PlayerAppearance appearance, EntityActionInfo actionInfo)
    {
        entity.UpdateMoveType(actionInfo.Running);

        entity.UpdateAppearance(appearance);
        entity.UpdatePAtkSpeed(stats.PAtkSpd);
        entity.UpdateMAtkSpeed(stats.MAtkSpd);
        entity.UpdateWalkSpeed(stats.WalkSpeed);
        entity.UpdateRunSpeed(stats.RunSpeed);

        ((PlayerStats)entity.Stats).UpdateStats(stats);

        entity.UpdateAppearance(appearance);

        UpdateAction(entity, actionInfo);
    }
    #endregion
}