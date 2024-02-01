using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetWindow : MonoBehaviour {
    [SerializeField]
    private VisualTreeAsset targetWindowTemplate;

    public float statusWindowMinWidth = 175.0f;
    public float statusWindowMaxWidth = 300.0f;
    private VisualElement targetWindowEle;
    private Label nameLabel;
    private VisualElement HPBar;
    private VisualElement HPBarBG;

    private static TargetWindow instance;
    public static TargetWindow GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }

    void Start() {
        if(targetWindowTemplate == null) {
            targetWindowTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/TargetWindow");
        }
        if(targetWindowTemplate == null) {
            Debug.LogError("Could not load status window template.");
        }
    }

    public void AddWindow(VisualElement root) {
        if(targetWindowTemplate == null) {
            return;
        }

        StartCoroutine(BuildWindow(root));
    }

    IEnumerator BuildWindow(VisualElement root) {

        targetWindowEle = targetWindowTemplate.Instantiate()[0];
        MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(targetWindowEle);
        targetWindowEle.AddManipulator(mouseOverDetection);

        var statusWindowDragArea = targetWindowEle.Q<VisualElement>(null, "drag-area");
        DragManipulator drag = new DragManipulator(statusWindowDragArea, targetWindowEle);
        statusWindowDragArea.AddManipulator(drag);

        var horizontalResizeHandle = targetWindowEle.Q<VisualElement>(null, "hor-resize-handle");
        HorizontalResizeManipulator horizontalResize = new HorizontalResizeManipulator(
            horizontalResizeHandle, targetWindowEle, statusWindowMinWidth, statusWindowMaxWidth);
        horizontalResizeHandle.AddManipulator(horizontalResize);

        var closeBtnHandle = targetWindowEle.Q<Button>("CloseBtn");
        closeBtnHandle.RegisterCallback<MouseDownEvent>(evt => {
            AudioManager.Instance.PlayUISound("click_01");
        }, TrickleDown.TrickleDown);
        closeBtnHandle.RegisterCallback<MouseUpEvent>(evt => {
            TargetManager.GetInstance().ClearTarget();
        });

        horizontalResizeHandle.AddManipulator(horizontalResize);

        targetWindowEle.style.display = DisplayStyle.None;

        root.Add(targetWindowEle);

        yield return new WaitForEndOfFrame();

        nameLabel = targetWindowEle.Q<Label>("TargetName");
        if(nameLabel == null) {
            Debug.LogError("Target window target name label is null.");
        }

        HPBar = targetWindowEle.Q<VisualElement>("HPBar");
        if(HPBar == null) {
            Debug.LogError("Target window HPBar is null");
        }

        HPBarBG = targetWindowEle.Q<VisualElement>("HPBarBG");
        if(HPBarBG == null) {
            Debug.LogError("Target window HPBarBG is null");
        }

        targetWindowEle.style.position = Position.Absolute;
        targetWindowEle.style.left = Screen.width / 2f - targetWindowEle.resolvedStyle.width / 2f;
        targetWindowEle.style.top = 0;
    }

    private void FixedUpdate() {
        if(targetWindowEle == null) {
            return;
        }

        if(TargetManager.GetInstance().HasTarget()) {
            targetWindowEle.style.display = DisplayStyle.Flex;

            TargetData targetData = TargetManager.GetInstance().GetTargetData();
            if(nameLabel != null) {
                nameLabel.text = targetData.identity.Name;
            }
            if(HPBarBG != null && HPBar != null) {
                float hpRatio = (float)targetData.status.Hp / targetData.status.MaxHp;
                float bgWidth = HPBarBG.resolvedStyle.width;
                float barWidth = bgWidth * hpRatio;
                HPBar.style.width = barWidth;
            }
        } else {
            if(targetWindowEle.resolvedStyle.display == DisplayStyle.Flex) {
                AudioManager.Instance.PlayUISound("window_close");
                targetWindowEle.style.display = DisplayStyle.None;
            }
        }
    }
}
