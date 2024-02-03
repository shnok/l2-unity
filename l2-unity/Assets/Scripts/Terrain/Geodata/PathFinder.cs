using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    private PathFinderFactory.PathfindingJobComplete _completeCallback;
    private List<Node> _foundPath;
    private float _nodeSize;

    private Node _startPosition;
    private Node _endPosition;
    private int _maximumLoopCount;
    private volatile bool jobDone = false;

    public bool JobDone { get { return jobDone; } }

    public PathFinder(Node start, Node target, PathFinderFactory.PathfindingJobComplete callback, float nodeSize, int maximumLoopCount) {
        _startPosition = start;
        _endPosition = target;
        _completeCallback = callback;
        _nodeSize = nodeSize;
        _maximumLoopCount = maximumLoopCount;
    }

    public void FindPath() {
        try {
            _foundPath = FindPathActual(_startPosition, _endPosition);
        } catch(Exception e) {
            Debug.LogWarning("Error while finding path: " + e.Message);
        }

        jobDone = true;
    }

    public void NotifyComplete() {
        if(_completeCallback != null) {
            _completeCallback(_foundPath);
        }
    }

    private List<Node> FindPathActual(Node start, Node target) {
        //Typical A* algorythm from here and on

        List<Node> foundPath = new List<Node>();

        //We need two lists, one for the nodes we need to check and one for the nodes we've already checked
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        //We start adding to the open set
        openSet.Add(start);

        int currentAttemps = 0;
        while(openSet.Count > 0) {
            Node currentNode = openSet[0];

            for(int i = 0; i < openSet.Count; i++) {
                //We check the costs for the current node
                //You can have more opt. here but that's not important now
                if(openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost &&
                    openSet[i].hCost < currentNode.hCost)) {
                    //and then we assign a new current node
                    if(!currentNode.nodeIndex.Equals(openSet[i].nodeIndex)) {
                        currentNode = openSet[i];
                    }
                }
            }

            //we remove the current node from the open set and add to the closed set
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            //if the current node is the target node
            if(currentNode.nodeIndex.Equals(target.nodeIndex)) {
                //that means we reached our destination, so we are ready to retrace our path
                foundPath = RetracePath(start, currentNode);
                break;
            }

            if(currentAttemps++ > _maximumLoopCount) {
                Debug.LogWarning("Find path timeout");
                return foundPath;
            }

            //if we haven't reached our target, then we need to start looking the neighbours
            foreach(Node neighbour in GetNeighbours(currentNode.worldPosition)) {
                if(neighbour == null) {
                    continue;
                }

                if(!closedSet.Contains(neighbour)) {
                    //we create a new movement cost for our neighbours
                    float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                    //and if it's lower than the neighbour's cost
                    if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {

                        //we calculate the new costs
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, target);
                        //Assign the parent node
                        neighbour.parentNode = currentNode;
                        //And add the neighbour node to the open set
                        if(!openSet.Contains(neighbour)) {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }
        
        return foundPath;
    }

    private List<Node> RetracePath(Node startNode, Node endNode) {
        //Retrace the path, is basically going from the endNode to the startNode
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode) {
            path.Add(currentNode);
            //by taking the parentNodes we assigned
            currentNode = currentNode.parentNode;
        }

        //then we simply reverse the list
        path.Reverse();

        return path;
    }

    private Node[] GetNeighbours(Vector3 nodePos) {
        Node[] returnList = new Node[8];
        short i = 0;

        for(int x = -1; x <= 1; x++) {
            for(int z = -1; z <= 1; z++) {
                if(x == 0 && z == 0)
                    continue;

                try {
                    Vector3 neighborPos = new Vector3(nodePos.x + x * _nodeSize,
                            nodePos.y,
                            nodePos.z + z * _nodeSize);
                    string mapId = Geodata.Instance.GetCurrentMap(neighborPos);

                    Node node = null;
                    try {
                        node = Geodata.Instance.GetNodeAt(neighborPos, mapId);
                    } catch(Exception) { }

                    if(node == null) {
                        neighborPos.y = nodePos.y + _nodeSize;
                        try {
                            node = Geodata.Instance.GetNodeAt(neighborPos, mapId);
                        } catch(Exception) { }
                    }

                    if(node == null) {
                        neighborPos.y = nodePos.y - _nodeSize;
                        try {
                            node = Geodata.Instance.GetNodeAt(neighborPos, mapId);
                        } catch(Exception) { }
                    }

                    if(node != null) {
                        returnList[i++] = node;
                    }
                } catch(Exception) {
                    Debug.Log("Node neighbor could not be found.");
                }
            }
        }

        return returnList;
    }

    /*private float GetDistance(Node posA, Node posB) {
        return Vector3.Distance(posA.center, posB.center);
    }*/

    private float GetDistance(Node posA, Node posB) {
        //We find the distance between each node
        //not much to explain here

        float distX = Mathf.Abs(posA.worldPosition.x - posB.worldPosition.x);
        float distZ = Mathf.Abs(posA.worldPosition.z - posB.worldPosition.z);
        float distY = Mathf.Abs(posA.worldPosition.y - posB.worldPosition.y);

        if(distX > distZ) {
            return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
        }

        return 14 * distX + 10 * (distZ - distX) + 10 * distY;
    }

    public static List<Node> SmoothPath(List<Node> path) {
        List<Node> waypoints = new List<Node>();

        int currentNode = 0;
        //waypoints.Add(path[0]);

        for(int i = 0; i < path.Count; i++) {
            Vector3 origin = path[currentNode].center;
            Vector3 destination = path[i].center;
            Vector3 yOffset = Vector3.up * Geodata.Instance.NodeSize * 1.5f;
            bool cantSeeTarget = Physics.Linecast(destination + yOffset, origin + yOffset, Geodata.Instance.ObstacleMask);

            if(cantSeeTarget) {
                waypoints.Add(path[i - 1]);
                currentNode = i - 1;
            }
        }

        waypoints.Add(path[path.Count - 1]);
        return waypoints;
    }
}