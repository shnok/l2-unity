using System;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class NameplatesManager3D : MonoBehaviour
{
    [SerializeField] protected float _nameplateViewDistance = 50f;
    [SerializeField] protected float _referenceDistance = 2f;
    [SerializeField] private LayerMask _entityMask;
    [SerializeField] private LayerMask _clickThroughMask;
    [SerializeField] private Entity _entityAimedAt;
    [SerializeField] private RaycastHit[] _entitiesInRange;
    [SerializeField] protected float _nameplateHeightMultiplier = 1.95f;

    protected readonly ConcurrentDictionary<int, Nameplate3D> nameplates = new();

    [SerializeField] protected Camera mainCamera;
    public Camera MainCamera { get => mainCamera; }
    protected VisualElement rootElement;
    protected VisualTreeAsset nameplateTemplate;

    public static NameplatesManager3D Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (!IsSystemReady()) return;
    }

    private void FixedUpdate()
    {
        if (!IsSystemReady()) return;

        ScanForEntities();
        ProcessNameplateVisibility();
        // CheckForAimedEntity();
    }

    private void LateUpdate()
    {
        if (!IsSystemReady()) return;
    }

    protected virtual bool IsSystemReady()
    {
        if (GameManager.Instance == null)
        {
            return false;
        }

        if (GameManager.Instance.State != GameState.IN_GAME)
        {
            return false;
        }

        return true;
    }

    protected virtual void ScanForEntities()
    {
        if (PlayerEntity.Instance == null)
        {
            Debug.LogWarning("PlayerEntity is null");
            return;
        }

        _entitiesInRange = Physics.SphereCastAll(
            PlayerEntity.Instance.transform.position,
            _nameplateViewDistance,
            transform.forward,
            0,
            _entityMask
        );

        ProcessEntitiesInRange();
    }

    public void ClearNameplates()
    {
        if (nameplates == null)
        {
            return;
        }

        var nameplateIds = nameplates.Keys.ToList();

        foreach (int id in nameplateIds)
        {
            RemoveAndHideNameplate(id);
        }
    }

    // private void CheckForAimedEntity()
    // {
    //     if (PlayerEntity.Instance == null)
    //     {
    //         return;
    //     }

    //     Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    //     RaycastHit hit;

    //     if (Physics.Raycast(ray, out hit, 1000f, ~_clickThroughMask))
    //     {
    //         int hitLayer = hit.collider.gameObject.layer;
    //         if (_entityMask != (_entityMask | (1 << hitLayer)))
    //         {
    //             ClearAimedAtEntity();
    //             return;
    //         }

    //         if (hit.collider.transform.parent.TryGetComponent(out Entity entity))
    //         {
    //             Nameplate3D nameplate = ShowNameplate(entity.ReferenceHolder.Nameplate);
    //             if (nameplate != null)
    //             {
    //                 nameplates.GetOrAdd(entity.Id, _ => nameplate);
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogWarning("Can't get Entity object from target collider.");
    //         }
    //     }
    //     else
    //     {
    //         ClearAimedAtEntity();
    //     }
    // }

    // private void ClearAimedAtEntity()
    // {
    //     if (_entityAimedAt != null)
    //     {
    //         RemoveAndHideNameplate(_entityAimedAt.Id);
    //         _entityAimedAt = null;
    //     }
    // }

    // Check entities around and show their nameplates if needed
    private void ProcessEntitiesInRange()
    {
        foreach (var hit in _entitiesInRange)
        {
            if (hit.transform.TryGetComponent<EntityReferenceHolder>(out var referenceHolder) &&
                CheckIfNotPlayer(referenceHolder) &&
                referenceHolder.Entity != _entityAimedAt &&
                IsNameplateVisible(referenceHolder.transform))
            {

                Nameplate3D nameplate = ShowNameplate(referenceHolder.Nameplate);
                if (nameplate != null)
                {
                    nameplates.TryAdd(referenceHolder.Entity.Identity.Id, nameplate);
                }
            }
        }
    }

    private Nameplate3D ShowNameplate(Nameplate3D nameplate)
    {
        if (nameplate == null)
        {
            return null;
        }
        nameplate.gameObject.SetActive(true);

        return nameplate;
    }


    // Process current nameplate list, remove or delete nameplates if needed
    protected void ProcessNameplateVisibility()
    {
        var nameplateIds = nameplates.Keys.ToList();
        foreach (int id in nameplateIds)
        {
            if (nameplates.TryGetValue(id, out var nameplate))
            {

                if (nameplate == null || nameplate.transform == null)
                {
                    RemoveAndHideNameplate(id);
                    continue;
                }

                nameplate.Visible = IsNameplateVisible(nameplate.Target.transform);
                if (!nameplate.Visible)
                {
                    RemoveAndHideNameplate(id);
                }
            }
        }
    }

    public void RemoveAndHideNameplate(int id)
    {
        if (nameplates.TryRemove(id, out Nameplate3D removed))
        {
            if (removed != null && removed.gameObject != null)
                removed.gameObject.SetActive(false);
        }
    }

    public Nameplate3D CreateOrUpdateNameplate(Entity entity)
    {
        if (entity.ReferenceHolder.Nameplate == null)
        {
            Debug.LogWarning($"[ID:{entity.Identity.Id}] Creating nameplate.");
            return CreateNameplate(entity);
        }
        else
        {
            return UpdateNameplate(entity);
        }
    }

    private Nameplate3D CreateNameplate(Entity entity)
    {
        Debug.LogWarning($"Create nameplate for entity with id: {entity.Identity.Id}.");

        GameObject nameplatePrefab = Resources.Load<GameObject>("Data/UI/_Elements/Game/WorldSpaceUI/Nameplate/Nameplate");
        GameObject nameplateObject = Instantiate(nameplatePrefab, entity.transform);
        Nameplate3D nameplate = nameplateObject.GetComponent<Nameplate3D>();
        nameplates.TryAdd(entity.Identity.Id, nameplate);

        UpdateNameplate(nameplate, entity);

        nameplateObject.SetActive(false);

        // Debug.Log($"[{entity.name}] Create Nameplate");

        return nameplate;
    }

    private Nameplate3D UpdateNameplate(Entity entity)
    {
        Debug.LogWarning($"Update nameplate for entity with id: {entity.Identity.Id}.");

        if (!nameplates.ContainsKey(entity.Identity.Id))
        {
            nameplates.TryAdd(entity.Identity.Id, entity.ReferenceHolder.Nameplate);
        }

        UpdateNameplate(entity.ReferenceHolder.Nameplate, entity);

        return entity.ReferenceHolder.Nameplate;
    }

    private void UpdateNameplate(Nameplate3D nameplate, Entity entity)
    {
        nameplate.transform.SetLocalPositionAndRotation(
            Vector3.up * entity.Appearance.CollisionHeight * _nameplateHeightMultiplier,
            Quaternion.identity);

        nameplate.SetTarget(entity);
        nameplate.UpdateStatus(entity.Status, entity.Stats);
    }

    protected virtual bool CheckIfNotPlayer(EntityReferenceHolder entity)
    {
        return true;
    }

    protected virtual bool IsNameplateVisible(Transform entityTransform)
    {
        if (PlayerEntity.Instance == null)
        {
            return false;
        }

        if (_entityAimedAt != null && _entityAimedAt.transform == entityTransform)
        {
            return true;
        }

        if (Vector3.Distance(entityTransform.position, PlayerEntity.Instance.transform.position) > _nameplateViewDistance)
        {
            return false;
        }

        return true;
    }
}
