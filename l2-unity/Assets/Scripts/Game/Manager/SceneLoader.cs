using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string _menuScene = "Menu";
    [SerializeField] private string _gameScene = "Game";
    [SerializeField] private List<string> _mapList = new List<string>();
    private int _totalLoadedScenes = 0;

    public string GameScene { get { return _gameScene; } }

    public static SceneLoader _instance;
    public static SceneLoader Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    public void LoadMenu() {
        SwitchScene(_menuScene, ((AsyncOperation operation) => {
            for (int i = 0; i < _mapList.Count; i++) {
                UnloaScene(_mapList[i]);
            }
        }));
    }

    public void LoadGame() {
        SwitchScene(_gameScene, ((AsyncOperation operation) => {
            for (int i = 0; i < _mapList.Count; i++) {
                LoadScene(_mapList[i], true);
            }
        }));
    }

    public void SwitchScene(string sceneName, Action<AsyncOperation> p) {
        if(SceneManager.GetActiveScene().name != sceneName) {
            Debug.Log("Switching to scene " + sceneName);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.completed += p;
        } else {
            Debug.Log("Skipping scene load " + sceneName);
            AsyncOperation dummyOperation = new AsyncOperation();
            p.Invoke(dummyOperation);
        }
    }

    private void LoadScene(string sceneName, bool initialLoad) {
        Debug.Log("Loading scene " + sceneName);

        // Does the scene need to be loaded ?
        if (!SceneManager.GetSceneByName(sceneName).IsValid()) {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if(initialLoad) {
                // Load world at startup
                asyncLoad.completed += (AsyncOperation operation) => OnInitialWorldload(operation, sceneName);
            } else {
                // Loading l2 maps at runtime
                asyncLoad.completed += (AsyncOperation operation) => OnSceneLoad(operation, sceneName);
            }
        } else {
            // Scene already loaded
            if(initialLoad) {
                // Load world at startup
                OnInitialWorldload(null, sceneName);
            } else {
                // Loading l2 maps at runtime
                OnSceneLoad(null, sceneName);
            }
        }
    }

    private void UnloaScene(string sceneName) {
        Debug.Log("Unoading scene " + sceneName);

        if (!SceneManager.GetSceneByName(sceneName).IsValid()) {
            return;
        } else {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }

    private void OnInitialWorldload(AsyncOperation operation, string sceneName) {
        Debug.Log("Initial scene " + sceneName + " loaded. " + "Load count: " + ++_totalLoadedScenes);

        if(_totalLoadedScenes >= _mapList.Count) {
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame() {
        yield return new WaitForSeconds(3f);

        Debug.LogWarning("All scenes loaded, sending LoadWorld packet.");

        if (World.Instance != null && !World.Instance.OfflineMode) {
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
