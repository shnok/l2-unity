using UnityEngine;

public class BillBoard : MonoBehaviour {
    [SerializeField] private BillboardType billboardType;

    public enum BillboardType { LookAtCamera, CameraForward };

    private void LateUpdate() {
        switch(billboardType) {
            case BillboardType.LookAtCamera:
                transform.LookAt(Camera.main.transform.position, Vector3.forward);
                break;
            case BillboardType.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            default:
                break;
        }
    }
}
