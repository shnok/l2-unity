using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TargetObjectData {
    public string targetLayer;
    public string targetTag;
    public string targetScene;
    public Transform targetObject;
}
public class ClickManager : MonoBehaviour {
    public Vector3 lastClickPosition = Vector3.zero;
    public TargetObjectData targetObjectData;
    public TargetObjectData hoverObjectData;

    public LayerMask walkableMask;
    public LayerMask entityMask;

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit)) {
            hoverObjectData.targetObject = hit.collider.gameObject.transform;
            hoverObjectData.targetTag = hit.collider.gameObject.tag;

            int hitLayer = hit.collider.gameObject.layer;
            hoverObjectData.targetLayer = LayerMask.LayerToName(hitLayer);
            hoverObjectData.targetScene = hit.collider.gameObject.scene.name;

            if(InputManager.GetInstance().IsInputPressed(InputType.LeftMouseButtonDown) &&
                !InputManager.GetInstance().IsInputPressed(InputType.RightMouseButton)) 
            {
                targetObjectData = hoverObjectData;
                lastClickPosition = hit.point;

                if(entityMask == (entityMask | (1 << hitLayer))) {
                    Debug.Log("Hit entity");
                } else {
                    ClickToMoveController.GetInstance().MoveTo(targetObjectData, lastClickPosition);
                }
            }
        }
    }
}
