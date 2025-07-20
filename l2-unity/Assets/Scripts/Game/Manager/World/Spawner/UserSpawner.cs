using UnityEngine;

public class UserSpawner : EntitySpawnStrategy<PlayerAppearance, Stats, PlayerStatus>
{
    private Transform _usersContainer;
    public UserSpawner(EventProcessor eventProcessor, Transform usersContainer)
        : base(eventProcessor)
    {
        _usersContainer = usersContainer;
    }

    #region Spawn
    protected override void SpawnEntity(NetworkIdentity identity, PlayerStatus status,
        Stats stats, PlayerAppearance appearance, EntityActionInfo actionInfo)
    {
        identity.SetPosY(World.Instance.GetGroundHeight(identity.Position));
        identity.EntityType = EntityType.User;

        CharacterRace race = (CharacterRace)appearance.Race;

        CharacterModelType raceId = CharacterModelTypeParser.ParseRace(race, appearance.Sex, identity.IsMage);

        GameObject go = CharacterBuilder.Instance.BuildCharacterBase(raceId, appearance, identity.EntityType);
        float rotation = VectorUtils.ConvertRotToUnity(identity.Heading);
        go.transform.eulerAngles = new Vector3(go.transform.eulerAngles.x, rotation, go.transform.eulerAngles.z);
        go.transform.position = identity.Position;
        go.transform.name = identity.Name;
        go.transform.SetParent(_usersContainer.transform);

        NetworkHumanoidEntity user = go.GetComponent<NetworkHumanoidEntity>();
        user.AnimationController.Initialize();
        ((NetworkEntityReferenceHolder)user.ReferenceHolder).NetworkTransformReceive.enabled = true;

        go.SetActive(true);

        user.Stats = new Stats();
        user.Status = new PlayerStatus();

        user.Gear.Initialize(user.Identity.Id);

        user.Initialize();

        UpdateEntitySync(user, identity, status, stats, appearance, actionInfo);

        WorldSpawner.Instance.AddObject(identity.Id, user);
        WorldSpawner.Instance.AddPlayer(identity.Id, user);

        Debug.Log($"Spawn user {race} - {raceId}");

    }
    #endregion

    #region Update
    protected override void UpdateEntitySync(Entity entity, NetworkIdentity identity,
        PlayerStatus status, Stats stats, PlayerAppearance appearance, EntityActionInfo actionInfo)
    {
        entity.Identity.UpdateIdentity(identity);
        ((PlayerStatus)entity.Status).UpdateStatus(status);

        entity.UpdateMoveType(actionInfo.Running);

        entity.UpdateAppearance(appearance);
        entity.UpdatePAtkSpeed(stats.PAtkSpd);
        entity.UpdateMAtkSpeed(stats.MAtkSpd);
        entity.UpdateWalkSpeed(stats.WalkSpeed);
        entity.UpdateRunSpeed(stats.RunSpeed);

        entity.Stats.UpdateStats(stats);

        entity.UpdateAppearance(appearance);

        UpdateAction(entity, actionInfo);

        ((NetworkEntityReferenceHolder)entity.ReferenceHolder)
            .NetworkTransformReceive
            .SetNewPosition(identity.Position);
    }
    #endregion
}