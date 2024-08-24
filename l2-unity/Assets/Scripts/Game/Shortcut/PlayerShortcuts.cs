using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShortcuts : MonoBehaviour
{
    private List<Shortcut> _shortcuts;

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

        _shortcuts = new List<Shortcut>();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void Update()
    {
        if (_shortcuts == null)
        {
            return;
        }

        foreach (Shortcut shortcut in _shortcuts)
        {
            bool shortcutUsed = InputManager.Instance.SkillbarInputs[shortcut.Page, shortcut.Slot];
            if (shortcutUsed)
            {
                UseShortcut(shortcut);
            }
        }
    }

    public void UseShortcut(Shortcut shortcut)
    {
        Debug.LogWarning($"Use shortcut {shortcut.Page * 12 + shortcut.Slot}.");
    }

    public void SetShortcutList(Shortcut[] shortcuts)
    {
        if (_shortcuts == null)
        {
            _shortcuts = new List<Shortcut>();
        }
        else
        {
            _shortcuts.Clear();
        }

        for (int i = 0; i < shortcuts.Length; i++)
        {
            Shortcut shortcut = shortcuts[i];
            _shortcuts.Add(shortcut);
        }

        SkillbarWindow.Instance.UpdateShortcuts(_shortcuts);
    }
}
