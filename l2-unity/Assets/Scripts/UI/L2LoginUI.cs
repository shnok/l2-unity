using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class L2LoginUI : L2UI
{
    [SerializeField] private VisualElement _loadingElement;

    private static L2LoginUI _instance;
    public static L2LoginUI Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void Start()
    {
        _windowsLoaded = 0;
    }

    protected override void LoadUI()
    {
        base.LoadUI();

        LoginWindow.Instance.AddWindow(_rootVisualContainer);
        CharSelectWindow.Instance.AddWindow(_rootVisualContainer);
        CharSelectWindow.Instance.HideWindow(true);
        CharCreationWindow.Instance.AddWindow(_rootVisualContainer);
        CharCreationWindow.Instance.HideWindow(true);
        LicenseWindow.Instance.AddWindow(_rootVisualContainer);
        LicenseWindow.Instance.HideWindow(true);
        ServerSelectWindow.Instance.AddWindow(_rootVisualContainer);
        ServerSelectWindow.Instance.HideWindow(true);
        L2ConfirmWindow.Instance.AddWindow(_rootVisualContainer);
        L2ConfirmWindow.Instance.HideWindow(true);

        if (GameManager.Instance.AutoLogin)
        {
            StartCoroutine(AutoLogin());
        }
    }

    private IEnumerator AutoLogin()
    {
        yield return new WaitForSeconds(1f);
        LoginWindow.Instance.ShowWindow();
    }

    public void ShowServerSelectWindow()
    {
        LoginWindow.Instance.HideWindow(false);
        LicenseWindow.Instance.HideWindow(false);
        ServerSelectWindow.Instance.ShowWindow();
    }

    public void ShowLicenseWindow()
    {
        LoginWindow.Instance.HideWindow(false);
        LicenseWindow.Instance.ShowWindow();
        ServerSelectWindow.Instance.HideWindow(false);
    }

    public void ShowCharSelectWindow()
    {
        LoginWindow.Instance.HideWindow(false);
        CharCreationWindow.Instance.HideWindow(false);
        CharSelectWindow.Instance.ShowWindow();
        ServerSelectWindow.Instance.HideWindow(false);
    }

    public void ShowLoginWindow()
    {
        CharSelectWindow.Instance.HideWindow(false);
        LoginWindow.Instance.ShowWindow();
        CharCreationWindow.Instance.HideWindow(false);
        CharSelectWindow.Instance.HideWindow(false);
        LicenseWindow.Instance.HideWindow(false);
        ServerSelectWindow.Instance.HideWindow(false);
    }

    public void ShowCharCreationWindow()
    {
        CharSelectWindow.Instance.HideWindow(false);
        CharCreationWindow.Instance.ShowWindow();
    }
}
