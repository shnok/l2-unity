using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IconManager {
    private string _iconFolder = "Data\\SysTextures\\Icon";

    private Texture2D _noImageIcon;

    private Dictionary<int, Texture2D> _icons = new Dictionary<int, Texture2D>();

    private static IconManager _instance;
    public static IconManager Instance {
        get {
            if (_instance == null) {
                _instance = new IconManager();
            }

            return _instance;
        }
    }

    public void Initialize() {
        _noImageIcon = GetNoImageIcon();
    }

    public void CacheIcons() {
        foreach(Weapon weapon in ItemTable.Instance.Weapons.Values) {
            Texture2D icon = LoadTextureByName(weapon.Icon);
            _icons.Add(weapon.Id, icon);
        }

        foreach (Armor armor in ItemTable.Instance.Armors.Values) {
            Texture2D icon = LoadTextureByName(armor.Icon);
            _icons.Add(armor.Id, icon);
        }

        foreach (EtcItem etcItem in ItemTable.Instance.EtcItems.Values) {
            Texture2D icon = LoadTextureByName(etcItem.Icon);
            _icons.Add(etcItem.Id, icon);
        }
    }

    private Texture2D LoadTextureByName(string name) {
        string icon = _iconFolder + "\\" + CleanIconName(name);
        var result = Resources.Load<Texture2D>(icon);

        if (result != null) {
            return result;
        }

        Debug.LogWarning($"Missing icon: {name}.");

        return _noImageIcon;
    }

    public Texture2D GetIcon(int id) {
        Texture2D icon;
        _icons.TryGetValue(id, out icon);

        if(icon == null) {
            _icons.Add(id, _noImageIcon);
            return _noImageIcon;
        }

        return icon;
    }

    private Texture2D GetNoImageIcon() {
        return Resources.Load<Texture2D>(_iconFolder + "\\" + "NOIMAGE");
    }

    private string CleanIconName(string name) {
        return name.Replace("icon.", "");
    }
}
