using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;

public class World : MonoBehaviour
{
    private EventProcessor _eventProcessor;
    private WorldSpawner _worldSpawner;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask _entityMask;
    [SerializeField] private LayerMask _simpleEntityMask;
    [SerializeField] private LayerMask _entityClickAreaMask;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private LayerMask _clickThroughMask;
    [SerializeField] private LayerMask _groundMask;

    [SerializeField] private bool _offlineMode = false;

    public bool OfflineMode { get { return _offlineMode; } }
    public LayerMask GroundMask { get { return _groundMask; } }

    private static World _instance;
    public static World Instance { get { return _instance; } }

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
        _worldSpawner = GetComponent<WorldSpawner>();
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void Start()
    {
        UpdateMasks();
    }

    void UpdateMasks()
    {
        NameplatesManagerGame.Instance.SetMask(_entityMask);
        Geodata.Instance.ObstacleMask = _obstacleMask;
        ClickManager.Instance.SetMasks(_entityClickAreaMask, _clickThroughMask);
        CameraController.Instance.SetMask(_obstacleMask);
        TargetManager.Instance.SetMask(_simpleEntityMask);
    }

    public float GetGroundHeight(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 1.5f, Vector3.down, out hit, 3.5f, _groundMask))
        {
            return hit.point.y;
        }

        return pos.y;
    }

    public Task UpdateObjectPosition(int id, Vector3 position)
    {
        return _worldSpawner.ExecuteWithEntityAsync(id, e =>
        {
            ((NetworkEntityReferenceHolder)e.ReferenceHolder).NetworkTransformReceive.SetNewPosition(position);
        });
    }

    public Task AdjustObjectPositionAndRotation(int id, Vector3 position, int heading)
    {
        return _worldSpawner.ExecuteWithEntityAsync(id, e =>
        {

            e.Identity.Position = position;
            e.Identity.Heading = heading;

            float rotation = VectorUtils.ConvertRotToUnity(heading);
            if (id == GameClient.Instance.CurrentPlayerId)
            {
                PlayerTransformReceive.Instance.SetNewPosition(position);
                NetworkCharacterControllerShare.Instance.Heading = heading;
            }
            else
            {
                ((NetworkEntityReferenceHolder)e.ReferenceHolder).NetworkTransformReceive.SetFinalRotation(rotation);
                ((NetworkEntityReferenceHolder)e.ReferenceHolder).NetworkTransformReceive.SetNewPosition(position);
            }
        });
    }

    public Task NpcHtmlReceived(int objectId, string html, int itemId)
    {
        _eventProcessor.QueueEvent(() => NpcHtmlWindow.Instance.RefreshContent(objectId, html, itemId));
        return _worldSpawner.ExecuteWithEntityAsync(objectId, e =>
        {
            ((NetworkEntityReferenceHolder)e.ReferenceHolder).NetworkTransformReceive.LookAt(PlayerEntity.Instance.transform);
        });
    }

    public Task UpdateObjectRotation(int id, float angle)
    {
        return _worldSpawner.ExecuteWithEntityAsync(id, e =>
        {
            ((NetworkEntityReferenceHolder)e.ReferenceHolder).NetworkTransformReceive.SetFinalRotation(angle);
        });
    }

    public Task UpdateObjectDestination(int id, Vector3 currentPosition, Vector3 destination)
    {
        return _worldSpawner.ExecuteWithEntityAsync(id, e =>
        {
            e.Identity.Position = currentPosition;
            StartCoroutine(HandleUpdateDestination(e, currentPosition, destination));
        });
    }

    IEnumerator HandleUpdateDestination(Entity e, Vector3 currentPosition, Vector3 destination)
    {
        // Debug.LogWarning("Entity move to destination: " + destination);

        NetworkEntityReferenceHolder referenceHolder = (NetworkEntityReferenceHolder)e.ReferenceHolder;
        //sync current position with server
        referenceHolder.NetworkTransformReceive.ResumePositionSync();
        referenceHolder.NetworkTransformReceive.SetNewPosition(currentPosition);

        //wait for position to be updated
        yield return new WaitForFixedUpdate();

        //tell the entity to move to location
        referenceHolder.NetworkTransformReceive.PausePositionSync();

        if (e.Combat.AttackTarget == null)
        {
            referenceHolder.NetworkCharacterControllerReceive.SetDestination(destination, 0);

            //set the entity expected position to destination
            referenceHolder.NetworkTransformReceive.SetNewPosition(destination);
        }
        else
        {
            referenceHolder.NetworkCharacterControllerReceive.SetDestination(destination, WorldCombat.Instance.GetRealAttackRange(e, e.Combat.AttackTarget));
        }

        //look at destination
        referenceHolder.NetworkTransformReceive.LookAt(destination);
    }

    public Task UpdateObjectMoveDirection(int id, Vector3 position, Vector3 direction, float verticalVelocity, long timestamp)
    {

        return _worldSpawner.ExecuteWithEntityAsync(id, e =>
        {
            ((NetworkEntityReferenceHolder)e.ReferenceHolder).NetworkCharacterControllerReceive.UpdateMoveDirection(position, direction, verticalVelocity, timestamp);
        });
    }

    public Task ObjectStoppedMove(int id, Vector3 position, int heading)
    {
        return _worldSpawner.ExecuteWithEntityAsync(id, e =>
        {
            Debug.LogWarning($"[{e.transform.name}] ObjectStoppedMove");
            e.Identity.Position = position;
            e.Identity.Heading = heading;

            float rotation = VectorUtils.ConvertRotToUnity(heading);

            if (id == GameClient.Instance.CurrentPlayerId)
            {
                PlayerTransformReceive.Instance.SetNewPosition(position);
            }
            else if (e.Identity.EntityType == EntityType.User)
            {
                ((NetworkEntityReferenceHolder)e.ReferenceHolder).NetworkTransformReceive.SetNewPosition(position);
            }
            else
            {
                ((NetworkEntityReferenceHolder)e.ReferenceHolder).NetworkTransformReceive.SetFinalRotation(rotation);
                ((NetworkEntityReferenceHolder)e.ReferenceHolder).NetworkTransformReceive.SetNewPosition(position);
                ((NetworkEntityReferenceHolder)e.ReferenceHolder).NetworkCharacterControllerReceive.ResetDestination();
            }
        });
    }

    public Task ChangeWaitType(int owner, ChangeWaitTypePacket.WaitType moveType, Vector3 entityPosition)
    {
        return _worldSpawner.ExecuteWithEntityAsync(owner, e =>
        {
            if (owner != GameClient.Instance.CurrentPlayerId)
            {
                ((NetworkEntityReferenceHolder)e.ReferenceHolder).NetworkTransformReceive.SetNewPosition(entityPosition);

                //Verify position on server
                // NetworkTransformShare.Instance.SharePosition();
            }
            else
            {
                PlayerTransformReceive.Instance.SetNewPosition(entityPosition);
            }

            e.UpdateWaitType(moveType);
        });
    }

    public Task ChangeMoveType(int owner, bool running)
    {
        return _worldSpawner.ExecuteWithEntityAsync(owner, e =>
                {
                    e.UpdateMoveType(running);
                });
    }

    public Task EntityTeleported(int entityId, Vector3 teleportTo, bool loadingScreen)
    {
        return _worldSpawner.ExecuteWithEntityAsync(entityId, e =>
        {
            if (entityId != GameClient.Instance.CurrentPlayerId)
            {
                ((NetworkEntityReferenceHolder)e.ReferenceHolder).NetworkTransformReceive.SetNewPosition(teleportTo);
            }
            else
            {
                // if (loadingScreen)
                // {
                //     Debug.LogWarning("TODO: HANDLE TELEPORT LOADING SCREEN");
                // }
                // PlayerTransformReceive.Instance.SetNewPosition(teleportTo);
                // GameClient.Instance.ClientPacketHandler.NotifyAppearing();
                GameManager.Instance.NotifyEvent(GameEvent.TELEPORTING);
            }

            e.Identity.Position = teleportTo;
        });
    }

    public Task SocialActionReceived(int objectId, int action)
    {
        return _worldSpawner.ExecuteWithEntityAsync(objectId, e =>
        {
            if (action == SocialActionPacket.LEVELUP_ACTION)
            {
                Debug.Log("Entity level up!");
                WorldCombat.Instance.EntityCastSkill(e, 2122);
            }
            else
            {
                e.AnimationController.Emote(action);
            }
        });
    }

    public void DestroyWorld()
    {
        ProjectileManager.Instance.DestroyAllProjectiles();
        ParticleManager.Instance.DestroyAllParticles();
        WorldSpawner.Instance.DestroyEntities();
        WorldSpawner.Instance.ClearEntities();
    }
}
