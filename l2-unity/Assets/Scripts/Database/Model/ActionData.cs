using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Linq;
using UnityEngine;

public class ActionData 
{
    [SerializeField] public int _id;
    [SerializeField] public string _name;
    [SerializeField] public string _description;
    [SerializeField] public string _icon;

    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public string Descripion { get => _description; set => _description = value; }
    public string Icon { get => _icon; set => _icon = value; }
}
