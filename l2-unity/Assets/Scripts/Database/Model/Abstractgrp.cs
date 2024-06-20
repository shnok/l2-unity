using UnityEngine;

[System.Serializable]
public class Abstractgrp {
    [SerializeField] protected int _objectId;
    [SerializeField] protected string _dropModel;
    [SerializeField] protected string _dropTexture;
    [SerializeField] protected string _icon;
    [SerializeField] protected int _weight;
    [SerializeField] protected ItemMaterial _material;
    [SerializeField] protected string _dropSound;
    [SerializeField] private ItemGrade _grade;
    [SerializeField] protected string _equipSound;
    [SerializeField] protected string _inventoryType;
    [SerializeField] private bool _crystallizable;

    public ItemMaterial Material { get { return _material; } set { _material = value; } }
    public int ObjectId { get { return _objectId; } set { _objectId = value; } }
    public int Weight { get { return _weight; } set { _weight = value; } }
    public string DropModel { get { return _dropModel; } set { _dropModel = value; } }
    public string DropTexture { get { return _dropTexture; } set { _dropTexture = value; } }
    public string Icon { get { return _icon; } set { _icon = value; } }
    public string DropSound { get { return _dropSound; } set { _dropSound = value; } }
    public string EquipSound { get { return _equipSound; } set { _equipSound = value; } }
    public string InventoryType { get { return _inventoryType; } set { _inventoryType = value; } }
    public bool Crystallizable { get { return _crystallizable; } set { _crystallizable = value; } }
    public ItemGrade Grade { get { return _grade; } set { _grade = value; } }
}
