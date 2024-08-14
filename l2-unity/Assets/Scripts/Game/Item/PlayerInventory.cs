using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
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
            Debug.Log(items[i].ToString());
        }
    }
}
