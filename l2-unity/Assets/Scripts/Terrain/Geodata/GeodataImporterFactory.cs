using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GeodataImporterFactory : MonoBehaviour {
    //Delegates are a variable that points to a function
    public delegate void ImportJobComplete(Node[,,] geodata);

    //The maximum simultaneous threads we allow to open
    [SerializeField] private int _maxJobs = 16;

    private List<GeodataImporter> _currentJobs;
    private List<GeodataImporter> _todoJobs;

    private static GeodataImporterFactory _instance;
    public static GeodataImporterFactory Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }

        _currentJobs = new List<GeodataImporter>();
        _todoJobs = new List<GeodataImporter>();
    }

    void OnDestroy() {
        _instance = null;
    }

    void Update() {
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
            GeodataImporter job = _todoJobs[0];
            _todoJobs.RemoveAt(0);
            _currentJobs.Add(job);

            Thread jobThread = new Thread(job.ImportGeodata);
            jobThread.Start();
        }
    }

    public void RequestImportGeodata(string mapId, float nodeSize, byte maximumLayerCount, Vector3 origin, ImportJobComplete completeCallback) {
        GeodataImporter newJob = new GeodataImporter(mapId, nodeSize, maximumLayerCount, origin, completeCallback);
        if(newJob != null) {
            _todoJobs.Add(newJob);
        } else {
            Debug.LogWarning("New job is null");
        }
    }
}