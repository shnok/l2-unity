using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string _defaultScene = "Game";
    [SerializeField] private List<string> _mapList = new List<string>();
    private int _totalLoadedScenes = 0;

    public static SceneLoader _instance;
    public static SceneLoader Instance { get { return _instance; } }

    void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    void Start() {
        LoadDefaultScene();
    }

    public void LoadDefaultScene() {
        Debug.Log("Loaded initial scene " + _defaultScene + ". Load count: " + ++_totalLoadedScenes);
        if(SceneManager.GetActiveScene().name != _defaultScene) {
            Debug.Log("Loading default scene " + _defaultScene);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_defaultScene);
            asyncLoad.completed += (AsyncOperation operation) => OnSceneSwitch(operation, _defaultScene);
        } else {
            Debug.Log("Skipping default scene load " + _defaultScene);
            OnDefaultSceneLoad(null, _defaultScene);
        }
    }

    private void OnDefaultSceneLoad(AsyncOperation operation, string sceneName) {
        if(!World.Instance.OfflineMode) {
            DefaultClient.Instance.Connect(StringUtils.GenerateRandomString());
        } else {
            SwitchScene("Game");
        }
    }

    public void SwitchScene(string sceneName) {
        if(SceneManager.GetActiveScene().name != sceneName) {
            Debug.Log("Switching to scene " + sceneName);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
            asyncLoad.completed += (AsyncOperation operation) => OnSceneSwitch(operation, sceneName);
        } else {
            Debug.Log("Skipping scene load " + sceneName);
            OnSceneSwitch(null, sceneName);
        }
    }

    private void OnSceneSwitch(AsyncOperation operation, string sceneName) {
        Debug.Log("Switched to scene " + sceneName + ". Load count: " + _totalLoadedScenes);
        if(sceneName == "Game") {
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
