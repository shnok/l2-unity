using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IconManager : MonoBehaviour
{

    private string defaultIconSpace = "Data\\SysTextures\\Icon";
    private string[] fillBackground = { "Data/UI/ShortCut/demo_skills/fill_black", "Data/UI/Window/Inventory/bg_windows/blue_tab_v5" };

    private static IconManager _instance;
    public static IconManager Instance { 
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

    public Texture2D LoadBlackBorderCell()
    {
        return Resources.Load<Texture2D>(fillBackground[0]);
    }

    public Texture2D LoadColorBorderCell()
    {
        return  Resources.Load<Texture2D>(fillBackground[1]);
    }

    public Texture2D LoadTextureByName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            string icon = defaultIconSpace + "\\" + clearIcon(name);
            var result = Resources.Load<Texture2D>(icon);
            if (result != null)
            {
                return result;
            }
        }
        return Resources.Load<Texture2D>(defaultIconSpace + "\\" + "NOIMAGE");

    }

    private string clearIcon(string name)
    {
        return name.Replace("icon.", "");
    }


}
