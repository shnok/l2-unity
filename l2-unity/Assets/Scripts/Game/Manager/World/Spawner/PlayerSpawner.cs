using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
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
        InitializeGameObject(go, identity);

        PlayerEntity player = go.GetComponent<PlayerEntity>();
        InitializePlayer(player, identity, status, stats, appearance, race, raceId, actionInfo);

        AddEntity(identity, player);

        Debug.Log($"Spawn player {race} - {raceId}");

    }

    private void InitializeGameObject(GameObject go, NetworkIdentity identity)
    {
        float rotation = VectorUtils.ConvertRotToUnity(identity.Heading);
        go.transform.eulerAngles = new Vector3(go.transform.eulerAngles.x, rotation, go.transform.eulerAngles.z);
        go.transform.position = identity.Position;
        go.transform.name = "_Player";
        go.layer = LayerMask.NameToLayer("Invisible");
    }

    private void InitializePlayer(PlayerEntity player, NetworkIdentity identity, Status status,
        Stats stats, PlayerAppearance appearance, CharacterRace race, CharacterModelType raceId, EntityActionInfo actionInfo)
    {
        player.Status = status;
        player.Identity = identity;
        player.Stats = stats;
        player.Appearance = appearance;
        player.Race = race;
        player.RaceId = raceId;

        var go = player.gameObject;
        go.GetComponent<NewPlayerAnimationController>().Initialize();
        go.GetComponent<NetworkTransformShare>().enabled = true;
        go.GetComponent<PlayerController>().enabled = true;
        go.GetComponent<PlayerController>().Initialize();

        go.SetActive(true);
        go.GetComponent<Gear>().Initialize(player.Identity.Id, player.RaceId);

        player.Initialize();
        player.UpdateMoveType(actionInfo.Running);

        CameraController.Instance.enabled = true;
        CameraController.Instance.SetTarget(go);

        PlayerStateMachine.Instance.ChangeState(PlayerState.IDLE);
    }

    protected override void AddEntity(NetworkIdentity identity, Entity player)
    {
        WorldSpawner.Instance.AddObject(identity.Id, player);
    }
    #endregion

    #region Update
    protected override void FindAndUpdateEntity(NetworkIdentity identity, PlayerStatus status, PlayerStats stats,
    PlayerAppearance appearance, EntityActionInfo actionInfo)
    {
        UpdateEntity(PlayerEntity.Instance, identity, status, stats, appearance, actionInfo);
    }

    protected override void UpdateEntity(Entity entity, NetworkIdentity identity,
        PlayerStatus status, PlayerStats stats, PlayerAppearance appearance, EntityActionInfo actionInfo)
    {
        entity.gameObject.layer = LayerMask.NameToLayer("Player");

        UpdateEntityComponents((PlayerEntity)entity, identity, status, stats, appearance, actionInfo);

        // Player-specific updates
        CharacterInfoWindow.Instance.UpdateValues();
        InventoryWindow.Instance.RefreshWeight();

        GameManager.Instance.NotifyEvent(GameEvent.CHAR_LOADED);

        NetworkTransformShare.Instance.SharePosition();
    }

    private void UpdateEntityComponents(PlayerEntity entity, NetworkIdentity identity, PlayerStatus status,
    PlayerStats stats, PlayerAppearance appearance, EntityActionInfo actionInfo)
    {
        UpdateIdentityAndStatus(entity, identity, status);
        UpdateStatsAndAppearance(entity, stats, appearance, actionInfo);
    }

    private void UpdateIdentityAndStatus(PlayerEntity entity, NetworkIdentity identity, PlayerStatus status)
    {
        entity.Identity.UpdateEntity(identity);
        ((PlayerStatus)entity.Status).UpdateStatus(status);
    }

    private void UpdateStatsAndAppearance(PlayerEntity entity, PlayerStats stats,
        PlayerAppearance appearance, EntityActionInfo actionInfo)
    {
        ((PlayerStats)entity.Stats).UpdateStats(stats);
        entity.UpdateMoveType(actionInfo.Running);

        ((PlayerAppearance)entity.Appearance).UpdateAppearance(appearance);

        entity.UpdatePAtkSpeed(stats.PAtkSpd);
        entity.UpdateMAtkSpeed(stats.MAtkSpd);
        entity.UpdateWalkSpeed(stats.WalkSpeed);
        entity.UpdateRunSpeed(stats.RunSpeed);
        entity.EquipAllWeapons();
        entity.EquipAllArmors();

        UpdateAction(entity, actionInfo);
    }
    #endregion
}