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
        if(target == null) {
            ClearTarget();
            return;
        }

        _target = new TargetData(target);

        PlayerEntity.Instance.TargetId = _target.Identity.Id;
        ClientPacketHandler.Instance.SendRequestSetTarget(_target.Identity.Id);
    }

    public TargetData GetTargetData() {
        return _target;
    }

    public void ClearTarget() {
        if(_target != null) {
            ClientPacketHandler.Instance.SendRequestSetTarget(-1);
            PlayerEntity.Instance.TargetId = -1;
            _target = null;
        }
    }

    void Update() {
        if (PlayerEntity.Instance == null) {
            return;
        }

        if(HasTarget()) {
            _target.Distance = Vector3.Distance(
                PlayerController.Instance.transform.position, 
                _target.Data.ObjectTransform.position);
        } else {
            ClearTarget();
        }

        if(InputManager.Instance.IsInputPressed(InputType.Escape)) {
            ClearTarget();
        }
    }

    internal bool HasTarget() {
        return (_target != null && _target.Data != null && _target.Data.ObjectTransform != null);
    }
}
