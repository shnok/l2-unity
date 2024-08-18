using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NameplatesManager : MonoBehaviour
{
    private VisualElement _rootElement;
    private VisualTreeAsset _nameplateTemplate;
    private readonly Dictionary<int, Nameplate> _nameplates = new Dictionary<int, Nameplate>();
    private Transform _playerTransform;

    [SerializeField] private float _nameplateViewDistance = 50f;
    [SerializeField] private LayerMask _entityMask;
    [SerializeField] public RaycastHit[] _entitiesInRange;

    private static NameplatesManager _instance;
    public static NameplatesManager Instance { get { return _instance; } }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    private void OnDestroy() {
        _nameplates.Clear();
        _instance = null;
    }

    void Start() {
        if(_nameplateTemplate == null) {
            _nameplateTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Game/Nameplate");
        }
        if(_nameplateTemplate == null) {
            Debug.LogError("Could not load chat window template.");
        }
    }

    public void SetMask(LayerMask mask) {
        _entityMask = mask;
    }

    private const int kUpdatesPerSecond = 200;
    private const float kUpdateInterval = 1.0f / kUpdatesPerSecond; // how many seconds pass before an update should happen
    private float _accumulation = 0.0f; // stores time elapsed
    private void Update() {
        // add to the accumulator
        _accumulation += Time.deltaTime;

        // while enough time has passed for an update, call our code we want executed 200 times per second.
        while(_accumulation >= kUpdateInterval) {
            UpdateNameplates();
            _accumulation -= kUpdateInterval;
        }
    }

    private void FixedUpdate() {
        if(_playerTransform == null) {
            if(PlayerEntity.Instance != null && PlayerEntity.Instance.transform != null) {
                _playerTransform = PlayerEntity.Instance.transform;
            } else {
                return;
            }
        }

        if(!L2GameUI.Instance.UILoaded) {
            return;
        }

        if(_rootElement == null) {
            _rootElement = L2GameUI.Instance.RootElement.Q<VisualElement>("NameplatesContainer");
            return;
        }

        _entitiesInRange = Physics.SphereCastAll(_playerTransform.position, _nameplateViewDistance, transform.forward, 0, _entityMask);
        CreateNameplateForEntities();
        CheckNameplateVisibility();
        CheckMouseOver();
        CheckTarget();
    }

    private void CheckMouseOver() {
        ObjectData hoverObjectData = ClickManager.Instance.HoverObjectData;
        if(hoverObjectData != null) {
            if(_entityMask == (_entityMask | (1 << hoverObjectData.ObjectLayer))) {
                Entity e = hoverObjectData.ObjectTransform.GetComponent<Entity>();
                if(e != null) {
                    if(!_nameplates.ContainsKey(e.Identity.Id)) {
                        CreateNameplate(e);
                    }
                }
            }
        }
    }

    private void CheckTarget() {
        if(!TargetManager.Instance.HasTarget()) {
            return;
        }

        Entity e = TargetManager.Instance.Target.Data.ObjectTransform.GetComponent<Entity>();
        if(e != null) {
            if(!_nameplates.ContainsKey(e.Identity.Id)) {
                CreateNameplate(e);
            }
        }
    }

    private void CreateNameplateForEntities() {
        foreach(RaycastHit hit in _entitiesInRange) {
            Entity objectEntity = hit.transform.GetComponent<Entity>();
            if(objectEntity != null) {
                int objectId = objectEntity.Identity.Id;

                if(!_nameplates.ContainsKey(objectId)) {
                    CreateNameplate(objectEntity);
                }
            }
        }
    }

    private void CreateNameplate(Entity entity) {
        if(!IsNameplateVisible(entity.transform)) {
            return;
        }

        VisualElement visualElement = _nameplateTemplate.Instantiate()[0];

        Nameplate nameplate = new Nameplate (
            visualElement,
            visualElement.Q<Label>("EntityName"),
            visualElement.Q<Label>("EntityTitle"),
            entity.transform,
            entity.Identity.Title,
            entity.Identity.TitleColor,
            entity.Appearance.CollisionHeight * 2.1f,
            entity.Identity.Name,
            entity.Identity.Id,
            true
            );

        _nameplates.Add(entity.Identity.Id, nameplate);
        _rootElement.Add(visualElement);
    }

    private void CheckNameplateVisibility() {
        foreach(var nameplateId in _nameplates.Keys) {
            var nameplate = _nameplates[nameplateId];
            if(!IsNameplateVisible(nameplate.Target)) {
                nameplate.Visible = false;
            } else {
                nameplate.Visible = true;
            }
        }
    }

    private void UpdateNameplates() {
        var keysToRemove = new List<int>();
        foreach(var nameplateId in _nameplates.Keys) {
            var nameplate = _nameplates[nameplateId];
            if(!nameplate.Visible) {
                keysToRemove.Add(nameplateId);
            } else {
                UpdateNameplatePosition(nameplate);
                UpdateNameplateStyle(nameplate);
            }
        }
        foreach(var key in keysToRemove) {
            _rootElement.Remove(_nameplates[key].NameplateEle);
            _nameplates.Remove(key);
        }
    }

    private void UpdateNameplateStyle(Nameplate nameplate) {
        if(TargetManager.Instance.HasTarget() && TargetManager.Instance.Target.Data.ObjectTransform == nameplate.Target) {
            if (TargetManager.Instance.AttackTarget == TargetManager.Instance.Target) {
                nameplate.SetStyle("target-bubble-attack");
            } else {
                nameplate.SetStyle("target-bubble-target");
                nameplate.RemoveStyle("target-bubble-attack");
            }
            return;
        } else {
            nameplate.RemoveStyle("target-bubble-attack");
            nameplate.RemoveStyle("target-bubble-target");
        }
        
        if(ClickManager.Instance.HoverObjectData != null && ClickManager.Instance.HoverObjectData.ObjectTransform == nameplate.Target) {
            nameplate.SetStyle("target-bubble-hover");
        } else {
            nameplate.RemoveStyle("target-bubble-hover");
        }
    }

    private void UpdateNameplatePosition(Nameplate nameplate) {
        try {
            Vector2 nameplatePos = Camera.main.WorldToScreenPoint(nameplate.Target.position + Vector3.up * nameplate.NameplateOffsetHeight);
            nameplate.NameplateEle.style.left = nameplatePos.x - nameplate.NameplateEle.resolvedStyle.width / 2f;
            nameplate.NameplateEle.style.top = Screen.height - nameplatePos.y - nameplate.NameplateEle.resolvedStyle.height;
        } 
        catch (NullReferenceException) { } 
        catch (MissingReferenceException) { }
    }

    private bool IsNameplateVisible(Transform target) {
        if(target == null) {
            return false;
        }

        bool isHover = ClickManager.Instance.HoverObjectData != null && ClickManager.Instance.HoverObjectData.ObjectTransform == target;
        if(isHover) {
            return true;
        }

        bool isTarget = TargetManager.Instance.HasTarget() && TargetManager.Instance.Target.Data.ObjectTransform == target;
        bool isTooFar = Vector3.Distance(_playerTransform.position, target.position) > _nameplateViewDistance;
        if(isTooFar && !isTarget) {
            return false;
        }

        return CameraController.Instance.IsObjectVisible(target);
    }
}
