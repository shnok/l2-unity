using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class L2ConfirmWindow : L2PopupWindow
{
    private Action _confirmAction;
    private Action _cancelAction;
    private Label _contentLabel;

    private static L2ConfirmWindow _instance;
    public static L2ConfirmWindow Instance { get { return _instance; } }

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

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Components/L2ConfirmWindow/L2ConfirmWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        VisualElement dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle, this);
        dragArea.AddManipulator(drag);

        Button cancelButton = GetElementById("CancelButton").Q<Button>("L2Button");
        cancelButton.AddManipulator(new ButtonClickSoundManipulator(cancelButton));
        cancelButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            HideWindow(false);
            if (_cancelAction != null)
            {
                _cancelAction();
            }
        });

        Button okButton = GetElementById("OkButton").Q<Button>("L2Button");
        okButton.AddManipulator(new ButtonClickSoundManipulator(okButton));
        okButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            if (_confirmAction != null)
            {
                _confirmAction();
            }
        });

        _contentLabel = GetLabelById("Content");

        _windowEle.style.left = new Length(50, LengthUnit.Percent);
        _windowEle.style.top = new Length(50, LengthUnit.Percent);
        _windowEle.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent)));

        if (L2GameUI.Instance != null)
        {
            L2GameUI.Instance.WindowLoadComplete();
        }
        else
        {
            L2LoginUI.Instance.WindowLoadComplete();
        }

        HideWindow(true);
    }


    public void ShowWindow(int systemMessageId, Action confirmAction, Action cancelAction)
    {
        string content = SystemMessageTable.Instance.GetSystemMessage(systemMessageId).Message;

        if (content == null)
        {
            _contentLabel.text = "Unkown SystemMessageId.";
        }
        else
        {
            _contentLabel.text = content;
        }

        _confirmAction = confirmAction;
        _cancelAction = cancelAction;

        base.ShowWindow();

        AudioManager.Instance.PlayUISound("window_open");

        if (L2GameUI.Instance != null)
            L2GameUI.Instance.WindowOpened(this);
    }

    public override void HideWindow(bool silent)
    {
        if (!silent && !_isWindowHidden)
            AudioManager.Instance.PlayUISound("window_close");

        base.HideWindow(silent);

        if (L2GameUI.Instance != null)
            L2GameUI.Instance.WindowClosed(this);
    }
}
