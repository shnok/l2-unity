using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour {
    public Vector3 lastClickPosition = Vector3.zero;
    public GameObject locator;
    public ObjectData objectTransformData;
    public ObjectData hoverObjectData;

    public LayerMask walkableMask;
    public LayerMask entityMask;

    public static ClickManager _instance;
    public static ClickManager GetInstance() {
        return _instance;
    }

    void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    void Start() {
        locator = GameObject.Find("Locator");
        HideLocator();
    }

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit)) {
            hoverObjectData = new ObjectData(hit.collider.gameObject);
            int hitLayer = hit.collider.gameObject.layer;

            if(InputManager.GetInstance().IsInputPressed(InputType.LeftMouseButtonDown) &&
                !InputManager.GetInstance().IsInputPressed(InputType.RightMouseButton)) 
            {
                objectTransformData = hoverObjectData;
                lastClickPosition = hit.point;

                if(entityMask == (entityMask | (1 << hitLayer)) && objectTransformData.objectTag != "Player") {
                    Debug.Log("Hit entity");
                } else if(objectTransformData != null) {
                    ClickToMoveController.GetInstance().MoveTo(objectTransformData, lastClickPosition);
                    PlaceLocator(lastClickPosition);
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
