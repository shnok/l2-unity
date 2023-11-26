using UnityEngine;

[System.Serializable]
public class NpcStatus : Status {
    [SerializeField]
    private bool friendly = false;
    [SerializeField]
    private bool aggressive = false;

    public bool Friendly { get => friendly; set => friendly = value; }
    public bool Aggressive { get => aggressive; set => aggressive = value; }

    public NpcStatus() {}
    

}