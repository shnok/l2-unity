using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkTransformShare))]
public class NetworkCharacterControllerShare : MonoBehaviour {
    private CharacterController _characterController;
    private NetworkTransformShare _networkTransformShare;

    [SerializeField] private bool _sharing = false;
    [SerializeField] private float _sharingLoopDelaySec = 0.1f;
    [SerializeField] private float _lastSpeed; 
    [SerializeField] private Vector3 _lastDirection;

    void Start() {
        _characterController = GetComponent<CharacterController>();
        if(_characterController == null || World.Instance.OfflineMode) {
            this.enabled = false;
            return;
        }

        if (_networkTransformShare == null) {
            _networkTransformShare = GetComponent<NetworkTransformShare>();
        }

        StartCoroutine(StartSharingMoveDirection());
    }

    IEnumerator StartSharingMoveDirection() {
        _sharing = true;
        while(_sharing) {
            yield return new WaitForSeconds(_sharingLoopDelaySec);

            float speed = _characterController.velocity.magnitude;
            Vector3 direction = _characterController.velocity.normalized;

            if(_lastSpeed != _characterController.velocity.magnitude || _lastDirection != direction) {
                _lastSpeed = _characterController.velocity.magnitude;
                _lastDirection = direction;
                ShareMoveDirection(direction);

                if(direction.x == 0 && direction.z == 0) {
                    Debug.Log("Player stopped, share pos");
                    _networkTransformShare.SharePosition();
                }
            }
        }
    } 

    public void ShareMoveDirection(Vector3 moveDirection) {
        ClientPacketHandler.Instance.UpdateMoveDirection(moveDirection);
    }
}
