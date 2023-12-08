using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacterControllerShare : MonoBehaviour {
    private CharacterController characterController;
    [SerializeField] private bool sharing = false;
    [SerializeField] private float sharingLoopDelaySec = 0.1f;
    [SerializeField] private float lastSpeed; 
    [SerializeField] private Vector3 lastDirection;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if(characterController == null || World.GetInstance().offlineMode) {
            this.enabled = false;
            return;
        }

        StartCoroutine(StartSharingMoveDirection());
    }

    IEnumerator StartSharingMoveDirection() {
        sharing = true;
        while(sharing) {
            yield return new WaitForSeconds(sharingLoopDelaySec);

            float speed = characterController.velocity.magnitude;
            Vector3 direction = characterController.velocity.normalized;

            if(lastSpeed != characterController.velocity.magnitude || lastDirection != direction) {
                lastSpeed = characterController.velocity.magnitude;
                lastDirection = direction;
                ShareMoveDirection(speed, direction);
            }
        }
    } 

    public void ShareMoveDirection(float speed, Vector3 moveDirection) {
        ClientPacketHandler.GetInstance().UpdateMoveDirection(speed, moveDirection);
    }
}
