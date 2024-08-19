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
    }

    private void Start()
    {
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
        cameras.Add("Dark Elf", GameObject.Find("DarkElf").GetComponent<Camera>());
        cameras.Add("Orc", GameObject.Find("Orc").GetComponent<Camera>());
        cameras.Add("Dwarf", GameObject.Find("Dwarf").GetComponent<Camera>());
        cameras.Add("Elf", GameObject.Find("Elf").GetComponent<Camera>());
        cameras.Add("Human", GameObject.Find("Human").GetComponent<Camera>());

        DisableCameras();

        if (GameManager.Instance.GameState == GameState.CHAR_SELECT)
        {
            SwitchCamera("CharSelect");
        }
        else
        {
            SwitchCamera("Login");
        }
    }

    public Camera SelectClassCamera(string race, string charClass)
    {
        if (cameras.TryGetValue(race, out Camera camera))
        {
            if (charClass == "Fighter")
            {
                return camera.transform.GetChild(0).GetComponent<Camera>();
            }

            return camera.transform.GetChild(1).GetComponent<Camera>();
        }


        return null;
    }

    public Camera SelectGenderCamera(string race, string charClass, string gender)
    {
        Camera classCamera = SelectClassCamera(race, charClass);
        if (classCamera != null)
        {
            if (gender == "Male")
            {
                return classCamera.transform.GetChild(0).GetComponent<Camera>();
            }

            return classCamera.transform.GetChild(1).GetComponent<Camera>();
        }

        return null;
    }

    public Camera SelectHeadCamera(string race, string charClass, string gender)
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
                LobbyNameplatesManager.Instance.Camera = obj;
                CharacterSelector.Instance.Camera = obj;
            }
            else
            {
                LobbyNameplatesManager.Instance.Camera = null;
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
            Debug.Log("Disabling camera " + Camera.main.transform);
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
