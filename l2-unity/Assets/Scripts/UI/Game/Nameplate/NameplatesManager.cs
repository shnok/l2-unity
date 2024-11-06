using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class NameplatesManager : MonoBehaviour
{
    // private VisualElement _rootElement;
    // private VisualTreeAsset _nameplateTemplate;
    // private readonly ConcurrentDictionary<int, Nameplate> _nameplates = new ConcurrentDictionary<int, Nameplate>();
    // private Transform _playerTransform;
    // private PlayerNameplate _currentPlayerNameplate;

    // [SerializeField] private float _nameplateViewDistance = 50f;
    // [SerializeField] private LayerMask _entityMask;
    // [SerializeField] public RaycastHit[] _entitiesInRange;
    // private Camera _mainCamera;

    // private static NameplatesManager _instance;
    // public static NameplatesManager Instance { get { return _instance; } }

    // private void Awake()
    // {
    //     if (_instance == null)
    //     {
    //         _instance = this;
    //     }
    //     else
    //     {
    //         Destroy(this);
    //     }
    // }

    // private void OnDestroy()
    // {
    //     _nameplates.Clear();
    //     _instance = null;
    // }

    // void Start()
    // {
    //     if (_nameplateTemplate == null)
    //     {
    //         _nameplateTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Game/Nameplate");
    //     }
    //     if (_nameplateTemplate == null)
    //     {
    //         Debug.LogError("Could not load chat window template.");
    //     }

    //     _mainCamera = CameraController.Instance.GetComponent<Camera>();
    // }

    // public void SetMask(LayerMask mask)
    // {
    //     _entityMask = mask;
    // }

    // private const int kUpdatesPerSecond = 200;
    // private const float kUpdateInterval = 1.0f / kUpdatesPerSecond; // how many seconds pass before an update should happen
    // private float _accumulation = 0.0f; // stores time elapsed
    // private void Update()
    // {
    //     // add to the accumulator
    //     _accumulation += Time.deltaTime;

    //     // while enough time has passed for an update, call our code we want executed 200 times per second.
    //     while (_accumulation >= kUpdateInterval)
    //     {
    //         UpdateNameplates();
    //         _accumulation -= kUpdateInterval;
    //     }
    // }

    // private void FixedUpdate()
    // {
    //     if (!L2GameUI.Instance.UILoaded)
    //     {
    //         return;
    //     }

    //     if (_playerTransform == null)
    //     {
    //         if (PlayerEntity.Instance != null && PlayerEntity.Instance.transform != null)
    //         {
    //             _playerTransform = PlayerEntity.Instance.transform;
    //         }
    //         else
    //         {
    //             return;
    //         }
    //     }

    //     if (_rootElement == null)
    //     {
    //         _rootElement = L2GameUI.Instance.RootElement.Q<VisualElement>("NameplatesContainer");
    //         return;
    //     }

    //     _entitiesInRange = Physics.SphereCastAll(_playerTransform.position, _nameplateViewDistance, transform.forward, 0, _entityMask);
    //     CreateNameplateForEntities();
    //     CheckNameplateVisibility();
    //     CheckMouseOver();
    //     CheckTarget();
    //     UpdateGauge();
    // }

    // public void StartCasting(SetupGaugePacket.GaugeColor color, int durationMs)
    // {
    //     _currentPlayerNameplate.ShowGauge(color, Time.time, durationMs);
    // }

    // private void UpdateGauge()
    // {
    //     if (_currentPlayerNameplate.GaugeEndTime - Time.time > 0)
    //     {
    //         _currentPlayerNameplate.UpdateGauge(Time.time);
    //     }
    //     else
    //     {
    //         _currentPlayerNameplate.HideGauge();
    //     }
    // }

    // private void CheckMouseOver()
    // {
    //     ObjectData hoverObjectData = ClickManager.Instance.HoverObjectData;
    //     if (hoverObjectData != null)
    //     {
    //         if (_entityMask == (_entityMask | (1 << hoverObjectData.ObjectLayer)))
    //         {
    //             if (hoverObjectData.ObjectTransform == null)
    //             {
    //                 return;
    //             }

    //             Entity e = hoverObjectData.ObjectTransform.GetComponent<Entity>();
    //             if (e != null && e.Identity.Id != GameClient.Instance.CurrentPlayerId)
    //             {
    //                 if (!_nameplates.ContainsKey(e.Identity.Id))
    //                 {
    //                     CreateNameplate(e);
    //                 }
    //             }
    //         }
    //     }
    // }

    // private void CheckTarget()
    // {
    //     if (!TargetManager.Instance.HasTarget())
    //     {
    //         return;
    //     }

    //     Entity e = TargetManager.Instance.Target.Data.ObjectTransform.GetComponent<Entity>();

    //     if (e != null && e.Identity.Id != GameClient.Instance.CurrentPlayerId)
    //     {
    //         if (!_nameplates.ContainsKey(e.Identity.Id))
    //         {
    //             CreateNameplate(e);
    //         }
    //     }
    // }

    // private void CreateNameplateForEntities()
    // {
    //     foreach (RaycastHit hit in _entitiesInRange)
    //     {
    //         Entity objectEntity = hit.transform.GetComponent<Entity>();
    //         if (objectEntity != null)
    //         {
    //             int objectId = objectEntity.Identity.Id;

    //             if (objectId == GameClient.Instance.CurrentPlayerId)
    //             {
    //                 continue;
    //             }

    //             if (!_nameplates.ContainsKey(objectId))
    //             {
    //                 if (!IsNameplateVisible(objectEntity.transform))
    //                 {
    //                     continue;
    //                 }

    //                 Nameplate nameplate = CreateNameplate(objectEntity);

    //                 _nameplates.TryAdd(objectEntity.Identity.Id, nameplate);
    //             }
    //         }
    //     }

    //     if (_currentPlayerNameplate == null)
    //     {
    //         _currentPlayerNameplate = CreatePlayerNameplate(PlayerEntity.Instance);
    //     }
    // }

    // private Nameplate CreateNameplate(Entity entity)
    // {
    //     VisualElement visualElement = _nameplateTemplate.Instantiate()[0];

    //     Nameplate nameplate = new Nameplate(
    //         visualElement,
    //         visualElement.Q<Label>("EntityName"),
    //         visualElement.Q<Label>("EntityTitle"),
    //         entity
    //         );

    //     _rootElement.Add(visualElement);

    //     return nameplate;
    // }

    // private PlayerNameplate CreatePlayerNameplate(Entity entity)
    // {
    //     VisualElement visualElement = _nameplateTemplate.Instantiate()[0];

    //     PlayerNameplate nameplate = new PlayerNameplate(
    //         visualElement,
    //         visualElement.Q<Label>("EntityName"),
    //         visualElement.Q<Label>("EntityTitle"),
    //         entity
    //         );

    //     _rootElement.Add(visualElement);

    //     return nameplate;
    // }

    // private void CheckNameplateVisibility()
    // {
    //     foreach (var nameplateId in _nameplates.Keys)
    //     {
    //         if (_nameplates.TryGetValue(nameplateId, out Nameplate nameplate))
    //         {
    //             nameplate.Visible = IsNameplateVisible(nameplate.Target);
    //             if (!nameplate.Visible)
    //             {
    //                 RemoveNameplate(nameplateId);
    //             }
    //         }
    //     }

    //     _currentPlayerNameplate.Visible = IsNameplateVisible(_currentPlayerNameplate.Target);

    //     if (!_currentPlayerNameplate.Visible)
    //     {
    //         _currentPlayerNameplate.Hide();
    //     }
    //     else
    //     {
    //         _currentPlayerNameplate.Show();
    //     }
    // }

    // private void UpdateNameplates()
    // {
    //     foreach (var nameplateId in _nameplates.Keys)
    //     {
    //         if (_nameplates.TryGetValue(nameplateId, out Nameplate nameplate))
    //         {

    //             UpdateNameplatePosition(nameplate);
    //             UpdateNameplateStyle(nameplate);

    //         }
    //     }

    //     UpdateNameplatePosition(_currentPlayerNameplate);
    //     UpdateNameplateStyle(_currentPlayerNameplate);

    //     if (_currentPlayerNameplate.Visible)
    //     {
    //         _currentPlayerNameplate.Show();
    //     }
    //     else
    //     {
    //         _currentPlayerNameplate.Hide();
    //     }
    // }

    // public void RemoveNameplate(int id)
    // {
    //     if (_nameplates.TryRemove(id, out var removed))
    //     {
    //         _rootElement.Remove(removed.NameplateEle);
    //     }
    // }

    // private void UpdateNameplateStyle(Nameplate nameplate)
    // {
    //     nameplate.NameplateOffsetHeight = nameplate.Entity.IsDead ? nameplate.Entity.Appearance.CollisionHeight : nameplate.Entity.Appearance.CollisionHeight * 2.1f;

    //     if (TargetManager.Instance.HasTarget() && TargetManager.Instance.Target.Data.ObjectTransform == nameplate.Target)
    //     {
    //         if (TargetManager.Instance.AttackTarget == TargetManager.Instance.Target
    //         && TargetManager.Instance.AttackTarget.Identity.EntityType != EntityType.NPC
    //         && !TargetManager.Instance.AttackTarget.Status.IsDead)
    //         {
    //             nameplate.SetStyle("target-bubble-attack");
    //         }
    //         else
    //         {
    //             nameplate.SetStyle("target-bubble-target");
    //             nameplate.RemoveStyle("target-bubble-attack");
    //         }
    //         return;
    //     }
    //     else
    //     {
    //         nameplate.RemoveStyle("target-bubble-attack");
    //         nameplate.RemoveStyle("target-bubble-target");
    //     }

    //     if (ClickManager.Instance.HoverObjectData != null && ClickManager.Instance.HoverObjectData.ObjectTransform == nameplate.Target)
    //     {
    //         nameplate.SetStyle("target-bubble-hover");
    //     }
    //     else
    //     {
    //         nameplate.RemoveStyle("target-bubble-hover");
    //     }
    // }

    // private void UpdateNameplatePosition(Nameplate nameplate)
    // {
    //     try
    //     {
    //         Vector2 nameplatePos = _mainCamera.WorldToScreenPoint(nameplate.Target.position + Vector3.up * nameplate.NameplateOffsetHeight);
    //         nameplate.NameplateEle.style.left = nameplatePos.x - nameplate.NameplateEle.resolvedStyle.width / 2f;
    //         nameplate.NameplateEle.style.top = Screen.height - nameplatePos.y - nameplate.NameplateEle.resolvedStyle.height;
    //     }
    //     catch (NullReferenceException) { }
    //     catch (MissingReferenceException) { }
    // }

    // private bool IsNameplateVisible(Transform target)
    // {
    //     if (target == null)
    //     {
    //         return false;
    //     }

    //     bool isHover = ClickManager.Instance.HoverObjectData != null && ClickManager.Instance.HoverObjectData.ObjectTransform == target;
    //     if (isHover)
    //     {
    //         return true;
    //     }

    //     bool isTarget = TargetManager.Instance.HasTarget() && TargetManager.Instance.Target.Data.ObjectTransform == target;
    //     bool isTooFar = Vector3.Distance(_playerTransform.position, target.position) > _nameplateViewDistance;
    //     if (isTooFar && !isTarget)
    //     {
    //         return false;
    //     }

    //     return CameraController.Instance.IsObjectVisible(target);
    // }
    [SerializeField] private float nameplateViewDistance = 50f;
    [SerializeField] private LayerMask entityMask;
    [SerializeField] private int updatesPerSecond = 200;
    [SerializeField] private float nameplateHeightMultiplier = 1.95f;

    private readonly ConcurrentDictionary<int, Nameplate> nameplates = new();
    private VisualElement rootElement;
    private VisualTreeAsset nameplateTemplate;
    private Transform playerTransform;
    private PlayerNameplate playerNameplate;
    private Camera mainCamera;
    private float updateInterval;
    private float accumulatedTime;

    private static NameplatesManager instance;
    public static NameplatesManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Initialize();
    }

    private void Initialize()
    {
        updateInterval = 1.0f / updatesPerSecond;
        LoadNameplateTemplate();
    }

    private void LoadNameplateTemplate()
    {
        nameplateTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Game/Nameplate");
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

    private void FixedUpdate()
    {
        if (!IsSystemReady()) return;

        ScanForEntities();
        ProcessNameplateVisibility();
        HandlePlayerNameplate();
    }

    private bool IsSystemReady()
    {
        if (!L2GameUI.Instance.UILoaded) return false;

        if (mainCamera == null)
        {
            if (CameraController.Instance == null)
            {
                Debug.LogWarning("CameraController instance is null");
                return false;
            }

            var cameraComponent = CameraController.Instance.GetComponent<Camera>();
            if (cameraComponent == null)
            {
                Debug.LogWarning("Camera component not found on CameraController");
                return false;
            }

            mainCamera = cameraComponent;
        }

        if (rootElement == null)
        {
            rootElement = L2GameUI.Instance.RootElement.Q<VisualElement>("NameplatesContainer");
            return false;
        }

        if (playerTransform == null && PlayerEntity.Instance?.transform != null)
        {
            playerTransform = PlayerEntity.Instance.transform;
        }

        return playerTransform != null;
    }

    private void ScanForEntities()
    {
        var entitiesInRange = Physics.SphereCastAll(
            playerTransform.position,
            nameplateViewDistance,
            transform.forward,
            0,
            entityMask
        );

        ProcessEntitiesInRange(entitiesInRange);
        ProcessHoveredEntity();
        ProcessTargetedEntity();
    }

    private void ProcessEntitiesInRange(RaycastHit[] entities)
    {
        foreach (var hit in entities)
        {
            if (hit.transform.TryGetComponent<Entity>(out var entity) &&
                entity.Identity.Id != GameClient.Instance.CurrentPlayerId &&
                IsNameplateVisible(entity.transform))
            {
                nameplates.GetOrAdd(entity.Identity.Id, _ => CreateNameplate(entity));
            }
        }
    }

    private void ProcessHoveredEntity()
    {
        var hoveredObject = ClickManager.Instance.HoverObjectData;
        if (ShouldCreateNameplateForEntity(hoveredObject))
        {
            var entity = hoveredObject.ObjectTransform.GetComponent<Entity>();
            nameplates.GetOrAdd(entity.Identity.Id, _ => CreateNameplate(entity));
        }
    }

    private bool ShouldCreateNameplateForEntity(ObjectData objectData)
    {
        return objectData != null &&
               objectData.ObjectTransform != null &&
               entityMask == (entityMask | (1 << objectData.ObjectLayer)) &&
               objectData.ObjectTransform.TryGetComponent<Entity>(out var entity) &&
               entity.Identity.Id != GameClient.Instance.CurrentPlayerId;
    }

    private void ProcessTargetedEntity()
    {
        if (!TargetManager.Instance.HasTarget()) return;

        var targetTransform = TargetManager.Instance.Target.Data.ObjectTransform;
        if (targetTransform.TryGetComponent<Entity>(out var entity) &&
            entity.Identity.Id != GameClient.Instance.CurrentPlayerId)
        {
            nameplates.GetOrAdd(entity.Identity.Id, _ => CreateNameplate(entity));
        }
    }

    private void HandlePlayerNameplate()
    {
        playerNameplate ??= CreatePlayerNameplate(PlayerEntity.Instance);
        UpdatePlayerNameplate();
    }

    private void UpdatePlayerNameplate()
    {
        playerNameplate.Visible = IsNameplateVisible(playerNameplate.Target);
        if (playerNameplate.Visible)
        {
            playerNameplate.Show();
            UpdateNameplatePosition(playerNameplate);
            UpdateNameplateStyle(playerNameplate);
            UpdatePlayerGauge();
        }
        else
        {
            playerNameplate.Hide();
        }
    }

    private void ProcessNameplateVisibility()
    {
        var nameplateIds = nameplates.Keys.ToList();
        foreach (var id in nameplateIds)
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

    private void UpdateNameplateSystem()
    {
        foreach (var nameplate in nameplates.Values)
        {
            if (nameplate.Target == null) continue;

            UpdateNameplatePosition(nameplate);
            UpdateNameplateStyle(nameplate);
        }
    }

    private void UpdateNameplateStyle(Nameplate nameplate)
    {
        nameplate.NameplateOffsetHeight = nameplate.Entity.IsDead ?
            nameplate.Entity.Appearance.CollisionHeight * 0.85f :
            nameplate.Entity.Appearance.CollisionHeight * nameplateHeightMultiplier;

        var target = TargetManager.Instance;
        var isCurrentTarget = target.HasTarget() && target.Target.Data.ObjectTransform == nameplate.Target;

        if (isCurrentTarget)
        {
            UpdateTargetedNameplateStyle(nameplate, target);
        }
        else
        {
            nameplate.RemoveStyle("target-bubble-attack");
            nameplate.RemoveStyle("target-bubble-target");
        }

        UpdateHoveredNameplateStyle(nameplate);
    }

    private void UpdateTargetedNameplateStyle(Nameplate nameplate, TargetManager target)
    {
        var isAttackTarget = target.AttackTarget == target.Target &&
                            target.AttackTarget.Identity.EntityType != EntityType.NPC &&
                            !target.AttackTarget.Status.IsDead;

        if (isAttackTarget)
        {
            nameplate.SetStyle("target-bubble-attack");
        }
        else
        {
            nameplate.SetStyle("target-bubble-target");
            nameplate.RemoveStyle("target-bubble-attack");
        }
    }

    private void UpdateHoveredNameplateStyle(Nameplate nameplate)
    {
        var isHovered = ClickManager.Instance.HoverObjectData?.ObjectTransform == nameplate.Target;
        if (isHovered)
        {
            nameplate.SetStyle("target-bubble-hover");
        }
        else
        {
            nameplate.RemoveStyle("target-bubble-hover");
        }
    }

    private void UpdateNameplatePosition(Nameplate nameplate)
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

    public void StartCasting(SetupGaugePacket.GaugeColor color, int durationMs)
    {
        playerNameplate?.ShowGauge(color, Time.time, durationMs);
    }

    private void UpdatePlayerGauge()
    {
        if (playerNameplate == null) return;

        if (playerNameplate.GaugeEndTime - Time.time > 0)
        {
            playerNameplate.UpdateGauge(Time.time);
        }
        else
        {
            playerNameplate.HideGauge();
        }
    }

    private bool IsNameplateVisible(Transform target)
    {
        if (target == null) return false;

        var isHovered = ClickManager.Instance.HoverObjectData?.ObjectTransform == target;
        if (isHovered) return true;

        var isTarget = TargetManager.Instance.HasTarget() &&
                      TargetManager.Instance.Target.Data.ObjectTransform == target;
        var isTooFar = Vector3.Distance(playerTransform.position, target.position) > nameplateViewDistance;

        return (!isTooFar || isTarget) && CameraController.Instance.IsObjectVisible(target);
    }

    private void OnDestroy()
    {
        nameplates.Clear();
        instance = null;
    }

    // Factory methods for creating nameplates
    private Nameplate CreateNameplate(Entity entity)
    {
        var element = nameplateTemplate.Instantiate()[0];
        var nameplate = new Nameplate(
            element,
            element.Q<Label>("EntityName"),
            element.Q<Label>("EntityTitle"),
            entity
        );
        rootElement.Add(element);
        return nameplate;
    }

    private PlayerNameplate CreatePlayerNameplate(Entity entity)
    {
        var element = nameplateTemplate.Instantiate()[0];
        var nameplate = new PlayerNameplate(
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
        if (nameplates.TryRemove(id, out var removed))
        {
            rootElement.Remove(removed.NameplateEle);
        }
    }

    public void SetMask(LayerMask mask) => entityMask = mask;
}
