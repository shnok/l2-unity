using UnityEngine;

public class World : MonoBehaviour {
    public GameObject player;

    public static World _instance;
    public static World GetInstance() {
        return _instance;
    }

    void Awake() {
        if(_instance == null) {
            _instance = this;
        } else {
            Object.Destroy(gameObject);
        }
    }

    void Start() {
        CameraController.GetInstance().SetTarget(player);
    }

    // Update is called once per frame
    void Update() {

    }
}
