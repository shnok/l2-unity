using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ArrowInputManipulator : PointerManipulator
{
    private string _labelValue;
    private Label _label;
    private string[] _values;
    private TextField _textField;
    private Button _leftArrow;
    private Button _rightArrow;
    private Action<int, string> _onArrowClick;
    private int _index;
    private int _defaultIndex;

    public String Value { get { return _textField.value; } }
    public int Index { get { return _index; } }

    public ArrowInputManipulator(VisualElement target, string label, string[] values, int defaultIndex, Action<int, string> onArrowClick)
    {
        _labelValue = label;
        this.target = target;
        if (_labelValue != null && _labelValue.Length > 0)
        {
            _label.text = label;
        }

        _values = values;
        _index = defaultIndex;

        if (defaultIndex != -1)
        {
            _textField.value = values[defaultIndex];
        }

        _onArrowClick = onArrowClick;
    }

    public void UpdateValues(string[] newVals)
    {
        _values = newVals;
        _index = -1;
        _textField.value = "";
    }

    private void LoadElements()
    {
        if (_labelValue != null && _labelValue.Length > 0)
        {
            _label = target.Q<Label>("Label");
        }
        _textField = target.Q<TextField>("DataField");
        _leftArrow = target.Q<Button>("LeftArrow");
        _rightArrow = target.Q<Button>("RightArrow");
    }

    protected override void RegisterCallbacksOnTarget()
    {
        LoadElements();
        _leftArrow.RegisterCallback<ClickEvent>(OnLeftArrowClick);
        _rightArrow.RegisterCallback<ClickEvent>(OnRightArrowClick);
        _leftArrow.AddManipulator(new ButtonClickSoundManipulator(_leftArrow));
        _rightArrow.AddManipulator(new ButtonClickSoundManipulator(_rightArrow));
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        _leftArrow.UnregisterCallback<ClickEvent>(OnLeftArrowClick);
        _rightArrow.UnregisterCallback<ClickEvent>(OnRightArrowClick);
    }

    private void OnLeftArrowClick(ClickEvent e)
    {
        if (--_index < 0)
        {
            _index = _values.Length - 1;
        }

        _textField.value = _values[_index];

        _onArrowClick(_index, _textField.value);
    }

    private void OnRightArrowClick(ClickEvent e)
    {
        if (++_index > _values.Length - 1)
        {
            _index = 0;
        }

        _textField.value = _values[_index];

        _onArrowClick(_index, _textField.value);
    }

    public void SelectIndex(int index)
    {
        if (index < 0 || index >= _values.Length)
        {
            Debug.LogWarning($"Trying to select wrong index [{index}] for arrow manipulator.");
            return;
        }

        _index = index;
        _textField.value = _values[_index];
    }

    public void ClearInput()
    {
        _index = -1;
        _textField.value = "";
    }

    public void ResetInput()
    {
        _index = 0;
        _textField.value = _values[_index];
    }
}