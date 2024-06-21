using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArrowInputManipulator : PointerManipulator {
    private Label _label;
    private string[] _values;
    private TextField _textField;
    private Button _leftArrow;
    private Button _rightArrow;
    private Action<int, string> _onArrowClick;
    private int _index;

    public String Value { get { return _textField.value; } }

    public ArrowInputManipulator(VisualElement target, string label, string[] values, int defaultIndex, Action<int, string> onArrowClick) {
        this.target = target;
        _label.text = label;
        _values = values;
        if(defaultIndex != -1) {
            _textField.value = values[defaultIndex];
        }
        _onArrowClick = onArrowClick;
    }

    private void LoadElements() {
        _label = target.Q<Label>("Label");
        _textField = target.Q<TextField>("DataField");
        _leftArrow = target.Q<Button>("LeftArrow");
        _rightArrow = target.Q<Button>("RightArrow");
    }

    protected override void RegisterCallbacksOnTarget() {
        LoadElements();
        _leftArrow.RegisterCallback<ClickEvent>(OnLeftArrowClick);
        _rightArrow.RegisterCallback<ClickEvent>(OnRightArrowClick);
        _leftArrow.AddManipulator(new ButtonClickSoundManipulator(_leftArrow));
        _rightArrow.AddManipulator(new ButtonClickSoundManipulator(_rightArrow));
    }

    protected override void UnregisterCallbacksFromTarget() {
        _leftArrow.UnregisterCallback<ClickEvent>(OnLeftArrowClick);
        _rightArrow.UnregisterCallback<ClickEvent>(OnRightArrowClick);
    }  
    
    private void OnLeftArrowClick(ClickEvent e) {
        if (--_index < 0) {
            _index = _values.Length - 1;
        }

        _textField.value = _values[_index];

        _onArrowClick(_index, _textField.value);
    }

    private void OnRightArrowClick(ClickEvent e) {
        if (++_index > _values.Length - 1) {
            _index = 0;
        }

        _textField.value = _values[_index];

        _onArrowClick(_index, _textField.value);
    }

    public void ClearInput() {
        _index = -1;
        _textField.value = "";
    }

    public void ResetInput() {
        _index = 0;
        _textField.value = _values[_index];
    }
}