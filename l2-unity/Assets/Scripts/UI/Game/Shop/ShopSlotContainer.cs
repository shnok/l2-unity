using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class ShopSlotContainer : L2SlotContainer
{
    [SerializeField] private List<Product> _products;
    [SerializeField] private L2Slot.SlotType _slotType;

    public void Initialize(VisualElement container, int rowLength, L2Slot.SlotType type, int minimumContainerSize)
    {
        base.Initialize(container, rowLength, minimumContainerSize);

        _slotType = type;
    }

    public void RefreshProducts(Product[] products)
    {
        if (products != null)
        {
            _products = products.ToList();
        }
        else
        {
            _products = new List<Product>();
        }

        RefreshProducts();
    }

    private void RefreshProducts()
    {
        List<ItemInstance> items = new List<ItemInstance>();

        for (int i = 0; i < _products.Count; i++)
        {
            Product p = _products[i];
            ItemInstance item = new ItemInstance(p.ObjectId, p.ItemId, ItemLocation.Void, i, p.Count, p.Type1, p.Type2, false, p.BodyPart, 0, 0);
            items.Add(item);
        }

        UpdateSlots(items.Count, _slotType);

        AssignItemsToSlots(items);
        AssignProductsToSlots();
    }

    public void AddOrUpdateProduct(Product product, int quantity)
    {
        if (quantity == 0)
        {
            quantity = 1;
        }

        int slot = GetProductSlot(product.ItemId);
        if (slot != -1)
        {
            if (quantity < 0 && _products[slot].Count + quantity <= 0)
            {
                RemoveProduct(slot);
            }
            else if (_products[slot].Type1 == ItemType1.TYPE1_ITEM_QUESTITEM_ADENA)
            {
                _products[slot].Count += quantity;
            }
            else
            {
                _products.Add(product);
            }
        }
        else
        {
            _products.Add(product);
        }
    }

    public void RemoveProduct(int slot)
    {
        _products.RemoveAt(slot);
    }

    private int GetProductSlot(int itemId)
    {
        for (int i = 0; i < _products.Count; i++)
        {
            if (_products[i].ItemId == itemId)
            {
                return i;
            }
        }

        return -1;
    }

    public void AddToBasket(Product product)
    {
        AddOrUpdateProduct(product, product.Count);
        //_products.Remove(product);
        RefreshProducts();
    }

    public void RemoveFromBasket(Product product, int index)
    {
        //_products.Remove(product);
        RemoveProduct(index);
        RefreshProducts();
    }

    private void AssignProductsToSlots()
    {
        if (_products == null)
        {
            return;
        }

        for (int i = 0; i < _products.Count; i++)
        {
            if (i < _slots.Length)
            {
                ((ProductSlot)_slots[i]).AssignProduct(_products[i]);
            }
            else
            {
                Debug.LogWarning("Product slot index out of range");
            }
        }
    }
}