using UnityEngine;
using UnityEngine.UIElements;

public class NameplatesManager2DLobby : NameplatesManager2DBase
{
    private static NameplatesManager2DLobby instance;
    public static NameplatesManager2DLobby Instance => instance;

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

    protected override void UpdateNameplateStyle(Nameplate2D nameplate)
    {
        base.UpdateNameplateStyle(nameplate);

        SelectableCharacterEntity e = (SelectableCharacterEntity)nameplate.Entity;

        int deleteTimer = e.CharacterInfo.DeleteTimer;
        if (deleteTimer > 0)
        {
            ((LobbyNameplate2D)nameplate).ShowDeleteTimer();
            ((LobbyNameplate2D)nameplate).UpdateTimer(deleteTimer);
        }
        else
        {
            ((LobbyNameplate2D)nameplate).HideDeleteTimer();
        }
    }

    protected override Nameplate2D CreateNameplate(Entity entity)
    {
        VisualElement element = nameplateTemplate.Instantiate()[0];
        Nameplate2D nameplate = new LobbyNameplate2D(
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
