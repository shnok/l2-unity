using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Linq;
using UnityEngine;

public class ActionData
{
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private string _icon;

    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public string Descripion { get => _description; set => _description = value; }
    public string Icon { get => _icon; set => _icon = value; }
}
