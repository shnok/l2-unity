using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkIdentity
{
    [SerializeField] private EntityType _entityType;
    [SerializeField] private int _id;
    [SerializeField] private string _type;
    [SerializeField] private string _npcClass;
    [SerializeField] private int _npcId;
    [SerializeField] private string _name;
    [SerializeField] private string _title;
    [SerializeField] private int _model;
    [SerializeField] private Vector3 _position = new Vector3(0, 0, 0);
    [SerializeField] private float _heading;
    [SerializeField] private float _collisionHeight;
    [SerializeField] private bool _owned = false;

    public EntityType EntityType { get => _entityType; set => _entityType = value; }
    public int Id { get => _id; set => _id = value; }
    public int NpcId { get => _npcId; set => _npcId = value; }
    public string Type { get => _type; set => _type = value; }
    public string NpcClass { get => _npcClass; set => _npcClass = value; }
    public string Name { get => _name; set => _name = value; }
    public string Title { get => _title; set => _title = value; }
    public int Model { get => _model; set => _model = value; }
    public Vector3 Position { get => _position; set => _position = value; }
    public float Heading { get => _heading; set => _heading = value; }
    public float CollisionHeight { get => _collisionHeight; set => _collisionHeight = value; }
    public bool Owned { get => _owned; set => _owned = value; }

    public NetworkIdentity() {}

    public void SetPosX(float x) {
        _position.x = x;
    }

    public void SetPosY(float y) {
        _position.y = y;
    }

    public void SetPosZ(float z) {
        _position.z = z;
    }
}
