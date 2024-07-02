using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyNameplatesManager : MonoBehaviour {
    private VisualElement _rootElement;
    private VisualTreeAsset _nameplateTemplate;
    private readonly Dictionary<int, Nameplate> _nameplates = new Dictionary<int, Nameplate>();

    [SerializeField] private Camera _camera;
    [SerializeField] private float _nameplateViewDistance = 50f;
    [SerializeField] private LayerMask _entityMask;
    [SerializeField] public RaycastHit[] _entitiesInRange;

    public Camera Camera { get { return _camera; }  set { _camera = value; } }

    private static LobbyNameplatesManager _instance;
    public static LobbyNameplatesManager Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
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
        if (_nameplateTemplate == null) {
            _nameplateTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Nameplate");
        }
        if (_nameplateTemplate == null) {
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
        while (_accumulation >= kUpdateInterval) {
            UpdateNameplates();
            _accumulation -= kUpdateInterval;
        }
    }


    private void FixedUpdate() {
        if (_camera == null) {
            ClearNameplates();
            return;
        }

        if (!L2LoginUI.Instance.UILoaded) {
            return;
        }

        if (_rootElement == null) {
            _rootElement = L2LoginUI.Instance.RootElement.Q<VisualElement>("NameplatesContainer");
            return;
        }

        _entitiesInRange = Physics.SphereCastAll(_camera.transform.position, _nameplateViewDistance, transform.forward, 0, _entityMask);
        CreateNameplateForEntities();
        CheckNameplateVisibility();
    }

    private void CreateNameplateForEntities() {
        foreach (RaycastHit hit in _entitiesInRange) {
            SelectableCharacterEntity objectEntity = hit.transform.GetComponent<SelectableCharacterEntity>();
            if (objectEntity != null) {
                int objectId = objectEntity.CharacterInfo.Id;

                if (!_nameplates.ContainsKey(objectId)) {
                    CreateNameplate(objectEntity);
                }
            }
        }
    }

    private void CreateNameplate(SelectableCharacterEntity entity) {
        if (!IsNameplateVisible(entity.transform)) {
            return;
        }

        VisualElement visualElement = _nameplateTemplate.Instantiate()[0];

        Nameplate nameplate = new Nameplate(
            visualElement,
            visualElement.Q<Label>("EntityName"),
            visualElement.Q<Label>("EntityTitle"),
            entity.transform,
            "",
            "9CE8A9FF",
            1f, //height
            entity.CharacterInfo.Name,
            entity.CharacterInfo.Id,
            true
            );

        _nameplates.Add(entity.CharacterInfo.Id, nameplate);
        _rootElement.Add(visualElement);
    }

    private void CheckNameplateVisibility() {
        foreach (var nameplateId in _nameplates.Keys) {
            var nameplate = _nameplates[nameplateId];
            if (!IsNameplateVisible(nameplate.Target)) {
                nameplate.Visible = false;
            } else {
                nameplate.Visible = true;
            }
        }
    }

    private void UpdateNameplates() {
        var keysToRemove = new List<int>();
        foreach (var nameplateId in _nameplates.Keys) {
            var nameplate = _nameplates[nameplateId];
            if (!nameplate.Visible) {
                keysToRemove.Add(nameplateId);
            } else {
                UpdateNameplatePosition(nameplate);
            }
        }
        foreach (var key in keysToRemove) {
            _rootElement.Remove(_nameplates[key].NameplateEle);
            _nameplates.Remove(key);
        }
    }


    private void UpdateNameplatePosition(Nameplate nameplate) {
        try {
            Vector2 nameplatePos = _camera.WorldToScreenPoint(nameplate.Target.position + Vector3.up * nameplate.NameplateOffsetHeight);
            nameplate.NameplateEle.style.left = nameplatePos.x - nameplate.NameplateEle.resolvedStyle.width / 2f;
            nameplate.NameplateEle.style.top = Screen.height - nameplatePos.y - nameplate.NameplateEle.resolvedStyle.height;
        } 
        catch (NullReferenceException) { } 
        catch  (MissingReferenceException) { }  
    }

    private bool IsNameplateVisible(Transform target) {
        if (target == null) {
            return false;
        }

        bool isTooFar = Vector3.Distance(_camera.transform.position, target.position) > _nameplateViewDistance;
        if (isTooFar) {
            return false;
        }

        return true;
    }

    private void ClearNameplates() {
        foreach (var nameplate in _nameplates.Values) {
            nameplate.NameplateEle.RemoveFromHierarchy();
        }

        _nameplates.Clear();
    }
}