using UnityEngine;

[System.Serializable]
public class NetworkIdentity
{
    [SerializeField] private EntityType _entityType;
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private string _title;
    [SerializeField] private string _titleColor = "9CE8A9FF"; // default color

    [Header("Npc")]
    [SerializeField] private int _npcId;
    [SerializeField] private string _npcClass;

    [Header("Player")]
    [SerializeField] private byte _playerClass;
    [SerializeField] private bool _isMage;

    [Header("Transform")]
    [SerializeField] private Vector3 _position = new Vector3(0, 0, 0);
    [SerializeField] private float _heading;

    [SerializeField] private bool _owned = false;

    public EntityType EntityType { get => _entityType; set => _entityType = value; }
    public int Id { get => _id; set => _id = value; }
    public int NpcId { get => _npcId; set => _npcId = value; }
    public string NpcClass { get => _npcClass; set => _npcClass = value; }
    public string Name { get => _name; set => _name = value; }
    public string Title { get => _title; set => _title = value; }
    public string TitleColor { get => _titleColor; set => _titleColor = value; }
    public Vector3 Position { get => _position; set => _position = value; }
    public float Heading { get => _heading; set => _heading = value; }
    public bool Owned { get => _owned; set => _owned = value; }
    public byte PlayerClass { get => _playerClass; set => _playerClass = value; }
    public bool IsMage { get => _isMage; set => _isMage = value; }

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
