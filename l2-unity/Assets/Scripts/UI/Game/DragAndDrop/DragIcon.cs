using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class DragIcon : L2PopupWindow
{
    public VisualElement icon;


    private static DragIcon _instance;
    public static DragIcon Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/DragAndDrop/DragIcon");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);


        yield return new WaitForEndOfFrame();

        icon = GetElementByClass("Icon");
        var content = GetElementByClass("drag_content");
        icon.style.backgroundImage = IconManager.Instance.LoadTextureByName("noimage");
        RegisterClickWindow(content);
       // HideWindow();
    }

    public void NewPosition(Vector2 position , Background targetBackground)
    {
        if(position != null)
        {
            // if (!icon.visible)
            // {
            //     ShowWindow();
            // }
           // icon.style.backgroundImage = targetBackground;
            Debug.Log("POSITIIIION");
            icon.transform.position = position;
        }
    }

    public void BringToFront1()
    {
        BringToFront();
    }
    public void RegisterClickWindow(VisualElement contentId)
    {

        if (contentId == null)
        {
            Debug.LogError(contentId + " can't be found.");
            return;
        }

        contentId.RegisterCallback<MouseDownEvent>(evt => {
            //Debug.Log("Evennt cliiikkk 1");
            BringToFront();

        }, TrickleDown.TrickleDown);

    }
}
