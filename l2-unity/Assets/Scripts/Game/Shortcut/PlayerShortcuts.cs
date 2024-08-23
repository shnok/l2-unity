using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShortcuts : MonoBehaviour
{
    private Dictionary<int, Shortcut> _shortcuts;

    private static PlayerShortcuts _instance;
    public static PlayerShortcuts Instance
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

        _shortcuts = new Dictionary<int, Shortcut>();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    public void UpdateShortcutList(Shortcut[] shortcuts)
    {
        if (_shortcuts == null)
        {
            _shortcuts = new Dictionary<int, Shortcut>();
        }
        else
        {
            _shortcuts.Clear();
        }

        for (int i = 0; i < shortcuts.Length; i++)
        {
            Shortcut shortcut = shortcuts[i];
            UpdateShortcut(shortcut);
        }
    }

    public void UpdateShortcut(Shortcut shortcut)
    {
        if (_shortcuts == null)
        {
            _shortcuts = new Dictionary<int, Shortcut>();
        }
        _shortcuts[shortcut.Slot + shortcut.Page * 12] = shortcut;
    }

    public Shortcut GetShortcut(int page, int slot)
    {
        if (_shortcuts.TryGetValue(slot + page * 12, out Shortcut shortcut))
        {
            return shortcut;
        }

        return null;
    }
}
