using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField] private TargetData target;

    private static TargetManager instance;
    public static TargetManager GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }

    private void Start() {
        target = null;
    }

    public void SetTarget(ObjectData target) {
        this.target = new TargetData(target);
    }

    public TargetData GetTargetData() {
        return target;
    }

    public void ClearTarget() {
        target = null;
    }

    void Update() {
        if(HasTarget()) {
            target.distance = Vector3.Distance(
                PlayerController.GetInstance().transform.position, 
                target.data.objectTransform.position);
        } else {
            ClearTarget();
        }

        if(InputManager.GetInstance().IsInputPressed(InputType.Escape)) {
            ClearTarget();
        }
    }


    internal bool HasTarget() {
        return (target != null && target.data != null && target.data.objectTransform != null);
    }
}
