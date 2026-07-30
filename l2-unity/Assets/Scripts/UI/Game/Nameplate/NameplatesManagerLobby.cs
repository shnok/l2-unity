using UnityEngine;
using UnityEngine.UIElements;

public class NameplatesManagerLobby : NameplatesManagerBase
{
    private static NameplatesManagerLobby instance;
    public static NameplatesManagerLobby Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Initialize();
    }

    private void FixedUpdate()
    {
        if (!IsSystemReady())
        {
            ClearNameplates();
            return;
        }

        ScanForEntities();
        ProcessNameplateVisibility();
    }

    protected override bool IsSystemReady()
    {
        if (!L2LoginUI.Instance.UILoaded) return false;

        if (mainCamera == null)
        {
            return false;
        }

        if (rootElement == null)
        {
            rootElement = L2LoginUI.Instance.RootElement.Q<VisualElement>("NameplatesContainer");
            return false;
        }

        if (playerTransform == null)
        {
            playerTransform = mainCamera.transform;
        }

        return playerTransform != null;
    }

    protected override void ScanForEntities()
    {
        base.ScanForEntities();
    }

    protected override void UpdateNameplateStyle(Nameplate nameplate)
    {
        base.UpdateNameplateStyle(nameplate);

        SelectableCharacterEntity e = (SelectableCharacterEntity)nameplate.Entity;

        int deleteTimer = e.CharacterInfo.DeleteTimer;
        if (deleteTimer > 0)
        {
            ((LobbyNameplate)nameplate).ShowDeleteTimer();
            ((LobbyNameplate)nameplate).UpdateTimer(deleteTimer);
        }
        else
        {
            ((LobbyNameplate)nameplate).HideDeleteTimer();
        }
    }

    protected override Nameplate CreateNameplate(Entity entity)
    {
        VisualElement element = nameplateTemplate.Instantiate()[0];
        Nameplate nameplate = new LobbyNameplate(
            element,
            element.Q<Label>("EntityName"),
            element.Q<Label>("EntityTitle"),
            entity
        );

        rootElement.Add(element);
        return nameplate;
    }

    private void OnDestroy()
    {
        nameplates.Clear();
        instance = null;
    }

    public void SetActiveCamera(Camera camera)
    {
        if (camera == null)
        {
            playerTransform = null;
            mainCamera = null;
            return;
        }

        playerTransform = camera.transform;
        mainCamera = camera;
    }
}
