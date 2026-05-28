using System;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class L2InputAmountWindow : L2PopupWindow
{
    private Action<int> _confirmAction;
    private Action _cancelAction;
    private Label _contentLabel;
    private VisualElement _cancelButton;
    private TextField _inputField;
    private bool _inputFocused = false;

    private int _maxAmount = 10;
    private int _currentAmount = 0;

    private static L2InputAmountWindow _instance;
    public static L2InputAmountWindow Instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
    }

    private void OnDestroy()
    {
        // _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Components/L2InputAmountWindow/L2InputAmountWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        VisualElement dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle, this);
        dragArea.AddManipulator(drag);

        _cancelButton = GetElementById("CancelButton");

        Button button = _cancelButton.Q<Button>("L2Button");
        button.AddManipulator(new ButtonClickSoundManipulator(button));
        button.RegisterCallback<MouseUpEvent>(evt =>
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
            HideWindow(false);
            if (_confirmAction != null)
            {
                if (_currentAmount == 0)
                {
                    _currentAmount = 1;
                }

                _confirmAction(_currentAmount);
            }
        });

        _inputField = (TextField)GetElementById("L2Input");
        _inputField.RegisterValueChangedCallback(evt => ValidateNewInput(evt));
        _inputField.RegisterCallback<FocusEvent>(evt =>
        {
            _inputFocused = true;
            L2GameUI.Instance.IsTyping = true;
        });

        _inputField.RegisterCallback<BlurEvent>(evt =>
        {
            _inputFocused = false;
            L2GameUI.Instance.IsTyping = false;
        });

        RegisterDigitButton("Number1", "1");
        RegisterDigitButton("Number2", "2");
        RegisterDigitButton("Number3", "3");
        RegisterDigitButton("Number4", "4");
        RegisterDigitButton("Number5", "5");
        RegisterDigitButton("Number6", "6");
        RegisterDigitButton("Number7", "7");
        RegisterDigitButton("Number8", "8");
        RegisterDigitButton("Number9", "9");
        RegisterDigitButton("Number0", "0");

        Button buttonAll = GetElementById("NumberAll").Q<Button>("L2Button");
        buttonAll.AddManipulator(new ButtonClickSoundManipulator(buttonAll));
        buttonAll.RegisterCallback<MouseUpEvent>(evt => _inputField.value = _maxAmount > 0 ? _maxAmount.ToString() : "");
        Button buttonC = GetElementById("NumberC").Q<Button>("L2Button");
        buttonC.AddManipulator(new ButtonClickSoundManipulator(buttonC));
        buttonC.RegisterCallback<MouseUpEvent>(evt => _inputField.value = "");
        Button buttonBS = GetElementById("NumberBS").Q<Button>("L2Button");
        buttonBS.AddManipulator(new ButtonClickSoundManipulator(buttonBS));
        buttonBS.RegisterCallback<MouseUpEvent>(evt => _inputField.value = _inputField.value.Length > 0 ? _inputField.value.Substring(0, _inputField.value.Length - 1) : "");

        _contentLabel = GetLabelById("Content");

        if (L2GameUI.Instance != null)
        {
            L2GameUI.Instance.WindowLoadComplete();
        }
        else
        {
            L2LoginUI.Instance.WindowLoadComplete();
        }

        CenterWindow();

        HideWindow(true);
    }

    private void RegisterDigitButton(string id, string digit)
    {
        Button button = GetElementById(id).Q<Button>("L2Button");
        button.AddManipulator(new ButtonClickSoundManipulator(button));
        button.RegisterCallback<MouseUpEvent>(evt =>
        {
            string raw = _inputField.value.Replace(",", "") + digit;
            if (long.TryParse(raw, out long val))
            {
                _inputField.value = val.ToString("N0", CultureInfo.InvariantCulture);
            }
        });
    }

    private void ValidateNewInput(ChangeEvent<string> evt)
    {
        var rawValue = evt.newValue.Replace(",", "");
        var regex = new Regex(@"^\d*$");
        if (!regex.IsMatch(rawValue))
        {
            _inputField.value = evt.previousValue;
            return;
        }

        if (string.IsNullOrEmpty(rawValue))
            return;

        long amount = long.Parse(rawValue);

        if (_maxAmount > 0 && amount > _maxAmount)
        {
            amount = _maxAmount;
        }
        else if (_maxAmount == 0 && amount > int.MaxValue)
        {
            amount = int.MaxValue;
        }

        _inputField.value = amount.ToString("N0", CultureInfo.InvariantCulture);

        _currentAmount = (int)amount;
    }

    public void ShowWindow(int systemMessageId, int count, Action<int> confirmAction, Action cancelAction)
    {
        string content = SystemMessageTable.Instance.GetSystemMessage(systemMessageId).Message;

        if (content == null)
        {
            content = "Unkown SystemMessageId.";
        }

        ShowWindow(content, count, confirmAction, cancelAction);
    }

    public void ShowWindow(SystemMessage systemMessage, int count, Action<int> confirmAction, Action cancelAction)
    {
        ShowWindow(systemMessage.PrintMessage(false), count, confirmAction, cancelAction);
    }

    private void ShowWindow(string content, int count, Action<int> confirmAction, Action cancelAction)
    {
        _inputFocused = false;
        _maxAmount = count;
        _inputField.value = "";
        _currentAmount = 0;

        if (cancelAction == null)
        {
            _cancelButton.style.display = DisplayStyle.None;
        }

        _contentLabel.text = content;
        _confirmAction = confirmAction;
        _cancelAction = cancelAction;

        base.ShowWindow();

        AudioManager.Instance.PlayUISound("window_open");

        if (L2GameUI.Instance != null)
            L2GameUI.Instance.WindowOpened(this);
    }

    public override void HideWindow(bool silent)
    {
        if (_inputFocused)
        {
            L2GameUI.Instance.IsTyping = false;
        }

        _inputFocused = false;

        if (_cancelButton != null)
        {
            _cancelButton.style.display = DisplayStyle.Flex;
        }

        if (!silent && !_isWindowHidden)
            AudioManager.Instance.PlayUISound("window_close");

        base.HideWindow(silent);

        if (L2GameUI.Instance != null)
            L2GameUI.Instance.WindowClosed(this);
    }
}
