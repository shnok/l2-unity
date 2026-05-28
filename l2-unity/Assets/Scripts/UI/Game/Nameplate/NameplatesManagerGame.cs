using UnityEngine;
using UnityEngine.UIElements;

public class NameplatesManagerGame : NameplatesManagerBase
{
    private PlayerNameplate playerNameplate;

    private static NameplatesManagerGame instance;
    public static NameplatesManagerGame Instance => instance;

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

    private void FixedUpdate()
    {
        if (!IsSystemReady()) return;

        ScanForEntities();
        ProcessNameplateVisibility();
        HandlePlayerNameplate();
    }

    protected override bool IsSystemReady()
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

    protected override void ScanForEntities()
    {
        base.ScanForEntities();

        ProcessHoveredEntity();
        ProcessTargetedEntity();
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

    protected virtual void ProcessTargetedEntity()
    {
        if (!TargetManager.Instance.HasTarget()) return;

        var targetTransform = TargetManager.Instance.Target.transform;
        if (targetTransform.TryGetComponent<Entity>(out var entity) &&
            entity.Identity.Id != GameClient.Instance.CurrentPlayerId)
        {
            nameplates.GetOrAdd(entity.Identity.Id, _ => CreateNameplate(entity));
        }
    }

    protected override bool CheckIfNotPlayer(Entity entity)
    {
        return entity.Identity.Id != GameClient.Instance.CurrentPlayerId;
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
    protected override bool IsNameplateVisible(Transform target)
    {
        if (target == null) return false;

        var isHovered = ClickManager.Instance.HoverObjectData?.ObjectTransform == target;
        if (isHovered) return true;

        var isTarget = TargetManager.Instance.HasTarget() &&
                      TargetManager.Instance.Target.transform == target;
        var isTooFar = Vector3.Distance(playerTransform.position, target.position) > nameplateViewDistance;

        return (!isTooFar || isTarget) && CameraController.Instance.IsObjectVisible(target);
    }

    protected override void UpdateNameplateStyle(Nameplate nameplate)
    {
        base.UpdateNameplateStyle(nameplate);

        var target = TargetManager.Instance;
        var isCurrentTarget = target.HasTarget() && target.Target.transform == nameplate.Target;

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

    protected void UpdateTargetedNameplateStyle(Nameplate nameplate, TargetManager target)
    {
        var isAttackTarget = target.AttackTarget == target.Target &&
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

    protected void UpdateHoveredNameplateStyle(Nameplate nameplate)
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

    public void StartCasting(SetupGaugePacket.GaugeColor color, int durationMs)
    {
        playerNameplate?.ShowGauge(color, Time.time, durationMs);
    }

    public void StopCasting()
    {
        playerNameplate?.HideGauge();
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

    private void OnDestroy()
    {
        nameplates.Clear();
        instance = null;
    }

    public override void ClearNameplates()
    {
        base.ClearNameplates();

        if (playerNameplate != null)
        {
            if (playerNameplate.NameplateEle != null)
            {
                rootElement.Remove(playerNameplate.NameplateEle);
            }

            playerNameplate = null;
        }
    }
}
