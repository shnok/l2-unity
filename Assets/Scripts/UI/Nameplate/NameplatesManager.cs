using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NameplatesManager : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset nameplateTemplate;
    private VisualElement rootElement;
    public float nameplateViewDistance = 50f;
    public LayerMask entityMask;
    public LayerMask occlusionMask;
    public float occlusionBaseHeight = 1.5f;
    [SerializeField] public RaycastHit[] entitiesInRange;

    private Dictionary<int, Nameplate> nameplates = new Dictionary<int, Nameplate>();
    public Transform playerTransform;

    void Start()
    {
        if(nameplateTemplate == null) {
            nameplateTemplate = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Data/UI/_Elements/Nameplate.uxml");
        }
        if(nameplateTemplate == null) {
            Debug.LogError("Could not load chat window template.");
        }
        playerTransform = PlayerEntity.GetInstance().transform;
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
        if(!L2GameUI.GetInstance().uiLoaded) {
            return;
        }

        if(rootElement == null) {
            rootElement = L2GameUI.GetInstance().GetRootElement().Q<VisualElement>("NameplatesContainer");
            return;
        }

        entitiesInRange = Physics.SphereCastAll(playerTransform.position, nameplateViewDistance, transform.forward, 0, entityMask);
        CreateNameplateForEntities();
        CheckNameplateVisibility();
        CheckMouseOver();
        CheckTarget();
    }

    private void CheckMouseOver() {
        ObjectData hoverObjectData = ClickManager.GetInstance().hoverObjectData;
        if(hoverObjectData != null) {
            if(entityMask == (entityMask | (1 << hoverObjectData.objectLayer))) {
                Entity e = hoverObjectData.objectTransform.GetComponent<Entity>();
                if(e != null) {
                    if(!nameplates.ContainsKey(e.Identity.Id)) {
                        CreateNameplate(e);
                    }
                }
            }
        }
    }

    private void CheckTarget() {
        if(!TargetManager.GetInstance().HasTarget()) {
            return;
        }

        Entity e = TargetManager.GetInstance().GetTargetData().data.objectTransform.GetComponent<Entity>();
        if(e != null) {
            if(!nameplates.ContainsKey(e.Identity.Id)) {
                CreateNameplate(e);
            }
        }
    }

    private void CreateNameplateForEntities() {
        foreach(RaycastHit hit in entitiesInRange) {
            Entity objectEntity = hit.transform.GetComponent<Entity>();
            if(objectEntity != null) {
                int objectId = objectEntity.Identity.Id;

                if(!nameplates.ContainsKey(objectId)) {
                    CreateNameplate(objectEntity);
                }
            }
        }
    }

    private void CreateNameplate(Entity entity) {
        if(!IsNameplateVisible(entity.transform)) {
            return;
        }

        VisualElement visualElement = nameplateTemplate.Instantiate()[0];

        Nameplate nameplate = new Nameplate {
            nameplateEle = visualElement,
            nameplateEntityName = visualElement.Q<Label>("EntityName"),
            nameplateEntityTitle = visualElement.Q<Label>("EntityTitle"),
            target = entity.transform,
            title = "",
            nameplateOffsetHeight = 1f,
            name = entity.Identity.Name,
            targetId = entity.Identity.Id,
            visible = true
        };

        nameplate.nameplateEntityName.text = nameplate.name;
        nameplate.nameplateEntityTitle.text = nameplate.title;

        nameplates.Add(entity.Identity.Id, nameplate);
        rootElement.Add(visualElement);
    }

    private void CheckNameplateVisibility() {
        foreach(var nameplateId in nameplates.Keys) {
            var nameplate = nameplates[nameplateId];
            if(!IsNameplateVisible(nameplate.target)) {
                nameplate.visible = false;
            } else {
                nameplate.visible = true;
            }
        }
    }

    private void UpdateNameplates() {
        var keysToRemove = new List<int>();
        foreach(var nameplateId in nameplates.Keys) {
            var nameplate = nameplates[nameplateId];
            if(!nameplate.visible) {
                keysToRemove.Add(nameplateId);
            } else {
                UpdateNameplatePosition(nameplate);
                UpdateNameplateStyle(nameplate);
            }
        }
        foreach(var key in keysToRemove) {
            rootElement.Remove(nameplates[key].nameplateEle);
            nameplates.Remove(key);
        }
    }

    private void UpdateNameplateStyle(Nameplate nameplate) {
        if(TargetManager.GetInstance().HasTarget() && TargetManager.GetInstance().GetTargetData().data.objectTransform == nameplate.target) {
            nameplate.SetStyle("target-bubble-target");
            return;
        } else {
            nameplate.RemoveStyle("target-bubble-target");
        }
        
        if(ClickManager.GetInstance().hoverObjectData != null && ClickManager.GetInstance().hoverObjectData.objectTransform == nameplate.target) {
            nameplate.SetStyle("target-bubble-hover");
        } else {
            nameplate.RemoveStyle("target-bubble-hover");
        }
    }

    private void UpdateNameplatePosition(Nameplate nameplate) {
        Vector2 nameplatePos = Camera.main.WorldToScreenPoint(nameplate.target.position + Vector3.up * nameplate.nameplateOffsetHeight);
        nameplate.nameplateEle.style.left = nameplatePos.x - nameplate.nameplateEle.resolvedStyle.width / 2f;
        nameplate.nameplateEle.style.top = Screen.height - nameplatePos.y - nameplate.nameplateEle.resolvedStyle.height / 2f;
    }

    private bool IsNameplateVisible(Transform target) {
        if(target == null) {
            return false;
        }

        if(ClickManager.GetInstance().hoverObjectData != null && ClickManager.GetInstance().hoverObjectData.objectTransform == target) {
            return true;
        }
        if(TargetManager.GetInstance().HasTarget() && TargetManager.GetInstance().GetTargetData().data.objectTransform == target) {
            return true;
        }
        if(Vector3.Distance(playerTransform.position, target.position) > nameplateViewDistance) {
            return false;
        }

        return CameraController.GetInstance().IsObjectVisible(target);
    }
}
