using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShortcuts : MonoBehaviour
{
    public const int MAXIMUM_SHORTCUTS_PER_BAR = 12;
    public const int MAXIMUM_SKILLBAR_COUNT = 5;
    private int[] _pageMap;

    private List<int> _toggledIds;
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
        _toggledIds = new List<int>();
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

        VerifySkillbarInputs();
    }

    private void VerifySkillbarInputs()
    {
        if (InputManager.Instance == null || InputManager.Instance.SkillbarInputs.Length == 0)
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

    public string GetKeybindForShortcut(int skillbarId, int slot)
    {
        InputAction action = InputManager.Instance.SkillbarActions[skillbarId, slot];
        return action.GetBindingDisplayString(0).ToUpper();
    }

    public void UseShortcut(Shortcut shortcut)
    {
        Debug.Log($"Use shortcut {shortcut.Page * MAXIMUM_SHORTCUTS_PER_BAR + shortcut.Slot}.");
        switch (shortcut.Type)
        {
            case Shortcut.TYPE_ITEM:
                PlayerInventory.Instance.UseItem(shortcut.Id);
                break;
            case Shortcut.TYPE_ACTION:
                PlayerActions.Instance.UseAction((ActionType)shortcut.Id);
                break;
            case Shortcut.TYPE_SKILL:
                PlayerSkill.Instance.UseSkill(shortcut.Id);
                break;
            default:
                Debug.LogWarning("Unkown shortcut type.");
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

        if (SkillbarWindow.Instance == null)
        {
            Debug.LogError("Skillbar window is not ready but already trying to update shortcuts.");
            return;
        }
        StartCoroutine(SkillbarWindow.Instance.UpdateAllShortcuts(shortcuts));
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

    public void RemoveShotcutLocally(int slot)
    {
        SkillbarWindow.Instance.RemoveShortcut(slot);
        _shortcuts.Remove(slot);
    }

    public void UpdatePageMapping(int skillbarIndex, int page)
    {
        _pageMap[skillbarIndex] = page;
    }

    #region ShortcutClientRequests
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

    #endregion

    public void RequestToggleShortcutItem(int id, bool enable)
    {
        GameClient.Instance.ClientPacketHandler.RequestAutoSoulshot(id, enable);
    }

    public void ToggleShortcutItem(int itemId, bool enable)
    {
        if (enable)
        {
            if (!_toggledIds.Contains(itemId))
            {
                Debug.LogWarning($"Adding toggled item with id: {itemId}");
                _toggledIds.Add(itemId);
            }
        }
        else
        {
            if (_toggledIds.Contains(itemId))
            {
                Debug.LogWarning($"Removing toggled item with id: {itemId}");
                _toggledIds.Remove(itemId);
            }
        }

        //Refresh skillbar
        // StartCoroutine(SkillbarWindow.Instance.UpdateAllShortcuts(_shortcuts.Values.ToList())); // TODO: Change this to only update the correct slot -> SkillbarWindow.Instance.AddToggledSlot

        ItemInstance item = PlayerInventory.Instance.GetItemById(itemId);
        if (item == null)
        {
            Debug.LogWarning($"Can't find item with itemId={itemId} in Inventory.");
            return;
        }

        foreach (Shortcut shortcut in Shortcuts)
        {
            if (shortcut.Type == Shortcut.TYPE_ITEM && shortcut.Id == item.ObjectId)
            {
                if (enable)
                {
                    SkillbarWindow.Instance.AddToggledSlot(shortcut.Page, shortcut.Slot);
                }
                else
                {
                    SkillbarWindow.Instance.RemoveToggledSlot(shortcut.Page, shortcut.Slot);
                }
            }
        }
    }

    public bool IsItemToggled(int itemId)
    {
        return _toggledIds.Contains(itemId);
    }

    public void OnSkillUsed(SkillInfo skillInfo)
    {
        // StartCoroutine(SkillbarWindow.Instance.UpdateAllShortcuts(_shortcuts.Values.ToList())); // TODO: Change this to only update the correct slot -> SkillbarWindow.Instance.AddSkillOnCooldown
        foreach (Shortcut shortcut in Shortcuts)
        {
            if (shortcut.Type == Shortcut.TYPE_SKILL && shortcut.Id == skillInfo.Id)
            {
                Debug.LogWarning($"Set shortcut on cooldown: Skill={skillInfo.Id} Page={shortcut.Page} Slot={shortcut.Slot}");
                SkillbarWindow.Instance.AddSkillOnCooldown(shortcut.Page, shortcut.Slot, skillInfo);
            }
        }
    }
}
