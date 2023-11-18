using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToMoveController : MonoBehaviour {

    private static ClickToMoveController _instance;
    public static ClickToMoveController GetInstance() {
        return _instance;
    }

    public Node targetNode;
    public bool debugPathFinder;

    private void Awake() {
        _instance = this;
    }

    void Start() {

    }

    public void MoveTo(TargetObjectData target, Vector3 clickPosition) {
        Node node = Geodata.GetInstance().GetNodeAt(target.targetScene, clickPosition);
        if(node != null) {
            targetNode = node;
        } else {
            targetNode = null;
        }
    }

    void OnDrawGizmos() {
        if(!debugPathFinder || !Application.isPlaying)
            return;

        if(targetNode != null) {
            float nodeSize = Geodata.GetInstance().nodeSize;
            Vector3 cubeSize = new Vector3(nodeSize - nodeSize / 10f, 0.1f, nodeSize - nodeSize / 10f);
            if(targetNode.walkable) {
                Gizmos.color = Color.yellow;
            } else {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawCube(targetNode.center, cubeSize);
        }
    }
}
