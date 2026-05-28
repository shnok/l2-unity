using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorldSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    [SerializeField] private GameObject _monstersContainer;
    [SerializeField] private GameObject _npcsContainer;
    [SerializeField] private GameObject _usersContainer;

    private EventProcessor _eventProcessor;

    private List<int> _idBag = new List<int>();
    private ConcurrentDictionary<int, Entity> _players = new ConcurrentDictionary<int, Entity>();
    private ConcurrentDictionary<int, Entity> _npcs = new ConcurrentDictionary<int, Entity>();
    private ConcurrentDictionary<int, Entity> _objects = new ConcurrentDictionary<int, Entity>();

    private NpcSpawner _npcSpawner;
    private PlayerSpawner _playerSpawner;
    private UserSpawner _userSpawner;

    private static WorldSpawner _instance;
    public static WorldSpawner Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }

        _eventProcessor = EventProcessor.Instance;
        _npcsContainer = GameObject.Find("Npcs");
        _monstersContainer = GameObject.Find("Monsters");
        _usersContainer = GameObject.Find("Users");

        _playerSpawner = new PlayerSpawner(_eventProcessor);
        _npcSpawner = new NpcSpawner(_eventProcessor, _npcsContainer.transform, _monstersContainer.transform);
        _userSpawner = new UserSpawner(_eventProcessor, _usersContainer.transform);
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void DestroyEntities()
    {
        foreach (Entity entity in _objects.Values)
        {
            if (entity != null && entity.gameObject != null)
            {
                Destroy(entity.gameObject);
            }
        }
    }

    public void ClearEntities()
    {
        _objects.Clear();
        _players.Clear();
        _npcs.Clear();
        _idBag.Clear();
    }

    public bool AddObject(int id, Entity entity)
    {
        if (!_objects.TryAdd(id, entity))
        {
            Debug.LogError($"Cant add npc with ID {id} in Objects.");
            return false;
        }

        return true;
    }

    public bool AddNpc(int id, Entity entity)
    {
        if (!_npcs.TryAdd(id, entity))
        {
            Debug.LogError($"Cant add npc with ID {id} in Npcs.");
            return false;
        }

        return true;
    }

    public bool AddPlayer(int id, Entity entity)
    {
        if (!_players.TryAdd(id, entity))
        {
            Debug.LogError($"Cant add npc with ID {id} in Players.");
            return false;
        }

        return true;
    }

    public Task RemoveObject(int id)
    {
        if (IsEntityPresent(id, true))
        {
            return ExecuteWithEntityAsync(id, e =>
            {
                _players.TryRemove(id, out Entity removed);
                _npcs.TryRemove(id, out Entity removed2);
                _objects.TryRemove(id, out Entity removed3);

                Debug.Log("Gameobject destroyed : " + e.gameObject.name);

                NameplatesManagerGame.Instance.RemoveNameplate(id);

                Destroy(e.gameObject);
            });
        }
        else
        {
            return null;
        }
    }

    // Execute action after entity is loaded
    public async Task ExecuteWithEntityAsync(int id, Action<Entity> action)
    {
        // Debug.Log($"ExecuteWithEntityAsync - ID: {id}  Action: {action}");
        if (id == GameClient.Instance.CurrentPlayerId)
        {
            _eventProcessor.QueueEvent(() => action(PlayerEntity.Instance));
            return;
        }

        var entity = await GetEntityAsync(id);
        if (entity != null)
        {
            try
            {
                _eventProcessor.QueueEvent(() => action(entity));
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Operation failed - Target {id} - Error {ex.Message}");
            }
        }
    }

    // Execute action after 2 entities are loaded
    public async Task ExecuteWithEntitiesAsync(int id1, int id2, Action<Entity, Entity> action)
    {
        if (id1 == id2)
        {
            // Load the entity once if the IDs are the same
            var entity = await GetEntityAsync(id1);
            if (entity != null)
            {
                try
                {
                    _eventProcessor.QueueEvent(() => action(entity, entity));
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Operation failed - Target {id1} - Error {ex.Message}");
                }
            }
            return;
        }

        // Load both entities in parallel if IDs are different
        var entity1Task = GetEntityAsync(id1);
        var entity2Task = GetEntityAsync(id2);

        await Task.WhenAll(entity1Task, entity2Task);

        var entity1 = await entity1Task;
        var entity2 = await entity2Task;

        if (entity1 != null && entity2 != null)
        {
            try
            {
                _eventProcessor.QueueEvent(() => action(entity1, entity2));
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Operation failed - Target {id1} or {id2} - Error {ex.Message}");
            }
        }
    }

    // Wait for entity to be fully loaded
    public async Task<Entity> GetEntityAsync(int id)
    {
        Entity entity;
        if (id == GameClient.Instance.CurrentPlayerId)
        {
            entity = PlayerEntity.Instance;
            if (entity == null)
            {
                Debug.LogError("Player entity is null");
            }
        }
        else if (!_objects.TryGetValue(id, out entity))
        {
            Debug.LogWarning($"GetEntityAsync - Entity {id} not found, retrying...");
        }

        if (entity == null)
        {
            await Task.Delay(300);
            if (!_objects.TryGetValue(id, out entity))
            {
                Debug.LogWarning($"GetEntityAsync - Entity {id} not found after retry");
                return null;
            }
            else
            {
                // Debug.LogWarning($"GetEntityAsync - Entity {id} found after retry");
            }
        }

        return entity;
    }

    public bool IsEntityPresent(int id)
    {
        return IsEntityPresent(id, false);
    }

    private bool IsEntityPresent(int id, bool remove)
    {
        lock (_idBag)
        {
            if (_idBag.Contains(id))
            {
                if (remove)
                {
                    _idBag.Remove(id);
                }
                return true;
            }
            else
            {
                _idBag.Add(id);
                return false;
            }
        }
    }

    public void OnReceivePlayerInfo(NetworkIdentity identity, PlayerStatus status, PlayerStats stats, PlayerAppearance appearance, EntityActionInfo actionInfo)
    {
        _playerSpawner.OnReceiveEntityInfo(identity, status, stats, appearance, actionInfo);
    }

    public void OnReceiveNpcInfo(NetworkIdentity identity, NpcStatus status, Stats stats, Appearance appearance, EntityActionInfo actionInfo)
    {
        _npcSpawner.OnReceiveEntityInfo(identity, status, stats, appearance, actionInfo);
    }

    public void OnReceiveUserInfo(NetworkIdentity identity, PlayerStatus status, Stats stats, PlayerAppearance appearance, EntityActionInfo actionInfo)
    {
        _userSpawner.OnReceiveEntityInfo(identity, status, stats, appearance, actionInfo);
    }
}
