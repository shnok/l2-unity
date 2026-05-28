using UnityEngine;

public class Windmill : MonoBehaviour {
    public float rotateSpeed = 1f;
    void Update() {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + Time.deltaTime * rotateSpeed);
    }
}
