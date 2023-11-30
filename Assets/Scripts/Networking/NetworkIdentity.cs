using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkIdentity
{
    [SerializeField]
    private int id;
    [SerializeField]
    private int npcId;
    [SerializeField]
    private string name;
    [SerializeField]
    private int model;
    [SerializeField]
    private Vector3 position = new Vector3(0, 0, 0);
    [SerializeField]
    private bool owned = false;

    public int Id { get => id; set => id = value; }
    public int NpcId { get => npcId; set => npcId = value; }
    public string Name { get => name; set => name = value; }
    public int Model { get => model; set => model = value; }
    public Vector3 Position { get => position; set => position = value; }
    public bool Owned { get => owned; set => owned = value; }

    public NetworkIdentity() {}
    public NetworkIdentity(int id) {
        Id = id;
    }

    public NetworkIdentity(int id, string name) {
        Id = id;
        Name = name;
    }

    public void SetPosX(float x) {
        position.x = x;
    }

    public void SetPosY(float y) {
        position.y = y;
    }
    public void SetPosZ(float z) {
        position.z = z;
    }

    public int GetPosX() {
        return (int)position.x;
    }

    public int GetPosY() {
        return (int)position.y;
    }

    public int GetPosZ() {
        return (int)position.z;
    }
}
