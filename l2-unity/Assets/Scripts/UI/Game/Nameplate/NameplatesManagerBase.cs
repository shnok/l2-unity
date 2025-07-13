using System;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class NameplatesManagerBase : MonoBehaviour
{
    [SerializeField] protected float nameplateViewDistance = 50f;
    [SerializeField] private LayerMask entityMask;
    [SerializeField] private int updatesPerSecond = 200;
    [SerializeField] protected float nameplateHeightMultiplier = 1.95f;

    protected readonly ConcurrentDictionary<int, Nameplate> nameplates = new();
    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected Camera mainCamera;
    protected VisualElement rootElement;
    protected VisualTreeAsset nameplateTemplate;
    private float updateInterval;
    private float accumulatedTime;

    protected void Initialize()
    {
        updateInterval = 1.0f / updatesPerSecond;
        LoadNameplateTemplate();
    }

    private void LoadNameplateTemplate()
    {
        nameplateTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Game/Nameplate/Nameplate");
        if (nameplateTemplate == null)
        {
            Debug.LogError("Failed to load nameplate template.");
        }
    }

    private void Update()
    {
        if (!IsSystemReady()) return;

        accumulatedTime += Time.deltaTime;
        while (accumulatedTime >= updateInterval)
        {
            UpdateNameplateSystem();
            accumulatedTime -= updateInterval;
        }
    }

    protected virtual bool IsSystemReady()
    {
        return true;
    }

    protected virtual void ScanForEntities()
    {
        var entitiesInRange = Physics.SphereCastAll(
            playerTransform.position,
            nameplateViewDistance,
            transform.forward,
            0,
            entityMask
        );

        ProcessEntitiesInRange(entitiesInRange);
    }

    private void ProcessEntitiesInRange(RaycastHit[] entities)
    {
        foreach (var hit in entities)
        {
            if (hit.transform.TryGetComponent<Entity>(out var entity) &&
                CheckIfNotPlayer(entity) &&
                IsNameplateVisible(entity.transform))
            {
                nameplates.GetOrAdd(entity.Identity.Id, _ => CreateNameplate(entity));
            }
        }
    }

    protected virtual bool CheckIfNotPlayer(Entity entity)
    {
        return true;
    }

    protected bool ShouldCreateNameplateForEntity(ObjectData objectData)
    {
        return objectData != null &&
               objectData.ObjectTransform != null &&
               entityMask == (entityMask | (1 << objectData.ObjectLayer)) &&
               objectData.ObjectTransform.TryGetComponent<Entity>(out var entity) &&
               entity.Identity.Id != GameClient.Instance.CurrentPlayerId;
    }

    protected void ProcessNameplateVisibility()
    {
        var nameplateIds = nameplates.Keys.ToList();
        foreach (int id in nameplateIds)
        {
            if (nameplates.TryGetValue(id, out var nameplate))
            {
                nameplate.Visible = IsNameplateVisible(nameplate.Target);
                if (!nameplate.Visible)
                {
                    RemoveNameplate(id);
                }
            }
        }
    }

    public virtual void ClearNameplates()
    {
        foreach (int id in nameplates.Keys.ToList())
        {
            if (nameplates.TryGetValue(id, out var nameplate))
            {
                RemoveNameplate(id);
            }
        }
    }

    protected void UpdateNameplateSystem()
    {
        foreach (Nameplate nameplate in nameplates.Values)
        {
            if (nameplate.Target == null) continue;

            UpdateNameplatePosition(nameplate);
            UpdateNameplateStyle(nameplate);
        }
    }

    protected virtual void UpdateNameplateStyle(Nameplate nameplate)
    {
        nameplate.NameplateOffsetHeight = nameplate.Entity.IsDead ?
            nameplate.Entity.Appearance.CollisionHeight * 0.85f :
            nameplate.Entity.Appearance.CollisionHeight * nameplateHeightMultiplier * (nameplate.Entity.IsSitting ? 0.70f : 1f);

        nameplate.ManageColors();
    }

    protected void UpdateNameplatePosition(Nameplate nameplate)
    {
        try
        {
            var worldPosition = nameplate.Target.position + Vector3.up * nameplate.NameplateOffsetHeight;
            var screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

            nameplate.NameplateEle.style.left = screenPosition.x - nameplate.NameplateEle.resolvedStyle.width / 2f;
            nameplate.NameplateEle.style.top = Screen.height - screenPosition.y - nameplate.NameplateEle.resolvedStyle.height;
        }
        catch (Exception ex) when (ex is NullReferenceException || ex is MissingReferenceException)
        {
            // Handle or log exception if needed
        }
    }

    protected virtual bool IsNameplateVisible(Transform target)
    {
        if (target == null) return false;

        var isTooFar = Vector3.Distance(playerTransform.position, target.position) > nameplateViewDistance;

        return !isTooFar;
    }

    // Factory methods for creating nameplates
    protected virtual Nameplate CreateNameplate(Entity entity)
    {
        VisualElement element = nameplateTemplate.Instantiate()[0];
        Nameplate nameplate = new Nameplate(
            element,
            element.Q<Label>("EntityName"),
            element.Q<Label>("EntityTitle"),
            entity
        );
        rootElement.Add(element);
        return nameplate;
    }

    public void RemoveNameplate(int id)
    {
        if (nameplates.TryRemove(id, out Nameplate removed))
        {
            rootElement.Remove(removed.NameplateEle);
        }
    }

    public void SetMask(LayerMask mask) => entityMask = mask;
}
