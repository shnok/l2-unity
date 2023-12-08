using UnityEngine;

public class NetworkTransformReceive : MonoBehaviour {
    [SerializeField] public Vector3 serverPosition;
    private Vector3 lastPos;
    private Quaternion lastRot, newRot;
    private float rotLerpValue, posLerpValue;
    [SerializeField] private bool positionSynced = false;
    public float lerpDuration = 0.3f;
    public bool noclip = true;

    void Start() {
        if(World.GetInstance().offlineMode) {
            this.enabled = false;
            return;
        }

        lastPos = transform.position;
        newRot = transform.rotation;
        lastRot = transform.rotation;
        serverPosition = transform.position;
    }

    void FixedUpdate() {
        AdjustPosition();           
        LerpToRotation();
    }

    /* POSITION */

    public void SetNewPosition(Vector3 pos) {
        lastPos = transform.position;
        posLerpValue = 0;
        serverPosition = pos;
        
        if(Vector3.Distance(VectorUtils.To2D(transform.position), VectorUtils.To2D(serverPosition)) > 0.5f) {
            positionSynced = false;
        }
    }

    public void AdjustPosition() {
        /* Is offset is too high */
        if(!positionSynced) {
            LerpToPosition();
        }      
    }

    /* Clip to destination */
    public void LerpToPosition() {
        transform.position = Vector3.Lerp(lastPos, serverPosition, posLerpValue);
        posLerpValue += (1 / lerpDuration) * Time.deltaTime;

        if(Vector3.Distance(VectorUtils.To2D(transform.position), VectorUtils.To2D(serverPosition)) <= 0.5f) {
            positionSynced = true;
        }
    }

    /* ROTATION */
    public void LookAt(Vector3 dest) {
        var heading = dest - transform.position;
        float angle = Vector3.Angle(heading, Vector3.forward);
        Vector3 cross = Vector3.Cross(heading, Vector3.forward);
        if(cross.y >= 0) angle = -angle;
        RotateTo(angle);
    }

    public void LerpToRotation() {
        if(Mathf.Abs(transform.rotation.eulerAngles.y - newRot.eulerAngles.y) > 2f) {
            transform.rotation = Quaternion.Lerp(lastRot, newRot, rotLerpValue);
            rotLerpValue += Time.deltaTime * 7.5f;
        }
    }

    public void RotateTo(float angle) {
        Quaternion rot = transform.rotation;
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.y = angle;
        rot.eulerAngles = eulerAngles;

        newRot = rot;
        lastRot = transform.rotation;
        rotLerpValue = 0;
    }
}