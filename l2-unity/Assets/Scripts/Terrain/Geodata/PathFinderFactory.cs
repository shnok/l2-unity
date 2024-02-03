using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

//This class controls the threads
public class PathFinderFactory : MonoBehaviour {

    //Delegates are a variable that points to a function
    public delegate void PathfindingJobComplete(List<Node> path);

    //The maximum simultaneous threads we allow to open
    [SerializeField] private int _maxJobs = 3;

    private List<PathFinder> _currentJobs;
    private List<PathFinder> _todoJobs;
    private float _nodeSize = 0.5f;

    private static PathFinderFactory _instance;
    public static PathFinderFactory Instance { get { return _instance; } }
    void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    void Start() {
        _currentJobs = new List<PathFinder>();
        _todoJobs = new List<PathFinder>();
        _nodeSize = Geodata.Instance.NodeSize;
    }

    void Update() {
        /*
         * Another way to keep track of the threads we have open would have been to create 
         * a new thread for it but we can also just use Unity's main thread too since this class
         * derives from monoBehaviour
         */

        int i = 0;

        while(i < _currentJobs.Count) {
            if(_currentJobs[i].JobDone) {
                _currentJobs[i].NotifyComplete();
                _currentJobs.RemoveAt(i);
            } else {
                i++;
            }
        }

        if(_todoJobs.Count > 0 && _currentJobs.Count < _maxJobs) {
            PathFinder job = _todoJobs[0];
            _todoJobs.RemoveAt(0);
            _currentJobs.Add(job);

            //Start a new thread

            Thread jobThread = new Thread(job.FindPath);
            jobThread.Start();

            //As per the doc
            //https://msdn.microsoft.com/en-us/library/system.threading.thread(v=vs.110).aspx
            //It is not necessary to retain a reference to a Thread object once you have started the thread. 
            //The thread continues to execute until the thread procedure is complete.				
        }
    }

    public void RequestPathfind(Node start, Node target, PathfindingJobComplete completeCallback) {
        PathFinder newJob = new PathFinder(start, target, completeCallback, _nodeSize);
        _todoJobs.Add(newJob);
    }

    public List<Node> SmoothPath(List<Node> path) {
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
