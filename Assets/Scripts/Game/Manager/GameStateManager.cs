using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void OnStateChangeHandler();

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private GameState state = GameState.MENU;
    public event OnStateChangeHandler StateChanged;
    private EventProcessor eventProcessor;
    public bool singleScene = true;
    public bool offlineMode = true;

    public static GameStateManager instance;
    public static GameStateManager GetInstance() {
        return instance;
    }
    void Awake() {
        instance = this;
        eventProcessor = EventProcessor.GetInstance();
        StateChanged += HandleEvent;
    }

    private void Start() {
        if(offlineMode) {
            SetState(GameState.CONNECTED);
        } else if(singleScene) {
            StartCoroutine(ConnectToServer());
        }
    }

    IEnumerator ConnectToServer() {
        yield return new WaitForSeconds(2f);
        DefaultClient.GetInstance().Connect(StringUtils.GenerateRandomString());
    }

    private void HandleEvent() {
        eventProcessor.QueueEvent(HandleOnStateChange);
    }

    private void HandleOnStateChange() {
        if(state == GameState.MENU) {
            StartCoroutine(LoadAsyncScene("LoginMenuScene"));
        }
        if(state == GameState.CONNECTED) {
            StartCoroutine(LoadAsyncScene("Game"));
        }
    }

    IEnumerator LoadAsyncScene(string scene) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads 
        while(!asyncLoad.isDone) {
            yield return null;
        }

        OnSceneLoaded();
    }

    public void OnSceneLoaded() {
        if(offlineMode) {
            if(state == GameState.CONNECTED) {
                World.GetInstance().InstantiatePlayerOfflineMode();
            }
        } else {
            if(state == GameState.CONNECTED) {
                DefaultClient.GetInstance().OnWorldSceneLoaded();
            }

            if(state == GameState.MENU) {
                DefaultClient.GetInstance().OnDisconnectReady();
            }
        }
    }

    public void SetState(GameState ns) {
        state = ns;
        if(!singleScene) {
            StateChanged?.Invoke();
        } else {
            OnSceneLoaded();
        }
    }

    public GameState GetState() {
        return state;
    }
}