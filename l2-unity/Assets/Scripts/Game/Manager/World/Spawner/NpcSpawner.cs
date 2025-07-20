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
        Npcgrp npcgrp = NpcgrpTable.Instance.GetNpcgrp(identity.NpcId);
        NpcName npcName = NpcNameTable.Instance.GetNpcName(identity.NpcId);

        if (npcName == null || npcgrp == null)
        {
            Debug.LogError($"Npc {identity.NpcId} could not be loaded correctly.");
            return;
        }

        identity.EntityType = npcgrp.Type;
        GameObject npcGo = CreateNpcGameObject(npcgrp, identity);
        if (npcGo == null) return;

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
        if (npc == null) return;

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
        npcGo.transform.name = identity.Name;
        npcGo.SetActive(true);

        npc.Status = status;
        // npc.Status.Hp = npc.Stats.MaxHp; // TODO: Error?
        // npc.Running = actionInfo.Running;

        npc.ReferenceHolder.NewAnimationController.Initialize();
        npc.ReferenceHolder.Gear.Initialize(npc.Identity.Id);
        npc.UpdateAppearance(appearance);
        npc.Initialize();

        UpdateEntitySync(npc, identity, status, stats, appearance, actionInfo);

        WorldSpawner.Instance.AddNpc(identity.Id, npc);
        WorldSpawner.Instance.AddObject(identity.Id, npc);
    }

    private GameObject CreateNpcGameObject(Npcgrp npcgrp, NetworkIdentity identity)
    {
        GameObject prefab = ModelTable.Instance.GetNpc(npcgrp.Mesh);
        if (prefab == null)
        {
            prefab = identity.EntityType == EntityType.Monster ? _monsterPlaceholder : _npcPlaceHolder;
            Debug.LogError($"Npc {identity.NpcId} could not be loaded correctly, loaded placeholder instead.");
        }

        identity.SetPosY(World.Instance.GetGroundHeight(identity.Position));
        GameObject npcGo = GameObject.Instantiate(prefab, identity.Position, Quaternion.identity);

        npcGo.transform.eulerAngles = new Vector3(
            npcGo.transform.eulerAngles.x,
            VectorUtils.ConvertRotToUnity(identity.Heading),
            npcGo.transform.eulerAngles.z
        );

        return npcGo;
    }
    #endregion

    #region Update
    protected override void UpdateEntitySync(
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

        // entity.Stats.UpdateStats(stats); //TODO: Needed?

        UpdateAction(entity, actionInfo);
    }

    #endregion
}