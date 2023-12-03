using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string defaultScene = "Game";
    public List<string> mapList = new List<string>();
    public bool singleScene = true;

    public static SceneLoader instance;
    public static SceneLoader GetInstance() {
        return instance;
    }
    void Awake() {
        instance = this;
    }
    void Start() {
        LoadDefaultScene();
    }

    public void LoadDefaultScene() {
        if(SceneManager.GetActiveScene().name != defaultScene) {
            Debug.Log("Loading default scene " + defaultScene);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(defaultScene);
            asyncLoad.completed += (AsyncOperation operation) => OnSceneSwitch(operation, defaultScene);
        } else {
            Debug.Log("Skipping default scene load " + defaultScene);
            OnDefaultSceneLoad(null, defaultScene);
        }
    }

    private void OnDefaultSceneLoad(AsyncOperation operation, string sceneName) {
        Debug.Log("OnDefaultSceneLoad");
        if(!World.GetInstance().offlineMode) {
            DefaultClient.GetInstance().Connect(StringUtils.GenerateRandomString());
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
        Debug.Log("Switched to scene " + sceneName + ".");
        if(sceneName == "Game") {
            for(int i = 0; i < mapList.Count; i++) {
                LoadScene(mapList[i], true);
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
        Debug.Log("Initial scene " + sceneName + " loaded.");
        if(SceneManager.loadedSceneCount >= mapList.Count) {
            if(!World.GetInstance().offlineMode) {
                DefaultClient.GetInstance().OnWorldSceneLoaded();
            } else {
                Debug.Log("Spawn player");
                World.GetInstance().SpawnPlayerOfflineMode();
            }
        }
    }

    private void OnSceneLoad(AsyncOperation operation, string sceneName) {
        Debug.Log("Scene " + sceneName + " loaded.");
    }

    public void LoadMap(string mapToLoad) {
        LoadScene(mapToLoad, false);
    }
}
