using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[RequireComponent(typeof(NetworkAnimationReceive), typeof(NetworkTransformReceive), typeof(CharacterController))]
public class NetworkCharacterControllerReceive : MonoBehaviour
{
    private CharacterController _characterController;
    private NetworkAnimationReceive _animationReceive;
    private NetworkTransformReceive _networkTransformReceive;
    private Entity _entity;
    [SerializeField] private Vector3 _direction;
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _destination;
    [SerializeField] private float _gravity = 28f;
    [SerializeField] private float _moveSpeedMultiplier = 0.95f;

    void Start() {
        if(World.Instance.OfflineMode) {
            this.enabled = false;
        }
        _entity = GetComponent<Entity>();
        _networkTransformReceive = GetComponent<NetworkTransformReceive>();
        _animationReceive = GetComponent<NetworkAnimationReceive>();
        _characterController = GetComponent<CharacterController>();

        if(_characterController == null || World.Instance.OfflineMode || _animationReceive == null) {
            this.enabled = false;
        }
        _direction = Vector3.zero;
        _destination = Vector3.zero;
    }

    public void UpdateMoveDirection(float speed, Vector3 direction) {
        _speed = speed;
        _direction = direction;
        _animationReceive.SetFloat("Speed", speed);
    }

    private void FixedUpdate() {

        if(!_networkTransformReceive.IsPositionSynced()) {
            /* pause script during position sync */
            //return;
        }

        if(_destination != null && _destination != Vector3.zero) {
            SetMoveDirectionToDestination();
        }

        Vector3 ajustedDirection = _direction * _speed * _moveSpeedMultiplier + Vector3.down * _gravity;
        _characterController.Move(ajustedDirection * Time.deltaTime);
    }

    public void SetDestination(Vector3 destination, float speed) {
        _speed = speed;
        _destination = destination;
        _animationReceive.SetFloat("Speed", speed);
    }

    public void SetMoveDirectionToDestination() {
        Vector3 transformFlat = VectorUtils.To2D(transform.position);
        Vector3 destinationFlat = VectorUtils.To2D(_destination);

        if(Vector3.Distance(transformFlat, destinationFlat) > Geodata.Instance.NodeSize / 20f) {
            _networkTransformReceive.PausePositionSync();
            _direction = (destinationFlat - transformFlat).normalized;
        } else {
            if(_direction != Vector3.zero) {
                _entity.OnStopMoving();
                //TODO check if has target and is attacking
            }

            _direction = Vector3.zero;
            _networkTransformReceive.ResumePositionSync();
        }
    }
}
