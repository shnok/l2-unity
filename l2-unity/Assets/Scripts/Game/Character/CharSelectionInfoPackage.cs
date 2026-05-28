using UnityEngine;

[System.Serializable]
public struct CharSelectionInfoPackage
{
    [SerializeField] private int _slot;
    [SerializeField] private string _name;
    [SerializeField] private string _account;
    [SerializeField] private int _id;
    [SerializeField] private Vector3 _position;
    [SerializeField] private PlayerAppearance _playerAppearance;
    [SerializeField] private CharacterModelType _characterRaceAnimation;
    [SerializeField] private PlayerStatus _playerStatus;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private bool _isMage;
    [SerializeField] private byte _classId;
    [SerializeField] private byte _baseClassId;
    [SerializeField] private int _exp;
    [SerializeField] private int _sp;
    [SerializeField] private float _expPercent;
    [SerializeField] private int _karma;
    [SerializeField] private int _pvpKills;
    [SerializeField] private int _pkKills;
    [SerializeField] private int _deleteTimer;
    [SerializeField] private int _clanId;
    [SerializeField] private bool _selected;

    public PlayerAppearance PlayerAppearance { get => _playerAppearance; set => _playerAppearance = value; }
    public CharacterModelType CharacterRaceAnimation { get => _characterRaceAnimation; set => _characterRaceAnimation = value; }
    public Vector3 Position { get => _position; set => _position = value; }
    public string Name { get => _name; set => _name = value; }
    public string Account { get => _account; set => _account = value; }
    public int Id { get => _id; set => _id = value; }
    public int Slot { get => _slot; set => _slot = value; }
    public bool IsMage { get => _isMage; set => _isMage = value; }
    public byte ClassId { get => _classId; set => _classId = value; }
    public PlayerStatus PlayerStatus { get => _playerStatus; set => _playerStatus = value; }
    public PlayerStats PlayerStats { get => _playerStats; set => _playerStats = value; }
    public int Exp { get => _exp; set => _exp = value; }
    public int Sp { get => _sp; set => _sp = value; }
    public float ExpPercent { get => _expPercent; set => _expPercent = value; }
    public int Karma { get => _karma; set => _karma = value; }
    public int PvpKills { get => _pvpKills; set => _pvpKills = value; }
    public int PkKills { get => _pkKills; set => _pkKills = value; }
    public int DeleteTimer { get => _deleteTimer; set => _deleteTimer = value; }
    public int ClanId { get => _clanId; set => _clanId = value; }
    public bool Selected { get => _selected; set => _selected = value; }
    public byte BaseClassId { get => _baseClassId; set => _baseClassId = value; }

}
