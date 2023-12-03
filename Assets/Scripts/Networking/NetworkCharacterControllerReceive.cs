using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacterControllerReceive : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float speed;
    private Vector3 destination;
    [SerializeField] private float gravity = 28f;


    void Start() {
        characterController = GetComponent<CharacterController>();
        if(characterController == null || World.GetInstance().offlineMode) {
            this.enabled = false;
        }
        direction = Vector3.zero;
        destination = transform.position;
    }

    void Update() {
        Vector3 ajustedDirection = direction * speed + Vector3.down * gravity;
        characterController.Move(ajustedDirection * Time.deltaTime);
    }

    public void UpdateMoveDirection(float speed, Vector3 direction) {
        this.speed = speed;
        this.direction = direction;
    }

    public void SetDestination(Vector3 destination) {
        this.destination = destination;
    }

    public void MoveToPosition() {
        Vector3 transformFlat = VectorUtils.To2D(transform.position);
        Vector3 destinationFlat = VectorUtils.To2D(destination);

        Vector3 direction = Vector3.zero;
        if(Vector3.Distance(transformFlat, destinationFlat) > 0.1f) {
            direction = transformFlat - destinationFlat;
        }

        direction = direction.normalized;
        direction.y = -10;
    }

}
