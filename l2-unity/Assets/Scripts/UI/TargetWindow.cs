using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetWindow : MonoBehaviour {
    private VisualTreeAsset _targetWindowTemplate;
    private VisualElement _targetWindowEle;
    private Label _nameLabel;
    private VisualElement _HPBar;
    private VisualElement _HPBarBG;

    [SerializeField] private float _targetWindowMinWidth = 175.0f;
    [SerializeField] private float _targetWindowMaxWidth = 300.0f;


    private static TargetWindow _instance;
    public static TargetWindow Instance { get { return _instance; } }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    void Start() {
        if(_targetWindowTemplate == null) {
            _targetWindowTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/TargetWindow");
        }
        if(_targetWindowTemplate == null) {
            Debug.LogError("Could not load status window template.");
        }
    }

    public void AddWindow(VisualElement root) {
        if(_targetWindowTemplate == null) {
            return;
        }

        StartCoroutine(BuildWindow(root));
    }

    IEnumerator BuildWindow(VisualElement root) {

        _targetWindowEle = _targetWindowTemplate.Instantiate()[0];
        MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(_targetWindowEle);
        _targetWindowEle.AddManipulator(mouseOverDetection);

        var statusWindowDragArea = _targetWindowEle.Q<VisualElement>(null, "drag-area");
        DragManipulator drag = new DragManipulator(statusWindowDragArea, _targetWindowEle);
        statusWindowDragArea.AddManipulator(drag);

        var horizontalResizeHandle = _targetWindowEle.Q<VisualElement>(null, "hor-resize-handle");
        HorizontalResizeManipulator horizontalResize = new HorizontalResizeManipulator(
            horizontalResizeHandle, _targetWindowEle, _targetWindowMinWidth, _targetWindowMaxWidth);
        horizontalResizeHandle.AddManipulator(horizontalResize);

        var closeBtnHandle = _targetWindowEle.Q<Button>("CloseBtn");
        closeBtnHandle.RegisterCallback<MouseDownEvent>(evt => {
            AudioManager.Instance.PlayUISound("click_01");
        }, TrickleDown.TrickleDown);
        closeBtnHandle.RegisterCallback<MouseUpEvent>(evt => {
            TargetManager.Instance.ClearTarget();
        });

        horizontalResizeHandle.AddManipulator(horizontalResize);

        _targetWindowEle.style.display = DisplayStyle.None;

        root.Add(_targetWindowEle);

        yield return new WaitForEndOfFrame();

        _nameLabel = _targetWindowEle.Q<Label>("TargetName");
        if(_nameLabel == null) {
            Debug.LogError("Target window target name label is null.");
        }

        _HPBar = _targetWindowEle.Q<VisualElement>("HPBar");
        if(_HPBar == null) {
            Debug.LogError("Target window HPBar is null");
        }

        _HPBarBG = _targetWindowEle.Q<VisualElement>("HPBarBG");
        if(_HPBarBG == null) {
            Debug.LogError("Target window HPBarBG is null");
        }

        _targetWindowEle.style.position = Position.Absolute;
        _targetWindowEle.style.left = Screen.width / 2f - _targetWindowEle.resolvedStyle.width / 2f;
        _targetWindowEle.style.top = 0;
    }

    private void FixedUpdate() {
        if(_targetWindowEle == null) {
            return;
        }

        if(TargetManager.Instance.HasTarget()) {
            _targetWindowEle.style.display = DisplayStyle.Flex;

            TargetData targetData = TargetManager.Instance.Target;
            if(_nameLabel != null) {
                _nameLabel.text = targetData.Identity.Name;
            }
            if(_HPBarBG != null && _HPBar != null) {
                float hpRatio = (float)targetData.Status.Hp / targetData.Status.MaxHp;
                float bgWidth = _HPBarBG.resolvedStyle.width;
                float barWidth = bgWidth * hpRatio;
                _HPBar.style.width = barWidth;
            }
        } else {
            if(_targetWindowEle.resolvedStyle.display == DisplayStyle.Flex) {
                AudioManager.Instance.PlayUISound("window_close");
                _targetWindowEle.style.display = DisplayStyle.None;
            }
        }
    }
}
