using UnityEngine;

[System.Serializable]
public class NpcStatus : Status {
    [SerializeField] private bool _friendly = false;
    [SerializeField] private bool _aggressive = false;

    public bool Friendly { get => _friendly; set => _friendly = value; }
    public bool Aggressive { get => _aggressive; set => _aggressive = value; }

    public NpcStatus() {}
}