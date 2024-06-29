using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static ServerListPacket;

public class ServerSelectWindow : MonoBehaviour
{
    private VisualTreeAsset _windowTemplate;
    private VisualTreeAsset _serverElementTemplate;
    private VisualElement _windowEle;
    private VisualElement _listContentContainer;
    private List<VisualElement> _serverElements;
    private List<ServerData> _serverData;
    private int _selectedServerId = 0;

    private static ServerSelectWindow _instance;
    public static ServerSelectWindow Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    private void OnDestroy() {
        _instance = null;
    }

    void Start() {
        LoadAssets();
    }

    private void LoadAssets() {
        if (_windowTemplate == null) {
            _windowTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Login/ServerListWindow");
        }
        if (_windowTemplate == null) {
            Debug.LogError("Could not load ServerListWindow template.");
        }
        if (_serverElementTemplate == null) {
            _serverElementTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Login/ServerElement");
        }
        if (_serverElementTemplate == null) {
            Debug.LogError("Could not load ServerElement template.");
        }
    }

    public void AddWindow(VisualElement root) {
        if (_windowTemplate == null) {
            return;
        }
        StartCoroutine(BuildWindow(root));
    }

    IEnumerator BuildWindow(VisualElement root) {
        _windowEle = _windowTemplate.Instantiate()[0];

        Button confirmButton = _windowEle.Q<Button>("ConfirmButton");
        confirmButton.AddManipulator(new ButtonClickSoundManipulator(confirmButton));
        confirmButton.RegisterCallback<ClickEvent>(evt => ConfirmButtonPressed());

        Button cancelButton = _windowEle.Q<Button>("CancelButton");
        cancelButton.AddManipulator(new ButtonClickSoundManipulator(cancelButton));
        cancelButton.RegisterCallback<ClickEvent>(evt => CancelButtonPressed());

        Button nameButton = _windowEle.Q<Button>("Name");
        nameButton.AddManipulator(new ButtonClickSoundManipulator(nameButton));
        Button trafficButton = _windowEle.Q<Button>("Traffic");
        trafficButton.AddManipulator(new ButtonClickSoundManipulator(trafficButton));
        Button characterButton = _windowEle.Q<Button>("Character");
        characterButton.AddManipulator(new ButtonClickSoundManipulator(characterButton));

        _listContentContainer = _windowEle.Q<VisualElement>("ServerContainer");

        _serverElements = new List<VisualElement>();

        root.Add(_windowEle);

        yield return new WaitForEndOfFrame();
    }

    public void UpdateServerList(int lastServer, List<ServerData> serverData, Dictionary<int, int> charsOnServers) {
        _serverData = serverData;

        for (int i = 0; i < serverData.Count; i++) {
            charsOnServers.TryGetValue(serverData[i].serverId, out int charCount);

            AddServerRow(i, ParseServerName(serverData[i].serverId), ParseServerStatus(serverData[i].status), charCount);

            if (serverData[i].serverId == lastServer) {
                SelectServer(i);
            }
        }

        for(int i = serverData.Count; i < 20; i++) {
            AddServerRow(i, "", "", -1);
        }

        if(lastServer == 0) {
            SelectServer(0);
        }
    }

    public void SelectServer(int rowId) {
        for (int i = 0; i < _serverElements.Count; i++) {
            _serverElements[i].RemoveFromClassList("selected");
        }

        _serverElements[rowId].AddToClassList("selected");

        _selectedServerId = _serverData[rowId].serverId;

        Debug.Log("Server selected: " + _selectedServerId);
    }

    private string ParseServerName(int serverId) {
        return ServerNameDAO.GetServer(serverId);
    }

    private string ParseServerStatus(int status) {
        switch (status) {
            case 0: return "Light";
            case 1: return "Normal";
            case 2: return "Heavy";
            case 3: return "Full";
            case 4: return "Down";
            case 5: return "GM Only";
            default: return "Unknown";
        }
    }

    private string GetStatusClass(string status) {
        switch (status) {
            case "Light": return "light";
            case "Normal": return "normal";
            case "Heavy": return "heavy";
            case "Full":
            case "Down":
            case "GM Only": return "full";
            default: return "normal";
        }
    }

    private void AddServerRow(int id, string serverName, string status, int charCount) {
        VisualElement row = _serverElementTemplate.Instantiate()[0];
        Label serverNameLabel = row.Q<Label>("ServerName");
        Label serverStatusLabel = row.Q<Label>("ServerStatus");
        Label charCountLabel = row.Q<Label>("CharacterCount");

        serverStatusLabel.AddToClassList(GetStatusClass(status));

        serverNameLabel.text = serverName;
        serverStatusLabel.text = status;
        if(charCount >= 0) {
            charCountLabel.text = charCount.ToString();
        } else {
            charCountLabel.text = "";
        }

        if (id % 2 == 1) {
            row.AddToClassList("odd");
        }

        if(charCount >= 0) {
            int rowId = id;
            row.RegisterCallback<ClickEvent>((evt) => {
                SelectServer(rowId);
            });
            row.AddManipulator(new SlotClickSoundManipulator(row));
        }

        _listContentContainer.Add(row);

        _serverElements.Add(row);
    }

    private void ConfirmButtonPressed() {
    }

    private void CancelButtonPressed() {
        LoginClient.Instance.Disconnect();
    }

    public void HideWindow() {
        _windowEle.style.display = DisplayStyle.None;
    }

    public void ShowWindow() {
        _windowEle.style.display = DisplayStyle.Flex;
    }
}