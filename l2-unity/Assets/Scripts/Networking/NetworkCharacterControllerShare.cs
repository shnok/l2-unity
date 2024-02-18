using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkTransformShare)), RequireComponent(typeof(CharacterController))]
public class NetworkCharacterControllerShare : MonoBehaviour {
    private CharacterController _characterController;
    private NetworkTransformShare _networkTransformShare;

    [SerializeField] private bool _sharing = false;
    [SerializeField] private float _sharingLoopDelaySec = 0.1f;
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
            
            if(_lastDirection != PlayerController.Instance.MoveDirection.normalized) { 
                ShareMoveDirection(PlayerController.Instance.MoveDirection.normalized);

                if(PlayerController.Instance.MoveDirection.x == 0 && PlayerController.Instance.MoveDirection.z == 0) {
                    Debug.Log("Player stopped, share pos");
                    _networkTransformShare.SharePosition();
                    _networkTransformShare.ShouldShareRotation = false;
                } else {
                    _networkTransformShare.ShouldShareRotation = true;
                }
            }
        }
    } 

    public void ShareMoveDirection(Vector3 moveDirection) {
        _lastDirection = moveDirection;
        ClientPacketHandler.Instance.UpdateMoveDirection(moveDirection); 
    }
}
