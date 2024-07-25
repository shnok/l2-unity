using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class L2LoginUI : L2UI
{
    [SerializeField] private VisualElement _loadingElement;

    private static L2LoginUI _instance;
    public static L2LoginUI Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    protected override void Update() {
        base.Update();
    }

    private void OnDestroy() {
        _instance = null;
    }

    protected override void LoadUI() {
        base.LoadUI();

        LoginWindow.Instance.AddWindow(_rootVisualContainer);
        CharSelectWindow.Instance.AddWindow(_rootVisualContainer);
        CharSelectWindow.Instance.HideWindow();
        CharCreationWindow.Instance.AddWindow(_rootVisualContainer);
        CharCreationWindow.Instance.HideWindow();
        LicenseWindow.Instance.AddWindow(_rootVisualContainer);
        LicenseWindow.Instance.HideWindow();
        ServerSelectWindow.Instance.AddWindow(_rootVisualContainer);
        ServerSelectWindow.Instance.HideWindow();

        if(GameManager.Instance.AutoLogin) {
            StartCoroutine(AutoLogin());
        }
    }

    private IEnumerator AutoLogin() {
        yield return new WaitForSeconds(1f);
        LoginWindow.Instance.ShowWindow();
    }

    public void ShowServerSelectWindow() {
        LoginWindow.Instance.HideWindow();
        LicenseWindow.Instance.HideWindow();
        ServerSelectWindow.Instance.ShowWindow();
    }

    public void ShowLicenseWindow() {
        LoginWindow.Instance.HideWindow();
        LicenseWindow.Instance.ShowWindow();
        ServerSelectWindow.Instance.HideWindow();
    }

    public void ShowCharSelectWindow() {
        LoginWindow.Instance.HideWindow();
        CharCreationWindow.Instance.HideWindow();
        CharSelectWindow.Instance.ShowWindow();
        ServerSelectWindow.Instance.HideWindow();
    }

    public void ShowLoginWindow() {
        CharSelectWindow.Instance.HideWindow();
        LoginWindow.Instance.ShowWindow();
        CharCreationWindow.Instance.HideWindow();
        CharSelectWindow.Instance.HideWindow();
        LicenseWindow.Instance.HideWindow();
        ServerSelectWindow.Instance.HideWindow();
    }

    public void ShowCharCreationWindow() {
        CharSelectWindow.Instance.HideWindow();
        CharCreationWindow.Instance.ShowWindow();
    }
}
