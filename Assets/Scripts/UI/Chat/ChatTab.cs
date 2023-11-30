using System.Collections.Generic;
using UnityEngine.UIElements; 

[System.Serializable]
public class ChatTab
{
    public string tabName = "Tab";
    public List<MessageType> filteredMessages;
    public bool autoscroll = true;
    public Tab tab;
    public ScrollView scrollView;
    public Label content;
    public Scroller scroller;
    public VisualElement chatWindowEle;

    public void Initialize(VisualElement chatWindowEle, Tab tab) {
        this.tab = tab;

        this.chatWindowEle = chatWindowEle;
        scrollView = tab.Q<ScrollView>("ScrollView");
        scroller = scrollView.verticalScroller;
        content = tab.Q<Label>("Content");
        content.text = "";
        
        RegisterAutoScrollEvent();
        RegisterPlayerScrollEvent();
    }

    private void RegisterAutoScrollEvent() {
        content.RegisterValueChangedCallback(evt => {
            if(autoscroll) {
                ChatWindow.GetInstance().ScrollDown(scroller);
            }
        });    
    }

    private void RegisterPlayerScrollEvent() {
        var highBtn = scroller.Q<RepeatButton>("unity-high-button");
        var lowBtn = scroller.Q<RepeatButton>("unity-low-button");
        var dragger = scroller.Q<VisualElement>("unity-drag-container");

        highBtn.RegisterCallback<MouseUpEvent>(evt => {
            VerifyScrollValue();
        });
        lowBtn.RegisterCallback<MouseUpEvent>(evt => {
            VerifyScrollValue();
        });
        dragger.RegisterCallback<MouseUpEvent>(evt => {
            VerifyScrollValue();
        });
        dragger.RegisterCallback<WheelEvent>(evt => {
            VerifyScrollValue();
        });
        
        chatWindowEle.RegisterCallback<GeometryChangedEvent>(evt => {
            if(autoscroll) {
                ChatWindow.GetInstance().ScrollDown(scroller);
            }
        });
    }

    private void VerifyScrollValue() {
        if(scroller.highValue > 0 && scroller.value == scroller.highValue || scroller.highValue == 0 && scroller.value == 0) {
            autoscroll = true;
        } else {
            autoscroll = false;
        }
    }
}
