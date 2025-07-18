using System.Threading;
using UnityEngine;

public class NpcSpawner : EntitySpawnStrategy<Appearance, Stats, NpcStatus>
{
    private readonly Transform _npcsContainer;
    private readonly Transform _monstersContainer;
    private readonly GameObject _npcPlaceHolder;
    private readonly GameObject _monsterPlaceholder;

    public NpcSpawner(
        EventProcessor eventProcessor,
        Transform npcsContainer,
        Transform monstersContainer)
        : base(eventProcessor)
    {
        _npcsContainer = npcsContainer;
        _monstersContainer = monstersContainer;
        _npcPlaceHolder = ModelTable.Instance.GetNpc("LineageNPCs.a_smith_MDwarf_m00");
        _monsterPlaceholder = ModelTable.Instance.GetNpc("LineageMonsters.gremlin_m00");
    }

    #region Spawn
    protected override void SpawnEntity(NetworkIdentity identity, NpcStatus status,
        Stats stats, Appearance appearance, EntityActionInfo actionInfo)
    {
        var npcgrp = NpcgrpTable.Instance.GetNpcgrp(identity.NpcId);
        var npcName = NpcNameTable.Instance.GetNpcName(identity.NpcId);

        if (npcName == null || npcgrp == null)
        {
            Debug.LogError($"Npc {identity.NpcId} could not be loaded correctly.");
            return;
        }

        identity.EntityType = npcgrp.Type;
        var npcGo = CreateNpcGameObject(npcgrp, identity);
        if (npcGo == null) return;

        var npc = InitializeNpcEntity(npcGo, identity);
        if (npc == null) return;

        ConfigureNpcComponents(npc, identity, status, stats, appearance, npcgrp, npcName, actionInfo);

        AddEntity(identity, npc);
    }

    private GameObject CreateNpcGameObject(Npcgrp npcgrp, NetworkIdentity identity)
    {
        var prefab = ModelTable.Instance.GetNpc(npcgrp.Mesh);
        if (prefab == null)
        {
            prefab = identity.EntityType == EntityType.Monster ? _monsterPlaceholder : _npcPlaceHolder;
            Debug.LogError($"Npc {identity.NpcId} could not be loaded correctly, loaded placeholder instead.");
        }

        identity.SetPosY(World.Instance.GetGroundHeight(identity.Position));
        var npcGo = GameObject.Instantiate(prefab, identity.Position, Quaternion.identity);

        npcGo.transform.eulerAngles = new Vector3(
            npcGo.transform.eulerAngles.x,
            VectorUtils.ConvertRotToUnity(identity.Heading),
            npcGo.transform.eulerAngles.z
        );

        return npcGo;
    }

    private Entity InitializeNpcEntity(GameObject npcGo, NetworkIdentity identity)
    {
        Entity npc;
        if (identity.EntityType == EntityType.NPC)
        {
            npcGo.transform.SetParent(_npcsContainer);
            npc = npcGo.GetComponent<NetworkHumanoidEntity>();
        }
        else
        {
            npcGo.transform.SetParent(_monstersContainer);
            npc = npcGo.GetComponent<NetworkMonsterEntity>();
        }
        return npc;
    }

    private void ConfigureNpcComponents(
        Entity npc,
        NetworkIdentity identity,
        Status status,
        Stats stats,
        Appearance appearance,
        Npcgrp npcgrp,
        NpcName npcName,
        EntityActionInfo actionInfo)
    {
        // ConfigureAppearance(appearance, npcgrp);
        ConfigureIdentity(npc, identity, npcgrp, npcName);

        npc.Status = status;
        // npc.Stats = stats;
        npc.Status.Hp = npc.Stats.MaxHp;

        // npc.Appearance = appearance;
        npc.Running = actionInfo.Running;

        var npcGo = npc.gameObject;
        npcGo.transform.name = identity.Name;
        npcGo.SetActive(true);

        npc.ReferenceHolder.NewAnimationController.Initialize();
        npc.ReferenceHolder.Gear.Initialize(npc.Identity.Id);
        npc.UpdateAppearance(appearance);
        npc.Initialize();
    }

    private void ConfigureIdentity(Entity npc, NetworkIdentity identity, Npcgrp npcgrp, NpcName npcName)
    {
        npc.Identity = identity;
        npc.Identity.NpcClass = npcgrp.ClassName;
        npc.Identity.IsHpShowable = npcgrp.HpVisible;

        if (string.IsNullOrEmpty(npc.Identity.Name))
            npc.Identity.Name = npcName.Name;

        if (string.IsNullOrEmpty(npc.Identity.Title))
        {
            npc.Identity.Title = npcName.Title;
            if (string.IsNullOrEmpty(npc.Identity.Title) && identity.EntityType == EntityType.Monster)
                npc.Identity.Title = npcName.Title;
        }

        npc.Appearance.ServerTitleColor = npcName.TitleColor;
    }

    protected override void AddEntity(NetworkIdentity identity, Entity npc)
    {
        WorldSpawner.Instance.AddNpc(identity.Id, npc);
        WorldSpawner.Instance.AddObject(identity.Id, npc);
    }
    #endregion

    #region Update
    protected override void UpdateEntity(Entity entity, NetworkIdentity identity,
        NpcStatus status, Stats stats, Appearance appearance, EntityActionInfo actionInfo)
    {
        UpdateNpcComponents(entity, identity, status, stats, appearance, actionInfo);
    }

    private void UpdateNpcComponents(
        Entity entity,
        NetworkIdentity identity,
        NpcStatus status,
        Stats stats,
        Appearance appearance,
        EntityActionInfo actionInfo)
    {
        entity.Identity.UpdateForNpcs(identity);

        var networkTransform = ((NetworkEntityReferenceHolder)entity.ReferenceHolder).NetworkTransformReceive;
        networkTransform.SetNewPosition(identity.Position);
        entity.UpdateAppearance(appearance);
        entity.Running = actionInfo.Running;

        entity.UpdatePAtkSpeed(stats.PAtkSpd);
        entity.UpdateMAtkSpeed(stats.MAtkSpd);
        entity.UpdateWalkSpeed(stats.WalkSpeed);
        entity.UpdateRunSpeed(stats.RunSpeed);

        entity.Stats.UpdateStats(stats);

        UpdateAction(entity, actionInfo);
    }

    #endregion
}