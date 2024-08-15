using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    enum InventoryChange {
        UNCHANGED = 0, ADDED = 1, REMOVED = 3, MODIFIED = 2
    }

    private List<ItemInstance> _playerInventory;

    public List<ItemInstance> Items { get { return _playerInventory; } }

    public static PlayerInventory _instance;
    public static PlayerInventory Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
        }

        Debug.LogWarning("Init PlayerInventory");

        _playerInventory = new List<ItemInstance>();
    }

    private void Start() {
        _playerInventory.Clear();
    }

    private void OnDestroy() {
        _instance = null;
    }

    public void SetInventory(ItemInstance[] items, bool openInventory) {
        _playerInventory = items.ToList();

        InventoryWindow.Instance.UpdateItemList(_playerInventory);

        if(openInventory) {
            InventoryWindow.Instance.ShowWindow();
        }
    }

    public void UpdateInventory(ItemInstance[] items) {
        for(int i = 0; i < items.Length; i++) {
            ItemInstance item = items[i];
            Debug.Log(item.ToString());
            if(item.LastChange == (int) InventoryChange.ADDED) {
                _playerInventory.Add(item);
            } else if (item.LastChange == (int) InventoryChange.MODIFIED) {
                ItemInstance oldItem = GetItemByObjectId(item.ObjectId);
                if(oldItem == null) {
                    _playerInventory.Add(item);
                } else {
                    oldItem.Update(item);
                }
            } else if(item.LastChange == (int) InventoryChange.REMOVED) {
                ItemInstance oldItem = GetItemByObjectId(item.ObjectId);
                _playerInventory.Remove(oldItem);
            }
        }

        InventoryWindow.Instance.UpdateItemList(_playerInventory);
    }

    public ItemInstance GetItemByObjectId(int objectId) {
        foreach(ItemInstance item in _playerInventory) {
            if(item.ObjectId == objectId) {
                return item;
            }
        }

        return null;
    }
}
