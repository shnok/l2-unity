using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
 
public class ChatInput : MonoBehaviour, IPointerClickHandler {
    public bool chatOpened = false; 
    public InputField inputField;
    public static ChatInput _instance;
    public static ChatInput GetInstance() {
        return _instance;
    }

    void Awake() {
        inputField = GetComponent<InputField>();

        if(_instance == null) {
            _instance = this;
        } else {
            Object.Destroy(gameObject);
        }
    }

    public void OnPointerClick (PointerEventData eventData) {
        inputField.interactable = true;
        chatOpened = true; 
        inputField.Select();
    }

    public void ToggleOpenChat() {
        inputField.interactable = !inputField.interactable;
        chatOpened = inputField.interactable;

        if(chatOpened) {
            inputField.Select();
        } else {
            if(inputField.text.Length > 0) {
               // DefaultClient.GetInstance().SendChatMessage(inputField.text);
                inputField.text = "";
            }
        }
    }
}