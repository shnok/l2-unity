using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class World : MonoBehaviour {
    public GameObject player;
    public GameObject playerPrefab;
    public GameObject userPrefab;
    public GameObject npcPrefab;
    public Dictionary<int, Entity> players = new Dictionary<int, Entity>();
    public Dictionary<int, Entity> npcs = new Dictionary<int, Entity>();
    public Dictionary<int, Entity> objects = new Dictionary<int, Entity>();

    [Header("Layer Masks")]
    public LayerMask entityMask;
    public LayerMask obstacleMask;
    public LayerMask clickThroughMask;
    public LayerMask groundMask;

    public bool offlineMode = false;

    public static World instance;
    public static World GetInstance() {
        return instance;
    }

    void Awake() {
        instance = this;

        playerPrefab = Resources.Load<GameObject>("Prefab/Player");
        userPrefab = Resources.Load<GameObject>("Prefab/User");
        npcPrefab = Resources.Load<GameObject>("Prefab/Npc");
    }

    void Start() {
        UpdateMasks();
    }

    void UpdateMasks() {
        NameplatesManager.GetInstance().SetMask(entityMask);
        Geodata.GetInstance().SetMask(obstacleMask);
        ClickManager.GetInstance().SetMasks(entityMask, clickThroughMask);
        CameraController.GetInstance().SetMask(obstacleMask);
    }

    public void RemoveObject(int id) {
        Entity transform;
        if(objects.TryGetValue(id, out transform)) {
            players.Remove(id);
            npcs.Remove(id);
            objects.Remove(id);

            Object.Destroy(transform.gameObject);
        }
    }

    public void SpawnPlayerOfflineMode() {
        if(World.GetInstance().offlineMode) {
            PlayerEntity entity = playerPrefab.GetComponent<PlayerEntity>();
            entity.Identity.Position = playerPrefab.transform.position;
            SpawnPlayer(entity.Identity, entity.Status);
        }
    }

    public void SpawnPlayer(NetworkIdentity identity, PlayerStatus status) {
        identity.SetPosY(GetGroundHeight(identity.Position));
        identity.EntityType = EntityType.Player;
        identity.CollisionHeight = 0.45f;
        GameObject go = (GameObject)Instantiate(playerPrefab, identity.Position, Quaternion.identity);
        PlayerEntity player = go.GetComponent<PlayerEntity>();
        player.Status = status;
        player.Identity = identity;

        players.Add(identity.Id, player);
        objects.Add(identity.Id, player);

        go.GetComponent<PlayerController>().enabled = true;

        if(!World.GetInstance().offlineMode) {
            go.GetComponent<NetworkTransformShare>().enabled = true;
        }
          
        go.transform.name = identity.Name;
        go.SetActive(true);

        CameraController.GetInstance().SetTarget(go);
        CameraController.GetInstance().enabled = true;

        ChatWindow.GetInstance().ReceiveChatMessage(new MessageLoggedIn(identity.Name));
    }

    public void SpawnUser(NetworkIdentity identity, PlayerStatus status) {
        identity.SetPosY(GetGroundHeight(identity.Position));
        identity.EntityType = EntityType.User;
        identity.CollisionHeight = 0.45f;
        GameObject go = (GameObject)Instantiate(userPrefab, identity.Position, Quaternion.identity);
        UserEntity player = go.GetComponent<UserEntity>();
        player.Status = status;
        player.Identity = identity;

        players.Add(identity.Id, player);
        objects.Add(identity.Id, player);

        go.GetComponent<NetworkTransformReceive>().enabled = true;

        go.transform.name = identity.Name;
        go.SetActive(true);
    }

    public void SpawnNpc(NetworkIdentity identity, NpcStatus status) {
        identity.SetPosY(GetGroundHeight(identity.Position));
        identity.EntityType = EntityTypeParser.ParseEntityType(identity.Type);

        string prefabName = identity.NpcClass.Split(".")[1].ToLower();
        GameObject go;
        if(identity.EntityType == EntityType.NPC) {
            go = Resources.Load<GameObject>(Path.Combine("Data/Animations/LineageNPCs/", prefabName, prefabName + "_prefab"));
        } else {
            go = Resources.Load<GameObject>(Path.Combine("Data/Animations/LineageMonsters/", prefabName, prefabName + "_prefab"));
            if(string.IsNullOrEmpty(identity.Title)) {
                identity.Title = "Lvl: " + status.Level;
            }
        }

        if(go == null) {
            go = npcPrefab;
        }

        GameObject npcGo = Instantiate(go, identity.Position, Quaternion.identity); 

        NpcEntity npc = npcGo.GetComponent<NpcEntity>();
        npc.Status = status;
        npc.Identity = identity;

        npcs.Add(identity.Id, npc);
        objects.Add(identity.Id, npc);

        npcGo.transform.eulerAngles = new Vector3(npcGo.transform.eulerAngles.x, identity.Heading, npcGo.transform.eulerAngles.z);

        npcGo.SetActive(true);
    }

    public float GetGroundHeight(Vector3 pos) {
        RaycastHit hit;
        if(Physics.Raycast(pos + Vector3.up * 3f, Vector3.down, out hit, 3.5f, groundMask)) {
            return hit.point.y;
        }

        return pos.y;
    }

    public void UpdateObjectPosition(int id, Vector3 position) {
        Entity e;
        if(objects.TryGetValue(id, out e)) {
            e.GetComponent<NetworkTransformReceive>().SetNewPosition(position);
        }
    }

    public void UpdateObjectDestination(int id, Vector3 position) {
        Entity e;
        if(objects.TryGetValue(id, out e)) {
            e.GetComponent<NetworkCharacterControllerReceive>().SetDestination(position);
            e.GetComponent<NetworkTransformReceive>().LookAt(position);
        }
    }

    public void UpdateObjectRotation(int id, float angle) {
        Entity e;
        if(objects.TryGetValue(id, out e)) {
            e.GetComponent<NetworkTransformReceive>().RotateTo(angle);
        }
    }

    public void UpdateObjectAnimation(int id, int animId, float value) {
        Entity e;
        if(objects.TryGetValue(id, out e)) {
            e.GetComponent<NetworkAnimationReceive>().SetAnimationProperty(animId, value);
        }
    }

    public void InflictDamageTo(int sender, int target, byte attackId, int value) {
        Entity senderEntity;
        Entity targetEntity;
        if(objects.TryGetValue(sender, out senderEntity)) {
            if(objects.TryGetValue(target, out targetEntity)) {
                //networkTransform.GetComponentInParent<Entity>().ApplyDamage(sender, attackId, value);
                WorldCombat.GetInstance().ApplyDamage(senderEntity.transform, targetEntity.transform, attackId, value);
            }
        }
    }

    public void UpdateObjectMoveDirection(int id, float speed, Vector3 direction) {
        Entity e;
        if(objects.TryGetValue(id, out e)) {
            e.GetComponent<NetworkCharacterControllerReceive>().UpdateMoveDirection(speed, direction);
        }
    }
}
