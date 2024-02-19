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

    void Awake() {
        if(_instance == null) {
            _instance = this;
        }

        _playerPlaceholder = Resources.Load<GameObject>("Prefab/Player");
        _userPlaceholder = Resources.Load<GameObject>("Prefab/User");
        _npcPlaceHolder = Resources.Load<GameObject>("Prefab/Npc");
        _monsterPlaceholder = Resources.Load<GameObject>("Data/Animations/LineageMonsters/gremlin/gremlin_prefab");
        _npcsContainer = GameObject.Find("Npcs");
        _monstersContainer = GameObject.Find("Monsters");
        _usersContainer = GameObject.Find("Users");
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
            SpawnPlayer(entity.Identity, (PlayerStatus) entity.Status);
        }
    }

    public void SpawnPlayer(NetworkIdentity identity, PlayerStatus status) {
        identity.SetPosY(GetGroundHeight(identity.Position));
        identity.EntityType = EntityType.Player;
        identity.CollisionHeight = 0.45f;
        GameObject go = (GameObject)Instantiate(_playerPlaceholder, identity.Position, Quaternion.identity);
        PlayerEntity player = go.GetComponent<PlayerEntity>();
        player.Status = status;
        player.Identity = identity;

        _players.Add(identity.Id, player);
        _objects.Add(identity.Id, player);

        go.GetComponent<PlayerController>().enabled = true;

        if(!_offlineMode) {
            go.GetComponent<NetworkTransformShare>().enabled = true;
        }
          
        go.transform.name = "Player";
        go.SetActive(true);

        go.transform.SetParent(_usersContainer.transform);

        CameraController.Instance.SetTarget(go);
        CameraController.Instance.enabled = true;

        ChatWindow.Instance.ReceiveChatMessage(new MessageLoggedIn(identity.Name));
    }

    public void SpawnUser(NetworkIdentity identity, PlayerStatus status) {
        Debug.Log("Spawn User");
        identity.SetPosY(GetGroundHeight(identity.Position));
        identity.EntityType = EntityType.User;
        identity.CollisionHeight = 0.45f;
        GameObject go = (GameObject)Instantiate(_userPlaceholder, identity.Position, Quaternion.identity);
        UserEntity player = go.GetComponent<UserEntity>();
        player.Status = status;
        player.Identity = identity;

        _players.Add(identity.Id, player);
        _objects.Add(identity.Id, player);

        go.GetComponent<NetworkTransformReceive>().enabled = true;

        go.transform.name = identity.Name;
        go.SetActive(true);

        go.transform.SetParent(_usersContainer.transform);
    }

    public void SpawnNpc(NetworkIdentity identity, NpcStatus status) {
        identity.SetPosY(GetGroundHeight(identity.Position));
        identity.EntityType = EntityTypeParser.ParseEntityType(identity.Type);

        string prefabName = identity.NpcClass.Split(".")[1].ToLower();
        GameObject go;
        if(identity.EntityType == EntityType.NPC) {
            go = Resources.Load<GameObject>(Path.Combine("Data/Animations/LineageNPCs/", prefabName, prefabName + "_prefab"));
            if(go == null) {
                go = _npcPlaceHolder;
            }
        } else {
            go = Resources.Load<GameObject>(Path.Combine("Data/Animations/LineageMonsters/", prefabName, prefabName + "_prefab"));
            if(go == null) {
                go = _monsterPlaceholder;
            }
            if(string.IsNullOrEmpty(identity.Title)) {
                identity.Title = "Lvl: " + status.Level;
            }
        }

        GameObject npcGo = Instantiate(go, identity.Position, Quaternion.identity);

        Entity npc;
        if (identity.EntityType == EntityType.NPC) {
            npcGo.transform.SetParent(_npcsContainer.transform);
            npc = npcGo.GetComponent<NpcEntity>();
        } else {
            npcGo.transform.SetParent(_monstersContainer.transform);
            npc = npcGo.GetComponent<MonsterEntity>();
        }

        npc.Status = status;
        npc.Identity = identity;

        _npcs.Add(identity.Id, npc);
        _objects.Add(identity.Id, npc);

        npcGo.transform.eulerAngles = new Vector3(npcGo.transform.eulerAngles.x, identity.Heading, npcGo.transform.eulerAngles.z);

        npcGo.transform.name = identity.Name;

        npcGo.SetActive(true);
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
            } catch(Exception) {
                Debug.LogWarning("Trying to update a null object");
                RemoveObject(id);
            }
        }
    }

    public void UpdateObjectRotation(int id, float angle) {
        Entity e;
        if(_objects.TryGetValue(id, out e)) {
            try {
                e.GetComponent<NetworkTransformReceive>().RotateTo(angle);
            } catch(Exception) {
                Debug.LogWarning("Trying to update a null object");
                RemoveObject(id);
            }
        }
    }

    public void UpdateObjectDestination(int id, Vector3 position, int speed, bool walking) {
        Entity e;
        if (_objects.TryGetValue(id, out e)) {
            try {
                if (speed != e.Status.Speed) {
                    e.UpdateSpeed(speed);
                }

                e.GetComponent<NetworkCharacterControllerReceive>().SetDestination(position);
                e.GetComponent<NetworkTransformReceive>().LookAt(position);
                e.OnStartMoving(walking);
                
            } catch (Exception) {
                Debug.LogWarning("Trying to update a null object");
                _objects.Remove(id);
            }

        }
    }

    public void UpdateObjectAnimation(int id, int animId, float value) {
        Entity e;
        if(_objects.TryGetValue(id, out e)) {
            try {
                Debug.Log($"Setting {id} anim {animId} at {value}");
                e.GetComponent<NetworkAnimationController>().SetAnimationProperty(animId, value);
            } catch(Exception) {
                Debug.LogWarning("Trying to update a null object");
                RemoveObject(id);
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
            } catch (Exception) {
                Debug.LogWarning("Trying to update a null object");
            }
        }

    }

    public void UpdateObjectMoveDirection(int id, int speed, Vector3 direction) {
        Entity e;
        if(_objects.TryGetValue(id, out e)) {
            try {
                if(speed != e.Status.Speed) {
                    e.UpdateSpeed(speed);
                }

                e.GetComponent<NetworkCharacterControllerReceive>().UpdateMoveDirection(direction);
            } catch(Exception) {
                Debug.LogWarning("Trying to update a null object");
                RemoveObject(id);
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
            } catch (Exception) {
                Debug.LogWarning("Trying to update a null object");
                RemoveObject(id);
            }
        }
    }

    public void EntityStopAutoAttacking(int id) {
        Entity e;
        if (_objects.TryGetValue(id, out e)) {
            try {
                WorldCombat.Instance.EntityStopAutoAttacking(e);
            } catch (Exception) {
                Debug.LogWarning("Trying to update a null object");
                RemoveObject(id);
            }
        }
    }
}
