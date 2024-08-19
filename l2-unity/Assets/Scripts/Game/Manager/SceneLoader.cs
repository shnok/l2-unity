using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string _menuScene = "Menu";
    [SerializeField] private string _lobbyScene = "l2_lobby";
    [SerializeField] private string _gameScene = "Game";
    [SerializeField] private List<SceneListObject> _mapList = new List<SceneListObject>();
    [SerializeField] private List<string> _mapsToLoad = new List<string>();
    private int _totalLoadedScenes = 0;

    public string GameScene { get { return _gameScene; } }

    public static SceneLoader _instance;
    public static SceneLoader Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }

        FillMapsToLoadList();
    }

    private void FillMapsToLoadList()
    {
        _mapsToLoad = new List<string>();
        for (int i = 0; i < _mapList.Count; i++)
        {
            var map = _mapList[i];

            if (!map.enabled)
            {
                continue;
            }

            _mapsToLoad.Add(map.name);
        }
    }

    public void LoadMenu()
    {
        GameManager.Instance.OnStartingGame();
        SwitchScene(_menuScene, (AsyncOperation o) =>
        {
            GameManager.Instance.StartLoading();

            LoadScene(_lobbyScene, (AsyncOperation operation) =>
            {
                GameManager.Instance.OnGameLaunched();
            });
        });
    }

    public void LoadGame()
    {
        _totalLoadedScenes = 0;
        SwitchScene(_gameScene, ((AsyncOperation o) =>
        {
            GameManager.Instance.OnWorldLoading();
            for (int i = 0; i < _mapsToLoad.Count; i++)
            {
                var map = _mapsToLoad[i];

                LoadScene(map, (AsyncOperation operation) =>
                {
                    OnInitialWorldload(operation, map);
                });
            }
        }));
    }

    public void SwitchScene(string sceneName, Action<AsyncOperation> p)
    {
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            Debug.Log("Switching to scene " + sceneName);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.completed += p;
        }
        else
        {
            Debug.Log("Skipping scene switch " + sceneName);
            AsyncOperation dummyOperation = new AsyncOperation();
            p.Invoke(dummyOperation);
        }
    }

    private void LoadScene(string sceneName, Action<AsyncOperation> p)
    {
        Debug.Log("Loading scene " + sceneName);

        // Does the scene need to be loaded ?
        if (!SceneManager.GetSceneByName(sceneName).IsValid())
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncLoad.completed += p;
        }
        else
        {
            Debug.Log("Skipping scene load " + sceneName);
            AsyncOperation dummyOperation = new AsyncOperation();
            p.Invoke(dummyOperation);
        }
    }

    private void UnloadScene(string sceneName)
    {
        Debug.Log("Unoading scene " + sceneName);

        if (!SceneManager.GetSceneByName(sceneName).IsValid())
        {
            return;
        }
        else
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }

    private void OnInitialWorldload(AsyncOperation operation, string sceneName)
    {
        Debug.Log("Initial scene " + sceneName + " loaded. " + "Load count: " + ++_totalLoadedScenes);

        if (_totalLoadedScenes >= _mapsToLoad.Count)
        {
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(.3f); //TODO: wait for everything to be loaded instead of waitforseconds

        Debug.LogWarning("All scenes loaded, sending LoadWorld packet.");

        if (World.Instance != null && !World.Instance.OfflineMode)
        {
            GameManager.Instance.OnWorldSceneLoaded();
        }
        else
        {
            Debug.Log("Spawn player");
            World.Instance.SpawnPlayerOfflineMode();
        }
    }
}
