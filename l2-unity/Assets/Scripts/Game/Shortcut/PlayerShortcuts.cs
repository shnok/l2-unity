using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerShortcuts : MonoBehaviour
{
    public const int MAXIMUM_SHORTCUTS_PER_BAR = 12;
    public const int MAXIMUM_SKILLBAR_COUNT = 5;
    private int[] _pageMap;

    private Dictionary<int, Shortcut> _shortcuts;
    public List<Shortcut> Shortcuts { get { return _shortcuts.Values.ToList(); } }

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
        _pageMap = new int[5] { 0, 1, 2, 3, 4 };
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

        foreach (Shortcut shortcut in _shortcuts.Values)
        {
            for (int i = 0; i < _pageMap.Length; i++)
            {
                if (_pageMap[i] == shortcut.Page)
                {
                    bool shortcutUsed = InputManager.Instance.SkillbarInputs[i, shortcut.Slot];
                    if (shortcutUsed)
                    {
                        UseShortcut(shortcut);
                    }
                }
            }
        }
    }

    public void UseShortcut(Shortcut shortcut)
    {
        Debug.LogWarning($"Use shortcut {shortcut.Page * MAXIMUM_SHORTCUTS_PER_BAR + shortcut.Slot}.");
        switch (shortcut.Type)
        {
            case Shortcut.TYPE_ITEM:
                PlayerInventory.Instance.UseItem(shortcut.Id);
                break;
            default:
                break;
        }
    }

    public void SetShortcutList(List<Shortcut> shortcuts)
    {
        if (_shortcuts == null)
        {
            _shortcuts = new Dictionary<int, Shortcut>();
        }
        else
        {
            _shortcuts.Clear();
        }

        for (int i = 0; i < shortcuts.Count; i++)
        {
            Shortcut shortcut = shortcuts[i];
            _shortcuts.Add(shortcut.Slot + shortcut.Page * MAXIMUM_SHORTCUTS_PER_BAR, shortcut);
        }

        SkillbarWindow.Instance.UpdateAllShortcuts(shortcuts);
    }

    public void RegisterShortcut(Shortcut shortcut)
    {
        if (_shortcuts == null)
        {
            _shortcuts = new Dictionary<int, Shortcut>();
        }

        int slot = shortcut.Slot + shortcut.Page * MAXIMUM_SHORTCUTS_PER_BAR;
        Debug.Log($"Register shortcut {shortcut.Id} at {slot}.");
        if (_shortcuts.TryAdd(slot, shortcut))
        {
            SkillbarWindow.Instance.AddShortcut(shortcut);
        }
        else
        {
            Debug.LogError($"Can't add shotcut in slot {slot}.");
        }

    }

    public Shortcut GetShortcutBySlot(int slot)
    {
        if (_shortcuts.TryGetValue(slot, out Shortcut shortcut))
        {
            return shortcut;
        }

        return null;
    }

    private void RemoveShotcutLocally(int slot)
    {
        SkillbarWindow.Instance.RemoveShortcut(slot);
        _shortcuts.Remove(slot);
    }

    // Shortcut dragged onto skillbar
    public void AddShortcut(int slot, int id, int type)
    {
        GameClient.Instance.ClientPacketHandler.RequestAddShortcut(type, id, slot);
    }

    // Shortcut dragged within bar
    public void MoveShortcut(int oldSlot, int newSlot)
    {
        Shortcut oldShortcut = GetShortcutBySlot(oldSlot);
        Shortcut newShortcut = GetShortcutBySlot(newSlot);
        DeleteShortcut(newSlot);
        DeleteShortcut(oldSlot);

        if (oldShortcut == null)
        {
            Debug.LogError($"MoveShortcut. Old slot is null at {oldSlot}.");
            return;
        }

        GameClient.Instance.ClientPacketHandler.RequestAddShortcut(oldShortcut.Type, oldShortcut.Id, newSlot);

        // Swap slots
        if (newShortcut != null)
        {
            GameClient.Instance.ClientPacketHandler.RequestAddShortcut(newShortcut.Type, newShortcut.Id, oldSlot);
        }
    }

    // Shortcut dragged out of bar
    public void DeleteShortcut(int oldSlot)
    {
        RemoveShotcutLocally(oldSlot);
        GameClient.Instance.ClientPacketHandler.RequestRemoveShortcut(oldSlot);
    }

    public void UpdatePageMapping(int skillbarIndex, int page)
    {
        _pageMap[skillbarIndex] = page;
    }
}
