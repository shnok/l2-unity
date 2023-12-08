using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour {
    public Vector3 lastClickPosition = Vector3.zero;
    public GameObject locator;
    public ObjectData targetObjectData;
    public ObjectData hoverObjectData;

    private LayerMask entityMask;
    private LayerMask clickThroughMask;

    public static ClickManager instance;
    public static ClickManager GetInstance() {
        return instance;
    }

    void Awake() {
        if(instance == null) {
            instance = this;
        }
    }

    void Start() {
        locator = GameObject.Find("Locator");
        HideLocator();
    }

    public void SetMasks(LayerMask entityMask, LayerMask clickThroughMask) {
        this.entityMask = entityMask;
        this.clickThroughMask = clickThroughMask;
    }

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 1000f, ~clickThroughMask)) {
            hoverObjectData = new ObjectData(hit.collider.gameObject);
            int hitLayer = hit.collider.gameObject.layer;

            if(InputManager.GetInstance().IsInputPressed(InputType.LeftMouseButtonDown) &&
                !InputManager.GetInstance().IsInputPressed(InputType.RightMouseButton)) 
            {
                targetObjectData = hoverObjectData;
                lastClickPosition = hit.point;

                if(entityMask == (entityMask | (1 << hitLayer)) && targetObjectData.objectTag != "Player") {
                    Debug.Log("Hit entity");
                    TargetManager.GetInstance().SetTarget(targetObjectData);
                } else if(targetObjectData != null) {                  
                    ClickToMoveController.GetInstance().MoveTo(targetObjectData, lastClickPosition);
                    float angle = Vector3.Angle(hit.normal, Vector3.up);
                    if(angle < 75f) {
                        PlaceLocator(lastClickPosition);
                    }
                }
            }
        }
    }

    public void PlaceLocator(Vector3 position) {
        locator.SetActive(true);
        locator.gameObject.transform.position = position;
    }

    public void HideLocator() {
        locator.SetActive(false);
    }
}
