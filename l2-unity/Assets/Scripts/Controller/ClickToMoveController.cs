using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToMoveController : MonoBehaviour {

    private static ClickToMoveController instance;
    public static ClickToMoveController GetInstance() {
        return instance;
    }

    public Node startNode;
    public Node targetNode;
    public Vector3 targetDestination;
    public float destinationThreshold = 0.10f;
    public List<Node> path;
    public ObjectData collidingWith;
    public LayerMask ignoredLayers;
    public CharacterController characterController;
    public bool grounded;
    public bool debugPathFinder;

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
    }

    void Start() {
        characterController = GetComponent<CharacterController>();
    }

    public void Update() {
        // Update initial node
        if(characterController.isGrounded && collidingWith != null) {
            startNode = Geodata.GetInstance().GetNodeAt(collidingWith.objectScene, transform.position);
        }

        // Reset path when user input
        if(InputManager.GetInstance().IsInputPressed(InputType.Move)) {
            path.Clear();
        }


    }

    public void FixedUpdate() {
        if(path.Count > 0) {
            Vector3 flatTransformPos = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 flatDestPos;
            if(path.Count > 1) {
                flatDestPos = new Vector3(path[0].center.x, 0, path[0].center.z);
            } else {
                // Check distance with clicked point instead
                flatDestPos = new Vector3(targetDestination.x, 0, targetDestination.z);
            }

            if(Vector3.Distance(flatDestPos, flatTransformPos) < destinationThreshold) {
                path.RemoveAt(0);
            }

            if(path.Count > 0) {
                PlayerController.GetInstance().SetTargetPosition(flatDestPos, destinationThreshold);
            } else {
                PlayerController.GetInstance().ResetTargetPosition();
            }
        }
    }

    public void MoveTo(ObjectData target, Vector3 clickPosition) {
        targetDestination = clickPosition;

        Node node = Geodata.GetInstance().GetNodeAt(target.objectScene, clickPosition);
        if(node != null && startNode != null) {
            targetNode = node;

            PathFinderFactory.GetInstance().RequestPathfind(startNode, targetNode, (callback) => {
                Debug.Log("Found path with " + callback.Count + " node(s).");
                if(callback.Count == 0) {
                    PlayerController.GetInstance().SetTargetPosition(targetDestination, destinationThreshold);
                } else {
                    path = PathFinderFactory.GetInstance().SmoothPath(callback);
                }
            });

        } else {
            targetNode = null;
            PlayerController.GetInstance().SetTargetPosition(targetDestination, destinationThreshold);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        if(ignoredLayers != (ignoredLayers | (1 << hit.gameObject.layer))) {
            collidingWith = new ObjectData(hit.gameObject);
        } else {
            collidingWith = null;
        }
    }

    void OnDrawGizmos() {
        if(!debugPathFinder || !Application.isPlaying)
            return;

        float nodeSize = Geodata.GetInstance().nodeSize;
        Vector3 cubeSize = new Vector3(nodeSize - nodeSize / 10f, 0.1f, nodeSize - nodeSize / 10f);

        Gizmos.color = Color.yellow;

        if(targetNode != null) {
            Gizmos.DrawCube(targetNode.center, cubeSize);
        }

        Gizmos.color = Color.green;
        if(startNode != null) {
            Gizmos.DrawCube(startNode.center, cubeSize);
        }

        Gizmos.color = Color.white;

        if(path != null && path.Count > 0) {
            foreach(var node in path) {
                Gizmos.DrawCube(node.center, cubeSize);
            }
        }
    }
}
