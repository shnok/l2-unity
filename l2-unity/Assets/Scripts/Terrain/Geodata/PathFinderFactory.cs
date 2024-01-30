using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

//This class controls the threads
public class PathFinderFactory : MonoBehaviour {
    //Singleton
    private static PathFinderFactory instance;
    void Awake() {
        instance = this;
    }
    public static PathFinderFactory GetInstance() {
        return instance;
    }

    //The maximum simultaneous threads we allow to open
    public int MaxJobs = 3;

    //Delegates are a variable that points to a function
    public delegate void PathfindingJobComplete(List<Node> path);

    private List<PathFinder> currentJobs;
    private List<PathFinder> todoJobs;

    void Start() {
        currentJobs = new List<PathFinder>();
        todoJobs = new List<PathFinder>();
    }

    void Update() {
        /*
         * Another way to keep track of the threads we have open would have been to create 
         * a new thread for it but we can also just use Unity's main thread too since this class
         * derives from monoBehaviour
         */

        int i = 0;

        while(i < currentJobs.Count) {
            if(currentJobs[i].jobDone) {
                currentJobs[i].NotifyComplete();
                currentJobs.RemoveAt(i);
            } else {
                i++;
            }
        }

        if(todoJobs.Count > 0 && currentJobs.Count < MaxJobs) {
            PathFinder job = todoJobs[0];
            todoJobs.RemoveAt(0);
            currentJobs.Add(job);

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
        PathFinder newJob = new PathFinder(start, target, completeCallback);
        todoJobs.Add(newJob);
    }

    public List<Node> SmoothPath(List<Node> path) {
        List<Node> waypoints = new List<Node>();

        int currentNode = 0;
        //waypoints.Add(path[0]);

        for(int i = 0; i < path.Count; i++) {
            Vector3 origin = path[currentNode].center;
            Vector3 destination = path[i].center;
            Vector3 yOffset = Vector3.up * Geodata.GetInstance().nodeSize * 1.5f;
            bool cantSeeTarget = Physics.Linecast(destination + yOffset, origin + yOffset, Geodata.GetInstance().GetMask());

            if(cantSeeTarget) {
                waypoints.Add(path[i - 1]);
                currentNode = i - 1;
            }
        }

        waypoints.Add(path[path.Count - 1]);
        return waypoints;
    }
}
