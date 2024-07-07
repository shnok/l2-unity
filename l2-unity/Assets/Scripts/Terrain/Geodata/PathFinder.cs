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
            _foundPath = SearchByClosest(_startPosition, _endPosition);
        } catch (Exception e) {
            Debug.LogWarning("Error while finding path: " + e.Message);
        }

        jobDone = true;
    }

    public void NotifyComplete() {
        if(_completeCallback != null) {
            _completeCallback(_foundPath);
        }
    }

    public List<Node> SearchByClosest(Node start, Node end)
    {
        // List of Visited Nodes
        List<Node> visitedNodes = new List<Node>();

        // List of Nodes to Visit
        List<Node> nodesToVisit = new List<Node>();
        nodesToVisit.Add(start);

        bool added;

        int i = 0;
        while (i++ < _maximumLoopCount) {
            Node node;

            // Get and remove node from the nodesToVisit list
            try {
                node = nodesToVisit[0];
                nodesToVisit.RemoveAt(0);
            } catch (Exception) {
                // No Path found
                Debug.Log($"No path found - {start.center} to {end.center}.");
                return null;
            }

            // Current node is the destination node
            // Path was found
            if (node.nodeIndex.Equals(end.nodeIndex)) {
               // Debug.Log($"Found path - {start.center} to {end.center} after {i} iteration(s).");
                return ConstructPathFull(node);
            }

            // Add node to visited nodes
            visitedNodes.Add(node);

            Node[] neighbors = GetNeighbours(node.center);
            if (neighbors == null) {
                continue;
            }

            //node.setNeighbors(neighbors);

            // Iterate through node's neighbors
            foreach (Node neighbor in neighbors) {
                if (neighbor == null) {
                    continue;
                }

                // check if neighbor was visited and needs to be visited
                if (!visitedNodes.Contains(neighbor) && !nodesToVisit.Contains(neighbor)) {

                    // Calculate neighbor node cost
                    neighbor.parentNode = new Node(node);
                    neighbor.cost = Vector3.Distance(neighbor.center, end.center);


                    // insert neighbor into the nodes to visit based on its cost
                    added = false;
                    for (int index = 0; index < nodesToVisit.Count; index++) {
                        if (neighbor.cost < nodesToVisit[index].cost) {
                            nodesToVisit.Insert(index, neighbor);
                            added = true;
                            break;
                        }
                    }

                    // add neighbor at the end of the nodes to visit list
                    if (!added) {
                        nodesToVisit.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    public List<Node> ConstructPathFull(Node node) {
        List<Node> path = new List<Node>();
        while (node.parentNode != null) {
            path.Insert(0, node);
            node = node.parentNode;
        }
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