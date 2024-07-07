using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterInfoWindow : L2Window
{
    public VisualElement minimal_panel;
    private VisualElement content;
    private ButtonCharacter _buttonCharacter;

    private static CharacterInfoWindow _instance;
    public static CharacterInfoWindow Instance {
        get { return _instance; }
    }
    private void Awake() {
        if (_instance == null) {
            _instance = this;
            _buttonCharacter = new ButtonCharacter(this);
        } else {
            Destroy(this);
        }
    }

    private void OnDestroy() {
        _instance = null;
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/CharacterInfoWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");
        content = GetElementByClass("content");
        _buttonCharacter.RegisterButtonCloseWindow("btn-close-frame");
        _buttonCharacter.RegisterClickWindow(_windowEle, content, dragArea);


        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
    }
}
