using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class L2ToolTip : L2PopupWindow {

    private Label _title;
    private VisualElement _tooltipTarget;
    private Coroutine _updateStyleCoroutine;

    private static L2ToolTip _instance;
    public static L2ToolTip Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    private void OnDestroy() {
        _instance = null;
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Template/Tooltip");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _title = GetLabelById("Title");
    }

    public void UpdateTooltip(string title, VisualElement target) {
        _windowEle.style.left = -1000;
        _tooltipTarget = target;

        ShowWindow();

        if(_updateStyleCoroutine != null) {
            StopCoroutine(_updateStyleCoroutine);
        }

        _updateStyleCoroutine = StartCoroutine(UpdateToolTipCoroutine(title, target));
    }

    IEnumerator UpdateToolTipCoroutine(string title, VisualElement target) {
        while(true) {
            _title.text = title;

            BringToFront();

            yield return new WaitForEndOfFrame();

            _windowEle.style.left = target.worldBound.x;
            _windowEle.style.top = target.worldBound.y - _windowEle.resolvedStyle.height;
        }
    }

    public void HideWindow(VisualElement exitElement) {
        if(exitElement == _tooltipTarget) {
            base.HideWindow();
            
            if(_updateStyleCoroutine != null) {
                StopCoroutine(_updateStyleCoroutine);
            }
        }
    }
}
