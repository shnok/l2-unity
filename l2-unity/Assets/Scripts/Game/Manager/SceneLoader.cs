using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string _gameSceme = "Game";
    [SerializeField] private List<string> _mapList = new List<string>();
    private int _totalLoadedScenes = 0;

    public string GameScene { get { return _gameSceme; } }

    public static SceneLoader _instance;
    public static SceneLoader Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    void OnDestroy() {
        _instance = null;
    }

    void Start() {
        LoadDefaultScene();
    }

    public void LoadDefaultScene() {
        Debug.Log("Loaded initial scene " + _gameSceme + ". Load count: " + ++_totalLoadedScenes);
        if(SceneManager.GetActiveScene().name != _gameSceme) {
            Debug.Log("Loading default scene " + _gameSceme);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_gameSceme);
            asyncLoad.completed += (AsyncOperation operation) => OnSceneSwitch(operation, _gameSceme);
        } else {
            Debug.Log("Skipping default scene load " + _gameSceme);
            OnDefaultSceneLoad(null, _gameSceme);
        }
    }

    private void OnDefaultSceneLoad(AsyncOperation operation, string sceneName) {
        if(!World.Instance.OfflineMode) {
            DefaultClient.Instance.Connect(StringUtils.GenerateRandomString());
        } else {
            SwitchScene(_gameSceme);
        }
    }

    public void SwitchScene(string sceneName) {
        if(SceneManager.GetActiveScene().name != sceneName) {
            Debug.Log("Switching to scene " + sceneName);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.completed += (AsyncOperation operation) => OnSceneSwitch(operation, sceneName);
        } else {
            Debug.Log("Skipping scene load " + sceneName);
            OnSceneSwitch(null, sceneName);
        }
    }

    private void OnSceneSwitch(AsyncOperation operation, string sceneName) {
        Debug.Log("Switched to scene " + sceneName + ". Load count: " + _totalLoadedScenes);
        if(sceneName == _gameSceme) {
            for(int i = 0; i < _mapList.Count; i++) {
                LoadScene(_mapList[i], true);
            }
        }
    }

    private void LoadScene(string sceneName, bool initialLoad) {
        Debug.Log("Loading scene " + sceneName);

        if(!SceneManager.GetSceneByName(sceneName).IsValid()) {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if(initialLoad) {
                asyncLoad.completed += (AsyncOperation operation) => OnInitialWorldload(operation, sceneName);
            } else {
                asyncLoad.completed += (AsyncOperation operation) => OnSceneLoad(operation, sceneName);
            }
        } else {
            if(initialLoad) {
                OnInitialWorldload(null, sceneName);
            } else {
                OnSceneLoad(null, sceneName);
            }
        }
    }

    private void OnInitialWorldload(AsyncOperation operation, string sceneName) {
        Debug.Log("Initial scene " + sceneName + " loaded. " + "Load count: " + ++_totalLoadedScenes);

        if(_totalLoadedScenes > _mapList.Count) {
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame() {
        // TODO wait for every scripts to be loaded.
        yield return new WaitForSeconds(1f);

        if(!World.Instance.OfflineMode) {
            DefaultClient.Instance.OnWorldSceneLoaded();
        } else {
            Debug.Log("Spawn player");
            World.Instance.SpawnPlayerOfflineMode();
        }
    }

    private void OnSceneLoad(AsyncOperation operation, string sceneName) {
        Debug.Log("Scene " + sceneName + " loaded. " + "Load count: " + ++_totalLoadedScenes);
    }

    public void LoadMap(string mapToLoad) {
        LoadScene(mapToLoad, false);
    }
}
