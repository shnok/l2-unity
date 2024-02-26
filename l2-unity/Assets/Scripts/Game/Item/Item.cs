using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField] protected int _id;
    [SerializeField] protected string _name;
    [SerializeField] protected string _description;
    [SerializeField] protected int _weight;
    [SerializeField] protected ItemSlot _bodypart;
    [SerializeField] protected int _price;
    [SerializeField] protected ItemMaterial _material;
    [SerializeField] protected ItemGrade _grade;
    [SerializeField] protected int _duration;
    [SerializeField] protected int _crystalCount;
    [SerializeField] protected bool _crystallizable;
    [SerializeField] protected bool _sellable;
    [SerializeField] protected bool _droppable;
    [SerializeField] protected bool _destroyable;
    [SerializeField] protected bool _tradeable;
    [SerializeField] protected string _icon;

    public Item(
        int id, 
        string name, 
        string description, 
        ItemSlot bodypart, 
        int weight, 
        int price, 
        ItemMaterial material, 
        ItemGrade grade, 
        int duration,
        bool crystallizable,
        int crystalCount,
        bool sellable,
        bool droppable,
        bool tradeable,
        bool destroyable) {
        _id = id;
        _name = name;
        _description = description;
        _bodypart = bodypart;
        _weight = weight;
        _price = price;
        _grade = grade;
        _material = material;
        _duration = duration;
        _crystallizable = crystallizable;
        _droppable = droppable;
        _destroyable = destroyable;
        _sellable = sellable;
        _crystalCount = crystalCount;
        _tradeable = tradeable;
    }
}
