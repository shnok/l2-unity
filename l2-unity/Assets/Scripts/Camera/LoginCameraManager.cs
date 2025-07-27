using System.Collections.Generic;
using UnityEngine;

public class LoginCameraManager : MonoBehaviour
{
    private bool _initialized = false;
    private Dictionary<string, Camera> cameras = new Dictionary<string, Camera>();

    private Camera _activeCamera;

    private static LoginCameraManager _instance;
    public static LoginCameraManager Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }

        if (!_initialized)
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        _initialized = true;

        cameras.Add("Login", GameObject.Find("Login").GetComponent<Camera>());
        cameras.Add("CharSelect", GameObject.Find("CharSelect").GetComponent<Camera>());
        cameras.Add(SysStringTable.Instance.GetSysString(172).Name, GameObject.Find("DarkElf").GetComponent<Camera>());
        cameras.Add(SysStringTable.Instance.GetSysString(173).Name, GameObject.Find("Orc").GetComponent<Camera>());
        cameras.Add(SysStringTable.Instance.GetSysString(174).Name, GameObject.Find("Dwarf").GetComponent<Camera>());
        cameras.Add(SysStringTable.Instance.GetSysString(171).Name, GameObject.Find("Elf").GetComponent<Camera>());
        cameras.Add(SysStringTable.Instance.GetSysString(170).Name, GameObject.Find("Human").GetComponent<Camera>());

        DisableCameras();

        // GameManager.Instance.OnLoginCamerasInitialized();

        // GameManager.Instance.NotifyEvent(GameEvent.LOADING_COMPLETE);
    }

    public Camera SelectClassCamera(string race, int charClass)
    {
        if (cameras.TryGetValue(race, out Camera camera))
        {
            if (charClass == 0)
            {
                return camera.transform.GetChild(0).GetComponent<Camera>();
            }

            return camera.transform.GetChild(1).GetComponent<Camera>();
        }


        return null;
    }

    public Camera SelectGenderCamera(string race, int charClass, int gender)
    {
        Camera classCamera = SelectClassCamera(race, charClass);
        if (classCamera != null)
        {
            if (gender == 0)
            {
                return classCamera.transform.GetChild(0).GetComponent<Camera>();
            }

            return classCamera.transform.GetChild(1).GetComponent<Camera>();
        }

        return null;
    }

    public Camera SelectHeadCamera(string race, int charClass, int gender)
    {
        Camera genderCamera = SelectGenderCamera(race, charClass, gender);
        if (genderCamera != null)
        {
            return genderCamera.transform.GetChild(0).GetComponent<Camera>();
        }

        return null;
    }

    public void SwitchCamera(Camera camera)
    {
        if (!_initialized)
        {
            Initialize();
        }

        DisableMainCamera();

        camera.enabled = true;
        _activeCamera = camera;

        UpdateListenerPosition();
    }

    public void SwitchCamera(string camera)
    {
        if (!_initialized)
        {
            Initialize();
        }

        Debug.Log("Switch Camera: " + camera);
        DisableMainCamera();

        if (cameras.TryGetValue(camera, out Camera obj))
        {
            Debug.Log(camera + " camera enabled.");
            obj.enabled = true;
            _activeCamera = obj;

            if (camera == "CharSelect")
            {
                NameplatesManagerLobby.Instance.SetActiveCamera(obj);
                CharacterSelector.Instance.Camera = obj;
            }
            else
            {
                NameplatesManagerLobby.Instance.SetActiveCamera(null);
                CharacterSelector.Instance.Camera = null;
            }

            UpdateListenerPosition();
        }
    }

    public void DisableCameras()
    {
        if (!_initialized)
        {
            Initialize();
        }

        foreach (var cam in cameras.Values)
        {
            cam.enabled = false;
        }
    }

    public void DisableMainCamera()
    {
        if (!_initialized)
        {
            Initialize();
        }

        if (Camera.main != null)
        {
            // Debug.Log("Disabling camera " + Camera.main.transform);
            Camera.main.enabled = false;
        }
        else if (_activeCamera != null)
        {
            _activeCamera.enabled = false;
        }
    }

    private void UpdateListenerPosition()
    {
        ThirdPersonListener.Instance.transform.position = _activeCamera.transform.position;
        ThirdPersonListener.Instance.Cam = _activeCamera.gameObject;
    }

    public void ZoomIn()
    {
        SwitchCamera(_activeCamera.transform.GetChild(0).GetComponent<Camera>());
    }

    public void ZoomOut()
    {
        SwitchCamera(_activeCamera.transform.parent.GetComponent<Camera>());
    }
}
