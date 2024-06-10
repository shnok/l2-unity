using System;
using System.Collections.Generic;
using System.IO;
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
        } else {
            Destroy(this);
        }

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

        GameObject go = CharacterBuilder.Instance.BuildCharacterBase(raceId, appearance, true);
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

        _players.Add(identity.Id, player);
        _objects.Add(identity.Id, player);


        if(!_offlineMode) {
            go.GetComponent<NetworkTransformShare>().enabled = true;
        }

        go.GetComponent<PlayerController>().enabled = true;
        go.GetComponent<PlayerController>().Initialize();

        go.SetActive(true);
        go.GetComponentInChildren<PlayerAnimationController>().Initialize();
        go.GetComponent<Gear>().Initialize();

        player.Initialize();

        go.transform.SetParent(_usersContainer.transform);

        CameraController.Instance.enabled = true;
        CameraController.Instance.SetTarget(go);
        ChatWindow.Instance.ReceiveChatMessage(new MessageLoggedIn(identity.Name));
    }

    public void SpawnUser(NetworkIdentity identity, Status status, Stats stats, PlayerAppearance appearance) {
        Debug.Log("Spawn User");
        identity.SetPosY(GetGroundHeight(identity.Position));
        identity.EntityType = EntityType.User;

        CharacterRace race = (CharacterRace)appearance.Race;
        CharacterRaceAnimation raceId = CharacterRaceAnimationParser.ParseRace(race, appearance.Race, identity.IsMage);

        GameObject go = CharacterBuilder.Instance.BuildCharacterBase(raceId, appearance, false);
        go.transform.position = identity.Position;
        go.transform.eulerAngles = new Vector3(transform.eulerAngles.x, identity.Heading, transform.eulerAngles.z);

        UserEntity user = go.GetComponent<UserEntity>();
        user.Status = status;
        user.Identity = identity;
        user.Appearance = appearance;
        user.Stats = stats;
        user.Race = race;
        user.RaceId = raceId;

        _players.Add(identity.Id, user);
        _objects.Add(identity.Id, user);

        go.GetComponent<NetworkTransformReceive>().enabled = true;

        go.transform.name = identity.Name;

        go.SetActive(true);

        user.GetComponent<NetworkAnimationController>().Initialize();
        go.GetComponent<Gear>().Initialize();
        user.Initialize();

        go.transform.SetParent(_usersContainer.transform);
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

        npc.Identity = identity;
        npc.Identity.NpcClass = npcgrp.ClassName;
        npc.Identity.Name = npcName.Name; 
        npc.Identity.Title = npcName.Title;
        if (npc.Identity.Title == null || npc.Identity.Title.Length == 0) {
            if(identity.EntityType == EntityType.Monster) {
                npc.Identity.Title = " Lvl: " + npc.Status.Level;
            }
        }
        npc.Identity.TitleColor = npcName.TitleColor;

        npc.Stats = stats;
        npc.Appearance = appearance;

        _npcs.Add(identity.Id, npc);
        _objects.Add(identity.Id, npc);

        npcGo.transform.eulerAngles = new Vector3(npcGo.transform.eulerAngles.x, identity.Heading, npcGo.transform.eulerAngles.z);

        npcGo.transform.name = identity.Name;

        npcGo.SetActive(true);

        npc.GetComponent<NetworkAnimationController>().Initialize();
        npcGo.GetComponent<Gear>().Initialize();
        npc.Initialize();
    }

    public float GetGroundHeight(Vector3 pos) {
        RaycastHit hit;
        if(Physics.Raycast(pos + Vector3.up * 1.0f, Vector3.down, out hit, 2.5f, _groundMask)) {
            return hit.point.y;
        }

        return pos.y;
    }

    public void UpdateObjectPosition(int id, Vector3 position) {
        Entity e;
        if(_objects.TryGetValue(id, out e)) {
            try {
                e.GetComponent<NetworkTransformReceive>().SetNewPosition(position);
            } catch(Exception ex) {
                Debug.LogWarning($"UpdateObjectPosition fail - Target {id} - Error {ex.Message}");
            }
        }
    }

    public void UpdateObjectRotation(int id, float angle) {
        Entity e;
        if(_objects.TryGetValue(id, out e)) {
            try {
                e.GetComponent<NetworkTransformReceive>().SetFinalRotation(angle);
            } catch (Exception ex) {
                Debug.LogWarning($"UpdateObjectRotation fail - Target {id} - Error {ex.Message}");
            }
        }
    }

    public void UpdateObjectDestination(int id, Vector3 position, int speed, bool walking) {
        Entity e;
        if (_objects.TryGetValue(id, out e)) {
            try {
                if (speed != e.Stats.Speed) {
                    e.UpdateSpeed(speed);
                }

                e.GetComponent<NetworkCharacterControllerReceive>().SetDestination(position);
                e.GetComponent<NetworkTransformReceive>().LookAt(position);
                e.OnStartMoving(walking);

            } catch (Exception ex) {
                Debug.LogWarning($"UpdateObjectDestination fail - Target {id} - Error {ex.Message}");
            }
        }
    }

    public void UpdateObjectAnimation(int id, int animId, float value) {
        Entity e;
        if(_objects.TryGetValue(id, out e)) {
            try {
               // Debug.Log($"Setting {id} anim {animId} at {value}");
                e.GetComponent<NetworkAnimationController>().SetAnimationProperty(animId, value);
            } catch (Exception ex) {
                Debug.LogWarning($"UpdateObjectAnimation fail - Target {id} - Error {ex.Message}");
            }
        }
    }

    public void InflictDamageTo(int sender, int target, int damage, int newHp, bool criticalHit) {
        Entity senderEntity;
        Entity targetEntity;
        if (_objects.TryGetValue(target, out targetEntity)) {
            _objects.TryGetValue(sender, out senderEntity);
            //networkTransform.GetComponentInParent<Entity>().ApplyDamage(sender, attackId, value);

            //Debug.Log($"{sender} inflicts {damage} damages to {target}. HP: {newHp}");
            //Debug.Log($"{senderEntity} inflicts {targetEntity}");

            try {
                if (senderEntity != null) {
                    WorldCombat.Instance.InflictAttack(senderEntity.transform, targetEntity.transform, damage, newHp, criticalHit);
                } else {
                    WorldCombat.Instance.InflictAttack(targetEntity.transform, damage, newHp, criticalHit);
                }
            } catch (Exception ex) {
                Debug.LogWarning($"InflictDamageTo fail - Sender {sender} Target {target} - Error {ex.Message}");
            }
        }
    }

    public void UpdateObjectMoveDirection(int id, int speed, Vector3 direction) {
        Entity e;
        if(_objects.TryGetValue(id, out e)) {
            try {
                if(speed != e.Stats.Speed) {
                    e.UpdateSpeed(speed);
                }

                e.GetComponent<NetworkCharacterControllerReceive>().UpdateMoveDirection(direction);
            } catch (Exception ex) {
                Debug.LogWarning($"InflictDamageTo fail - Target {id} - Error {ex.Message}");
            }
        }
    }

    public void UpdateEntityTarget(int id, int targetId) {
        Entity targeter;
        Entity targeted;
        if (_objects.TryGetValue(id, out targeter)) {
            if (_objects.TryGetValue(targetId, out targeted)) {
                try {
                    targeter.TargetId = targetId;
                    targeter.Target = targeted.transform;
                } catch (Exception) {
                    Debug.LogWarning("Trying to update a null object");
                    if(targeter == null) {
                        RemoveObject(id);
                    }
                    if (targeted == null) {
                        RemoveObject(targetId);
                    }
                }
            }
        }
    }

    public void EntityStartAutoAttacking(int id) {
        Entity e;
        if (_objects.TryGetValue(id, out e)) {
            try {
                WorldCombat.Instance.EntityStartAutoAttacking(e);
            } catch (Exception ex) {
                Debug.LogWarning($"EntityStartAutoAttacking fail - Target {id} - Error {ex.Message}");
            }
        }
    }

    public void EntityStopAutoAttacking(int id) {
        Entity e;
        if (_objects.TryGetValue(id, out e)) {
            try {
                WorldCombat.Instance.EntityStopAutoAttacking(e);
            } catch (Exception ex) {
                Debug.LogWarning($"EntityStopAutoAttacking fail - Target {id} - Error {ex.Message}");
            }
        }
    }
}
