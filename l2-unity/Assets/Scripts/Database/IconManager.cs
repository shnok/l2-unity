using System.Collections.Generic;
using UnityEngine;

public class IconTable
{
    private string _iconFolder = "Data\\SysTextures\\Icon";

    private Texture2D _noImageIcon;

    private Dictionary<int, Texture2D> _icons = new Dictionary<int, Texture2D>();
    private Dictionary<string, Texture2D> _keyIcons = new Dictionary<string, Texture2D>();

    private static IconTable _instance;
    public static IconTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new IconTable();
            }

            return _instance;
        }
    }

    public void Initialize()
    {
        _noImageIcon = GetNoImageIcon();
        
        CacheIcons();
    }

    private void CacheIcons()
    {
        foreach (Weapon weapon in ItemTable.Instance.Weapons.Values)
        {
            Texture2D icon = LoadTextureByName(weapon.Icon);
            _icons.Add(weapon.Id, icon);
        }

        foreach (Armor armor in ItemTable.Instance.Armors.Values)
        {
            Texture2D icon = LoadTextureByName(armor.Icon);
            _icons.Add(armor.Id, icon);
        }

        foreach (EtcItem etcItem in ItemTable.Instance.EtcItems.Values)
        {
            Texture2D icon = LoadTextureByName(etcItem.Icon);
            _icons.Add(etcItem.Id, icon);
        }
    }

    public Texture2D LoadTextureByName(string name)
    {
        string icon = _iconFolder + "\\" + CleanIconName(name);
        var result = Resources.Load<Texture2D>(icon);

        //Debug.Log($"Loading icon {name}.");
        if (result != null)
        {
            return result;
        }

        Debug.LogWarning($"Missing icon: {name}.");

        return _noImageIcon;
    }

    public Texture2D GetIcon(int id)
    {
        Texture2D icon;
        _icons.TryGetValue(id, out icon);

        if (icon == null)
        {
            _icons.Add(id, _noImageIcon);
            return _noImageIcon;
        }

        return icon;
    }

    private Texture2D GetNoImageIcon()
    {
        return Resources.Load<Texture2D>(_iconFolder + "\\" + "NOIMAGE");
    }

    private string CleanIconName(string name)
    {
        return name.Replace("icon.", "");
    }
}
