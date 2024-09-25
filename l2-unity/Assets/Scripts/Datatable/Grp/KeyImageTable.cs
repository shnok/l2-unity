using System.Collections.Generic;
using UnityEngine;

public class KeyImageTable
{
    private string _iconFolder = "Data\\UI\\Assets\\SkillBar\\Shortcuts";
    private Dictionary<string, Texture2D> _keyImages = new Dictionary<string, Texture2D>();

    private static KeyImageTable _instance;
    public static KeyImageTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new KeyImageTable();
            }

            return _instance;
        }
    }

    public void Initialize() {
        _keyImages = new Dictionary<string, Texture2D>();
    }

    public Texture2D LoadTextureByKey(string name)
    {
        if(_keyImages.TryGetValue(name, out Texture2D result)) {
            return result;
        }

        string icon = _iconFolder + "\\ShortcutWnd_DF_Key_" + name;
        result = Resources.Load<Texture2D>(icon);

        if (result != null)
        {
            _keyImages.Add(name, result);
            return result;
        }

        Debug.LogWarning($"Missing icon: {name}.");

        return null;
    }
}
