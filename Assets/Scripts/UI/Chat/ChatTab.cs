using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
