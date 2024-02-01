using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour {
    [SerializeField] private GameObject _locator;
    [SerializeField] private ObjectData _targetObjectData;
    [SerializeField] private ObjectData _hoverObjectData;

    public ObjectData HoverObjectData { get { return _hoverObjectData; } }

    private Vector3 _lastClickPosition = Vector3.zero;
    private LayerMask _entityMask;
    private LayerMask _clickThroughMask;

    private static ClickManager _instance;
    public static ClickManager Instance { get { return _instance; } }

    void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    void Start() {
        _locator = GameObject.Find("Locator");
        HideLocator();
    }

    public void SetMasks(LayerMask entityMask, LayerMask clickThroughMask) {
        _entityMask = entityMask;
        _clickThroughMask = clickThroughMask;
    }

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 1000f, ~_clickThroughMask)) {
            int hitLayer = hit.collider.gameObject.layer;
            if(_entityMask == (_entityMask | (1 << hitLayer))) {
                _hoverObjectData = new ObjectData(hit.transform.parent.gameObject);
            } else {
                _hoverObjectData = new ObjectData(hit.collider.gameObject);
            }

            if(InputManager.GetInstance().IsInputPressed(InputType.LeftMouseButtonDown) &&
                !InputManager.GetInstance().IsInputPressed(InputType.RightMouseButton)) 
            {
                _targetObjectData = _hoverObjectData;
                _lastClickPosition = hit.point;

                if(_entityMask == (_entityMask | (1 << hitLayer)) && _targetObjectData.ObjectTag != "Player") {
                    Debug.Log("Hit entity");
                    TargetManager.Instance.SetTarget(_targetObjectData);
                } else if(_targetObjectData != null) {                  
                    ClickToMoveController.Instance.MoveTo(_targetObjectData, _lastClickPosition);
                    float angle = Vector3.Angle(hit.normal, Vector3.up);
                    if(angle < 85f) {
                        PlaceLocator(_lastClickPosition);
                    } else {
                        HideLocator();
                    }
                }
            }
        }
    }

    public void PlaceLocator(Vector3 position) {
        _locator.SetActive(true);
        _locator.gameObject.transform.position = position;
    }

    public void HideLocator() {
        _locator.SetActive(false);
    }
}
