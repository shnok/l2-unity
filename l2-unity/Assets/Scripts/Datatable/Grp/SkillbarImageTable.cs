using System.Collections.Generic;
using UnityEngine;

public class SkillbarImageTable
{
    private string _skillbarFolder = "Data\\UI\\Assets\\SkillBar";
    private Dictionary<string, Texture2D> _keyImages = new Dictionary<string, Texture2D>();
    private Texture2D[] _cooltimeTextures = new Texture2D[360];
    private Texture2D[] _cooltimeEndTextures = new Texture2D[9];
    private Texture2D[] _toggleTextures = new Texture2D[13];

    public Texture2D[] CooltimeTextures { get => _cooltimeTextures; }
    public Texture2D[] CooltimeEndTextures { get => _cooltimeEndTextures; }
    public Texture2D[] ToggleTextures { get => _toggleTextures; }

    private static SkillbarImageTable _instance;
    public static SkillbarImageTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillbarImageTable();
            }

            return _instance;
        }
    }

    public void Initialize()
    {
        _keyImages = new Dictionary<string, Texture2D>();


        LoadCooltimeTextures();
        LoadCooltimeEndTextures();
        LoadToggleTextures();
    }

    private void LoadCooltimeTextures()
    {
        for (int i = 0; i < _cooltimeTextures.Length; i++)
        {
            string image = $"{_skillbarFolder}\\Effects\\Cooltime\\cooltime{i}";
            Texture2D texture = Resources.Load<Texture2D>(image);
            if (texture != null)
            {
                _cooltimeTextures[i] = texture;
            }
            else
            {
                Debug.LogWarning($"Missing cooltime icon: {i}.");
            }
        }
    }

    private void LoadCooltimeEndTextures()
    {
        for (int i = 1; i <= _cooltimeEndTextures.Length; i++)
        {
            string image = $"{_skillbarFolder}\\Effects\\CooltimeEnd\\CoolTimeEnd{i:D3}";
            Texture2D texture = Resources.Load<Texture2D>(image);
            if (texture != null)
            {
                _cooltimeEndTextures[i - 1] = texture;
            }
            else
            {
                Debug.LogWarning($"Missing cooltimeEnd icon: {i}.");
            }
        }
    }

    private void LoadToggleTextures()
    {
        for (int i = 1; i <= _toggleTextures.Length; i++)
        {
            string image = $"{_skillbarFolder}\\Effects\\Toggle\\ToggleEffect{i:D3}";
            Texture2D texture = Resources.Load<Texture2D>(image);
            if (texture != null)
            {
                _toggleTextures[i - 1] = texture;
            }
            else
            {
                Debug.LogWarning($"Missing toggle icon: {i}.");
            }
        }
    }

    public Texture2D LoadTextureByKey(string name)
    {
        if (name.Length == 0)
        {
            return null;
        }

        if (_keyImages.TryGetValue(name, out Texture2D result))
        {
            return result;
        }

        string icon = _skillbarFolder + "\\Shortcuts\\ShortcutWnd_DF_Key_" + name;
        result = Resources.Load<Texture2D>(icon);

        if (result != null)
        {
            _keyImages.Add(name, result);
            return result;
        }

        Debug.LogWarning($"Missing skillbar icon: {name}");

        return null;
    }
}
