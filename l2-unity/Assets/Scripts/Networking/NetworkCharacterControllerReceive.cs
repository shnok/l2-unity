using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[RequireComponent(typeof(NetworkAnimationReceive), typeof(NetworkTransformReceive), typeof(CharacterController))]
public class NetworkCharacterControllerReceive : MonoBehaviour
{
    private CharacterController characterController;
    private NetworkAnimationReceive animationReceive;
    private NetworkTransformReceive networkTransformReceive;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 destination;
    [SerializeField] private float gravity = 28f;


    void Start() {
        if(World.GetInstance().offlineMode) {
            this.enabled = false;
        }
        networkTransformReceive = GetComponent<NetworkTransformReceive>();
        animationReceive = GetComponent<NetworkAnimationReceive>();
        characterController = GetComponent<CharacterController>();

        if(characterController == null || World.GetInstance().offlineMode || animationReceive == null) {
            this.enabled = false;
        }
        direction = Vector3.zero;
        destination = transform.position;
    }

    public void UpdateMoveDirection(float speed, Vector3 direction) {
        this.speed = speed;
        this.direction = direction;
        animationReceive.SetFloat("Speed", speed);
    }

    private void FixedUpdate() {

        if(!networkTransformReceive.IsPositionSynced()) {
            /* pause script during position sync */
            return;
        }

        if(destination != null && destination != Vector3.zero) {
            SetMoveDirectionToDestination();
        }

        Vector3 ajustedDirection = direction * speed + Vector3.down * gravity;
        characterController.Move(ajustedDirection * Time.deltaTime);
    }

    public void SetDestination(Vector3 destination, float speed) {
        this.speed = speed;
        this.destination = destination;
        animationReceive.SetFloat("Speed", speed);
    }

    public void SetMoveDirectionToDestination() {
        Vector3 transformFlat = VectorUtils.To2D(transform.position);
        Vector3 destinationFlat = VectorUtils.To2D(destination);

        if(Vector3.Distance(transformFlat, destinationFlat) > 0.1f) {
            networkTransformReceive.PausePositionSync();
            direction = (destinationFlat - transformFlat).normalized;
        } else {
            direction = Vector3.zero;
            networkTransformReceive.ResumePositionSync();
        }
    }

}
