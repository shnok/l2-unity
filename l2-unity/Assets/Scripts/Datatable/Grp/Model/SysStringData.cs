using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysStringData
{
    [SerializeField] public int _id;
    [SerializeField] public string _name;

    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
}
