using UnityEngine;

public class NetworkTransformReceive : MonoBehaviour {
    private Animator _animator;
    private CharacterController _characterController;
    public Vector3 _serverPosition;
    private Vector3 _lastPos, _destination;
    private Quaternion _lastRot, _newRot;
    private float _rotLerpValue, _posLerpValue;
    private bool _positionSync = false;
    public float moveSpeed = 4f;
    public float lerpDuration = 0.3f;
    public bool noclip = true;

    void Start() {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _lastPos = transform.position;
        _destination = transform.position;
        _newRot = transform.rotation;
        _lastRot = transform.rotation;
        _serverPosition = transform.position;
    }

    void FixedUpdate() {
        UpdatePosition();           
        LerpToRotation();
    }

    /* POSITION */
    public Vector3 To2D(Vector3 pos) {
        return new Vector3(pos.x, 0, pos.z);
    }

    public void SetNewPosition(Vector3 pos) {
        _lastPos = transform.position;
        _posLerpValue = 0;
        _serverPosition = pos;
        
        if(Vector3.Distance(To2D(transform.position), To2D(_serverPosition)) > 0.5f) {
            _positionSync = false;
        }
    }

    public void SetDestination(Vector3 pos) {
        _lastPos = transform.position;
        _destination = pos;
    }

    public void UpdatePosition() {
        /* Is offset is too high */
        if(!_positionSync || noclip) {
            LerpToPosition();
        } else {
            MoveToPosition();
        }      
    }

    /* Clip to destination */
    public void LerpToPosition() {
        transform.position = Vector3.Lerp(_lastPos, _serverPosition, _posLerpValue);
        _posLerpValue += (1 / lerpDuration) * Time.deltaTime;

        if(Vector3.Distance(To2D(transform.position), To2D(_serverPosition)) <= 0.5f) {
            _positionSync = true;
        }
    }

    /* Walk to destination */
    public void MoveToPosition() {
        Vector3 transformFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetFlat = new Vector3(_destination.x, 0, _destination.z);

        Vector3 direction = Vector3.zero;
        if(Vector3.Distance(transformFlat, targetFlat) > 0.1f) {
            direction = targetFlat - transformFlat;      
        }
   
        direction = direction.normalized * moveSpeed;
        direction.y = -10;
        _characterController.Move(direction * Time.deltaTime);
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
        if(Mathf.Abs(transform.rotation.eulerAngles.y - _newRot.eulerAngles.y) > 2f) {
            transform.rotation = Quaternion.Lerp(_lastRot, _newRot, _rotLerpValue);
            _rotLerpValue += Time.deltaTime * 7.5f;
        }
    }

    public void RotateTo(float angle) {
        Quaternion rot = transform.rotation;
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.y = angle;
        rot.eulerAngles = eulerAngles;

        _newRot = rot;
        _lastRot = transform.rotation;
        _rotLerpValue = 0;
    }

    public void SetAnimationProperty(int animId, float value) {
        if(animId > 0 && animId < _animator.parameters.Length) {
            AnimatorControllerParameter anim = _animator.parameters[animId];
            switch(anim.type) {
                case AnimatorControllerParameterType.Float:
                    _animator.SetFloat(anim.name, value);
                    break;
                case AnimatorControllerParameterType.Int:
                    _animator.SetInteger(anim.name, (int)value);
                    break;
                case AnimatorControllerParameterType.Bool:
                    _animator.SetBool(anim.name, (int)value == 1);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    _animator.SetTrigger(anim.name);
                    break;
            }
        }     
    }
}