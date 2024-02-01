using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField] private TargetData _target;

    private static TargetManager _instance;
    public static TargetManager Instance { get { return _instance; } }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    private void Start() {
        _target = null;
    }

    public void SetTarget(ObjectData target) {
        this._target = new TargetData(target);
    }

    public TargetData GetTargetData() {
        return _target;
    }

    public void ClearTarget() {
        _target = null;
    }

    void Update() {
        if(HasTarget()) {
            _target.Distance = Vector3.Distance(
                PlayerController.Instance.transform.position, 
                _target.Data.ObjectTransform.position);
        } else {
            ClearTarget();
        }

        if(InputManager.GetInstance().IsInputPressed(InputType.Escape)) {
            ClearTarget();
        }
    }

    internal bool HasTarget() {
        return (_target != null && _target.Data != null && _target.Data.ObjectTransform != null);
    }
}
