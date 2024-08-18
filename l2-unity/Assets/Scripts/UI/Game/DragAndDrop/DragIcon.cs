using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class DragIcon : L2PopupWindow
{
    public VisualElement icon;


    private static DragIcon _instance;
    private bool _isShow = false;
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
        HideWindow();
    }


    public void SetBackground(Background baackground)
    {
        icon.style.backgroundImage = baackground;
    }
    public void NewPosition(Vector2 position)
    {
        if(position != null)
        {
            if (!_isShow)
            {
                ShowWindow();
                _isShow = true;
            }
            //icon.style.backgroundImage = IconManager.Instance.LoadTextureByName("skill0914");
            icon.transform.position = position;
        }
    }

    public void ResetPosition()
    {
        icon.transform.position = Vector2.zero;
        if (_isShow)
        {
            _isShow = false;
            HideWindow();
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
