using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class StatusWindow : MonoBehaviour
{
    [SerializeField]
    private VisualTreeAsset statusWindowTemplate;

    public float statusWindowMinWidth = 175.0f;
    public float statusWindowMaxWidth = 400.0f;
    private Label nameLabel;
    private Label levelLabel;
    private Label HPTextLabel;
    private Label MPTextLabel;
    private Label CPTextLabel;
    private VisualElement CPBar;
    private VisualElement CPBarBG;
    private VisualElement HPBar;
    private VisualElement HPBarBG;
    private VisualElement MPBar;
    private VisualElement MPBarBG;

    private static StatusWindow instance;
    public static StatusWindow GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }

    void Start() {
        if(statusWindowTemplate == null) {
            statusWindowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Data/UI/_Elements/StatusWindow.uxml");
        }
        if(statusWindowTemplate == null) {
            Debug.LogError("Could not load status window template.");
        }
    }

    public void AddWindow(VisualElement root) {
        if(statusWindowTemplate == null) {
            return;
        }

        var statusWindowEle = statusWindowTemplate.Instantiate()[0];
        MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(statusWindowEle);
        statusWindowEle.AddManipulator(mouseOverDetection);

        var statusWindowDragArea = statusWindowEle.Q<VisualElement>(null, "drag-area");
        DragManipulator drag = new DragManipulator(statusWindowDragArea, statusWindowEle);
        statusWindowDragArea.AddManipulator(drag);

        var horizontalResizeHandle = statusWindowEle.Q<VisualElement>(null, "hor-resize-handle");
        HorizontalResizeManipulator horizontalResize = new HorizontalResizeManipulator(
            horizontalResizeHandle, statusWindowEle, statusWindowMinWidth, statusWindowMaxWidth);
        horizontalResizeHandle.AddManipulator(horizontalResize);

        nameLabel = statusWindowEle.Q<Label>("PlayerNameText");
        if(nameLabel == null) {
            Debug.LogError("Status window PlayerNameText is null.");
        }

        levelLabel = statusWindowEle.Q<Label>("LevelText");
        if(levelLabel == null) {
            Debug.LogError("Status window LevelText is null.");
        }

        CPTextLabel = statusWindowEle.Q<Label>("CPText");
        if(CPTextLabel == null) {
            Debug.LogError("Status window CPText is null.");
        }

        HPTextLabel = statusWindowEle.Q<Label>("HPText");
        if(HPTextLabel == null) {
            Debug.LogError("Status window Hp text is null.");
        }

        MPTextLabel = statusWindowEle.Q<Label>("MPText");
        if(MPTextLabel == null) {
            Debug.LogError("Status window MPText is null.");
        }

        CPBarBG = statusWindowEle.Q<VisualElement>("CPBarBG");
        if(CPBarBG == null) {
            Debug.LogError("Status window CPBarBG is null");
        }

        CPBar = statusWindowEle.Q<VisualElement>("CPBar");
        if(CPBar == null) {
            Debug.LogError("Status window CPBar is null");
        }

        HPBar = statusWindowEle.Q<VisualElement>("HPBar");
        if(HPBar == null) {
            Debug.LogError("Status window HPBar is null");
        }

        HPBarBG = statusWindowEle.Q<VisualElement>("HPBarBG");
        if(HPBarBG == null) {
            Debug.LogError("Status window HPBarBG is null");
        }

        HPBar = statusWindowEle.Q<VisualElement>("HPBar");
        if(HPBar == null) {
            Debug.LogError("Status window HPBar is null");
        }

        MPBarBG = statusWindowEle.Q<VisualElement>("MPBarBG");
        if(MPBarBG == null) {
            Debug.LogError("Status window MPBarBG is null");
        }

        MPBar = statusWindowEle.Q<VisualElement>("MPBar");
        if(MPBar == null) {
            Debug.LogError("Status windowar MPBar is null");
        }


        root.Add(statusWindowEle);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(PlayerEntity.GetInstance() == null) { 
            return; 
        }

        if(levelLabel != null) {
            levelLabel.text = PlayerEntity.GetInstance().Status.Level.ToString();
        }

        if(nameLabel != null) {
            nameLabel.text = PlayerEntity.GetInstance().Identity.Name;
        }

        if(CPTextLabel != null) {
            CPTextLabel.text = PlayerEntity.GetInstance().Status.Cp + "/" + PlayerEntity.GetInstance().Status.MaxCp;
        }

        if(HPTextLabel != null) {
            HPTextLabel.text = PlayerEntity.GetInstance().Status.Hp + "/" + PlayerEntity.GetInstance().Status.MaxHp;
        }

        if(MPTextLabel != null) {
            MPTextLabel.text = PlayerEntity.GetInstance().Status.Mp + "/" + PlayerEntity.GetInstance().Status.MaxMp;
        }

        if(CPBarBG != null && CPBar != null) {
            float cpRatio = (float)PlayerEntity.GetInstance().Status.Cp / PlayerEntity.GetInstance().Status.MaxCp;
            float bgWidth = CPBarBG.resolvedStyle.width;
            float barWidth = bgWidth * cpRatio;
            CPBar.style.width = barWidth;
        }

        if(HPBarBG != null && HPBar != null) {
            float hpRatio = (float)PlayerEntity.GetInstance().Status.Hp / PlayerEntity.GetInstance().Status.MaxHp;
            float bgWidth = HPBarBG.resolvedStyle.width;
            float barWidth = bgWidth * hpRatio;
            HPBar.style.width = barWidth;
        }

        if(MPBarBG != null && MPBar != null) {
            float mpRatio = (float)PlayerEntity.GetInstance().Status.Mp / PlayerEntity.GetInstance().Status.MaxMp;
            float bgWidth = MPBarBG.resolvedStyle.width;
            float barWidth = bgWidth * mpRatio;
            MPBar.style.width = barWidth;
        }
    }
}
