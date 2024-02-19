using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField] protected int _id;
    [SerializeField] protected int _weight;
    [SerializeField] protected int _price;
    [SerializeField] protected string _name;
    [SerializeField] protected string _description;
    [SerializeField] protected bool _droppable;
    [SerializeField] protected ItemMaterial _material;
    [SerializeField] protected string _icon;
    [SerializeField] protected string _clientName;
    [SerializeField] protected GameObject _prefab;

    public GameObject Prefab { get { return _prefab; } }

    public Item(int id, string name, string description, int weight, int price, bool droppable, ItemMaterial material) {
        _id = id;
        _clientName = name.Replace("'", string.Empty).Replace(" ", "_").ToLower();
        _weight = weight;
        _price = price;
        _name = name;
        _description = description;
        _droppable = droppable;
        _material = material;
    }
}
