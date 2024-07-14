using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class World : MonoBehaviour {
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _playerPlaceholder;
    [SerializeField] private GameObject _userPlaceholder;
    [SerializeField] private GameObject _npcPlaceHolder;
    [SerializeField] private GameObject _monsterPlaceholder;

    [SerializeField] private GameObject _monstersContainer;
    [SerializeField] private GameObject _npcsContainer;
    [SerializeField] private GameObject _usersContainer;

    private EventProcessor _eventProcessor;

    private Dictionary<int, Entity> _players = new Dictionary<int, Entity>();
    private Dictionary<int, Entity> _npcs = new Dictionary<int, Entity>();
    private Dictionary<int, Entity> _objects = new Dictionary<int, Entity>();

    [Header("Layer Masks")]
    [SerializeField] private LayerMask _entityMask;
    [SerializeField] private LayerMask _entityClickAreaMask;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private LayerMask _clickThroughMask;
    [SerializeField] private LayerMask _groundMask;

    [SerializeField] private bool _offlineMode = false;

    public bool OfflineMode { get { return _offlineMode; } }
    public LayerMask GroundMask { get { return _groundMask; } }

    private static World _instance;
    public static World Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
        }

        _eventProcessor = EventProcessor.Instance;
        _playerPlaceholder = Resources.Load<GameObject>("Prefab/Player_FDarkElf");
        _userPlaceholder = Resources.Load<GameObject>("Prefab/User_FDarkElf");
        _npcPlaceHolder = Resources.Load<GameObject>("Prefab/Npc");
        _monsterPlaceholder = Resources.Load<GameObject>("Data/Animations/LineageMonsters/gremlin/gremlin_prefab");
        _npcsContainer = GameObject.Find("Npcs");
        _monstersContainer = GameObject.Find("Monsters");
        _usersContainer = GameObject.Find("Users");
    }

    void OnDestroy() {
        _instance = null;
    }

    void Start() {
        UpdateMasks();
    }

    void UpdateMasks() {
        NameplatesManager.Instance.SetMask(_entityMask);
        Geodata.Instance.ObstacleMask = _obstacleMask;
        ClickManager.Instance.SetMasks(_entityClickAreaMask, _clickThroughMask);
        CameraController.Instance.SetMask(_obstacleMask);
    }

    public void ClearEntities() {
        _objects.Clear();
        _players.Clear();
        _npcs.Clear();
    }

    public void RemoveObject(int id) {
        Entity transform;
        if(_objects.TryGetValue(id, out transform)) {
            _players.Remove(id);
            _npcs.Remove(id);
            _objects.Remove(id);

            Destroy(transform.gameObject);
        }
    }

    public void SpawnPlayerOfflineMode() {
        if(_offlineMode) {
            PlayerEntity entity = _playerPlaceholder.GetComponent<PlayerEntity>();
            entity.Identity.Position = _playerPlaceholder.transform.position;
            // TODO: Add default stats
            SpawnPlayer(entity.Identity, (PlayerStatus) entity.Status, new PlayerStats(), new PlayerAppearance());
        }
    }

    public void SpawnPlayer(NetworkIdentity identity, PlayerStatus status, PlayerStats stats, PlayerAppearance appearance) {
        identity.SetPosY(GetGroundHeight(identity.Position));
        identity.EntityType = EntityType.Player;

        CharacterRace race = (CharacterRace) appearance.Race;
        CharacterRaceAnimation raceId = CharacterRaceAnimationParser.ParseRace(race, appearance.Race, identity.IsMage);

        GameObject go = CharacterBuilder.Instance.BuildCharacterBase(raceId, appearance, identity.EntityType);
        go.transform.eulerAngles = new Vector3(transform.eulerAngles.x, identity.Heading, transform.eulerAngles.z);
        go.transform.position = identity.Position;
        go.transform.name = "Player";

        PlayerEntity player = go.GetComponent<PlayerEntity>();

        player.Status = status;
        player.Identity = identity;
        player.Stats = stats;
        player.Appearance = appearance;
        player.Race = race;
        player.RaceId = raceId;

        go.GetComponent<NetworkTransformShare>().enabled = true;
        go.GetComponent<PlayerController>().enabled = true;
        go.GetComponent<PlayerController>().Initialize();

        go.SetActive(true);
        go.GetComponentInChildren<PlayerAnimationController>().Initialize();
        go.GetComponent<Gear>().Initialize(player.Identity.Id, player.RaceId);

        player.Initialize();

        go.transform.SetParent(_usersContainer.transform);

        CameraController.Instance.enabled = true;
        CameraController.Instance.SetTarget(go);

        //ChatWindow.Instance.ReceiveChatMessage(new MessageLoggedIn(identity.Name));

        CharacterInfoWindow.Instance.UpdateValues();

        _players.Add(identity.Id, player);
        _objects.Add(identity.Id, player);
    }

    public void SpawnUser(NetworkIdentity identity, Status status, Stats stats, PlayerAppearance appearance) {
        Debug.Log("Spawn User");
        identity.SetPosY(GetGroundHeight(identity.Position));
        identity.EntityType = EntityType.User;

        CharacterRace race = (CharacterRace)appearance.Race;
        CharacterRaceAnimation raceId = CharacterRaceAnimationParser.ParseRace(race, appearance.Race, identity.IsMage);

        GameObject go = CharacterBuilder.Instance.BuildCharacterBase(raceId, appearance, identity.EntityType);
        go.transform.position = identity.Position;
        go.transform.eulerAngles = new Vector3(transform.eulerAngles.x, identity.Heading, transform.eulerAngles.z);

        UserEntity user = go.GetComponent<UserEntity>();

        user.Status = status;
        user.Identity = identity;
        user.Appearance = appearance;
        user.Stats = stats;
        user.Race = race;
        user.RaceId = raceId;

        go.GetComponent<NetworkTransformReceive>().enabled = true;

        go.transform.name = identity.Name;

        go.SetActive(true);

        user.GetComponent<NetworkAnimationController>().Initialize();
        go.GetComponent<Gear>().Initialize(user.Identity.Id, user.RaceId);
        user.Initialize();

        go.transform.SetParent(_usersContainer.transform);

        _players.Add(identity.Id, user);
        _objects.Add(identity.Id, user);
    }

    public void SpawnNpc(NetworkIdentity identity, NpcStatus status, Stats stats) {


        Npcgrp npcgrp = NpcgrpTable.Instance.GetNpcgrp(identity.NpcId);
        NpcName npcName = NpcNameTable.Instance.GetNpcName(identity.NpcId);
        if (npcName == null || npcgrp == null) {
            Debug.LogError($"Npc {identity.NpcId} could not be loaded correctly.");
            return;
        }

        GameObject go = ModelTable.Instance.GetNpc(npcgrp.Mesh);
        if (go == null) {
            Debug.LogError($"Npc {identity.NpcId} could not be loaded correctly.");
            return;
        }

        identity.SetPosY(GetGroundHeight(identity.Position));
        GameObject npcGo = Instantiate(go, identity.Position, Quaternion.identity);
        NpcData npcData = new NpcData(npcName, npcgrp);

        identity.EntityType = EntityTypeParser.ParseEntityType(npcgrp.ClassName);
        Entity npc;

        if (identity.EntityType == EntityType.NPC) {
            npcGo.transform.SetParent(_npcsContainer.transform);
            npc = npcGo.GetComponent<NpcEntity>();
            ((NpcEntity)npc).NpcData = npcData;
        } else {
            npcGo.transform.SetParent(_monstersContainer.transform);
            npc = npcGo.GetComponent<MonsterEntity>();
            ((MonsterEntity)npc).NpcData = npcData;
        }

        Appearance appearance = new Appearance();
        appearance.RHand = npcgrp.Rhand;
        appearance.LHand = npcgrp.Lhand;
        appearance.CollisionRadius = npcgrp.CollisionRadius;
        appearance.CollisionHeight = npcgrp.CollisionHeight;

        npc.Status = status;

        npc.Stats = stats;

        npc.Identity = identity;
        npc.Identity.NpcClass = npcgrp.ClassName;
        npc.Identity.Name = npcName.Name; 
        npc.Identity.Title = npcName.Title;
        if (npc.Identity.Title == null || npc.Identity.Title.Length == 0) {
            if(identity.EntityType == EntityType.Monster) {
                npc.Identity.Title = " Lvl: " + npc.Stats.Level;
            }
        }
        npc.Identity.TitleColor = npcName.TitleColor;

        npc.Appearance = appearance;

        npcGo.transform.eulerAngles = new Vector3(npcGo.transform.eulerAngles.x, identity.Heading, npcGo.transform.eulerAngles.z);

        npcGo.transform.name = identity.Name;

        npcGo.SetActive(true);

        npc.GetComponent<NetworkAnimationController>().Initialize();
        npcGo.GetComponent<Gear>().Initialize(npc.Identity.Id, npc.RaceId);
        npc.Initialize();

        _npcs.Add(identity.Id, npc);
        _objects.Add(identity.Id, npc);
    }

    public float GetGroundHeight(Vector3 pos) {
        RaycastHit hit;
        if(Physics.Raycast(pos + Vector3.up * 1.0f, Vector3.down, out hit, 2.5f, _groundMask)) {
            return hit.point.y;
        }

        return pos.y;
    }

    public Task UpdateObjectPosition(int id, Vector3 position) {
        return ExecuteWithEntityAsync(id, e => {
            e.GetComponent<NetworkTransformReceive>().SetNewPosition(position);
        });
    }

    public Task UpdateObjectRotation(int id, float angle) {
        return ExecuteWithEntityAsync(id, e => {
            e.GetComponent<NetworkTransformReceive>().SetFinalRotation(angle);
        });
    }

    public Task UpdateObjectDestination(int id, Vector3 position, int speed, bool walking) {
        return ExecuteWithEntityAsync(id, e => {
            if (speed != e.Stats.Speed) {
                e.UpdateSpeed(speed);
            }

            e.GetComponent<NetworkCharacterControllerReceive>().SetDestination(position);
            e.GetComponent<NetworkTransformReceive>().LookAt(position);
            e.OnStartMoving(walking);
        });
    }

    public Task UpdateObjectAnimation(int id, int animId, float value) {
        return ExecuteWithEntityAsync(id, e => {
            e.GetComponent<NetworkAnimationController>().SetAnimationProperty(animId, value);
        });
    }

    public Task InflictDamageTo(int sender, int target, int damage, bool criticalHit) {
        return ExecuteWithEntitiesAsync(sender, target, (senderEntity, targetEntity) => {
            if (senderEntity != null) {
                WorldCombat.Instance.InflictAttack(senderEntity.transform, targetEntity.transform, damage, criticalHit);
            } else {
                WorldCombat.Instance.InflictAttack(targetEntity.transform, damage, criticalHit);
            }
        });
    }

    public Task UpdateObjectMoveDirection(int id, int speed, Vector3 direction) {
        return ExecuteWithEntityAsync(id, e => {
            if (speed != e.Stats.Speed) {
                e.UpdateSpeed(speed);
            }

            e.GetComponent<NetworkCharacterControllerReceive>().UpdateMoveDirection(direction);
        });
    }

    public Task UpdateEntityTarget(int id, int targetId) {
        return ExecuteWithEntitiesAsync(id, targetId, (targeter, targeted) => {
            targeter.TargetId = targetId;
            targeter.Target = targeted.transform;
        });
    }

    public Task EntityStartAutoAttacking(int id) {
        return ExecuteWithEntityAsync(id, e => {
            WorldCombat.Instance.EntityStartAutoAttacking(e);
        });
    }

    public Task EntityStopAutoAttacking(int id) {
        return ExecuteWithEntityAsync(id, e => {
            WorldCombat.Instance.EntityStopAutoAttacking(e);
        });
    }

    public Task StatusUpdate(int id, List<StatusUpdatePacket.Attribute> attributes) {
        return ExecuteWithEntityAsync(id, e => {
            WorldCombat.Instance.StatusUpdate(e, attributes);
            if(e == PlayerEntity.Instance) { 
                CharacterInfoWindow.Instance.UpdateValues();
            }
        });
    }

    // Wait for entity to be fully loaded
    private async Task<Entity> GetEntityAsync(int id) {
        Entity entity;
        lock (_objects) {
            if (!_objects.TryGetValue(id, out entity)) {
                //Debug.LogWarning($"GetEntityAsync - Entity {id} not found, retrying...");
            }
        }

        if (entity == null) {
            await Task.Delay(150); // Wait for 150 ms retrying

            lock (_objects) {
                if (!_objects.TryGetValue(id, out entity)) {
                    Debug.LogWarning($"GetEntityAsync - Entity {id} not found after retry");
                    return null;
                } else {
                   // Debug.LogWarning($"GetEntityAsync - Entity {id} found after retry");
                }
            }
        }

        return entity;
    }

    // Execute action after entity is loaded
    private async Task ExecuteWithEntityAsync(int id, Action<Entity> action) {
        var entity = await GetEntityAsync(id);
        if (entity != null) {
            try {
                _eventProcessor.QueueEvent(() => action(entity));
            } catch (Exception ex) {
                Debug.LogWarning($"Operation failed - Target {id} - Error {ex.Message}");
            }
        }
    }

    // Execute action after 2 entities are loaded
    private async Task ExecuteWithEntitiesAsync(int id1, int id2, Action<Entity, Entity> action) {
        var entity1Task = GetEntityAsync(id1);
        var entity2Task = GetEntityAsync(id2);

        await Task.WhenAll(entity1Task, entity2Task);

        var entity1 = await entity1Task;
        var entity2 = await entity2Task;

        if (entity1 != null && entity2 != null) {
            try {
                _eventProcessor.QueueEvent(() => action(entity1, entity2));
            } catch (Exception ex) {
                Debug.LogWarning($"Operation failed - Target {id1} or {id2} - Error {ex.Message}");
            }
        }
    }
}
