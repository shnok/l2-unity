using System.Threading;
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
        Debug.Log("Spawn User");
        identity.SetPosY(World.Instance.GetGroundHeight(identity.Position));
        identity.EntityType = EntityType.User;

        CharacterRace race = (CharacterRace)appearance.Race;
        CharacterModelType raceId = CharacterModelTypeParser.ParseRace(race, appearance.Sex, identity.IsMage);

        GameObject go = CharacterBuilder.Instance.BuildCharacterBase(raceId, appearance, identity.EntityType);
        InitializeGameObject(go, identity);

        NetworkHumanoidEntity user = go.GetComponent<NetworkHumanoidEntity>();
        InitializeUser(user, identity, status, stats, appearance, race, raceId);

        go.SetActive(true);
        go.transform.SetParent(_usersContainer.transform);

        UpdateEntityComponents(user, identity, status, stats, appearance, actionInfo);

        WorldSpawner.Instance.AddObject(identity.Id, user);
        WorldSpawner.Instance.AddPlayer(identity.Id, user);
    }

    private void InitializeGameObject(GameObject go, NetworkIdentity identity)
    {
        go.transform.position = identity.Position;
        float rotation = VectorUtils.ConvertRotToUnity(identity.Heading);
        go.transform.eulerAngles = new Vector3(go.transform.eulerAngles.x, rotation, go.transform.eulerAngles.z);
        go.transform.name = identity.Name;
    }

    private void InitializeUser(NetworkHumanoidEntity user, NetworkIdentity identity, PlayerStatus status,
        Stats stats, PlayerAppearance appearance, CharacterRace race, CharacterModelType raceId)
    {
        user.Status = status;
        user.Identity = identity;
        // user.Appearance = appearance;
        user.Stats = new Stats();
        // user.Race = race;
        // user.RaceId = raceId;

        // user.UpdateMoveType(running);



        ((NetworkEntityReferenceHolder)user.ReferenceHolder).NetworkTransformReceive.enabled = true;

        user.ReferenceHolder.NewAnimationController.Initialize();
        user.ReferenceHolder.Gear.Initialize(user.Identity.Id);

        UpdateIdentityAndStatus(user, identity, status);
        UpdateStatsAndAppearance(user, stats, appearance, null);

        // user.UpdateAppearance(appearance);
        user.Initialize();
    }
    #endregion

    #region Update
    protected override void UpdateEntity(Entity entity, NetworkIdentity identity,
        PlayerStatus status, Stats stats, PlayerAppearance appearance, EntityActionInfo actionInfo)
    {
        // Debug.LogWarning("[" + Thread.CurrentThread.ManagedThreadId + "] UPDATE ENTITY FUNC");

        UpdateEntityComponents((NetworkHumanoidEntity)entity, identity, status, stats, appearance, actionInfo);
    }

    private void UpdateEntityComponents(NetworkHumanoidEntity entity, NetworkIdentity identity, PlayerStatus status,
    Stats stats, PlayerAppearance appearance, EntityActionInfo actionInfo)
    {
        UpdateIdentityAndStatus(entity, identity, status);
        UpdateStatsAndAppearance(entity, stats, appearance, actionInfo);
    }

    private void UpdateIdentityAndStatus(NetworkHumanoidEntity entity, NetworkIdentity identity, PlayerStatus status)
    {
        entity.Identity.UpdateEntity(identity);
        ((NetworkEntityReferenceHolder)entity.ReferenceHolder).NetworkTransformReceive.SetNewPosition(identity.Position);
        ((PlayerStatus)entity.Status).UpdateStatus(status);
    }

    private void UpdateStatsAndAppearance(NetworkHumanoidEntity entity, Stats stats,
        PlayerAppearance appearance, EntityActionInfo actionInfo)
    {

        entity.UpdateAppearance(appearance);
        // ((PlayerAppearance)entity.Appearance).UpdateAppearance(appearance);

        entity.UpdatePAtkSpeed(stats.PAtkSpd);
        entity.UpdateMAtkSpeed(stats.MAtkSpd);
        entity.UpdateWalkSpeed(stats.WalkSpeed);
        entity.UpdateRunSpeed(stats.RunSpeed);

        entity.Stats.UpdateStats(stats);
        // entity.EquipAllWeapons();
        // entity.EquipAllArmors();
        if (actionInfo != null)
        {
            UpdateAction(entity, actionInfo);
        }
    }
    #endregion
}