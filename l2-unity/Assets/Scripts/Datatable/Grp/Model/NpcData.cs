using UnityEngine;

[System.Serializable]
public class NpcData {
    [SerializeField] private NpcName _npcName;
    [SerializeField] private Npcgrp _npcgrp;

    public NpcName NpcName { get { return _npcName; } set { _npcName = value; } }
    public Npcgrp Npcgrp { get { return _npcgrp; } set { _npcgrp = value;} }

    public NpcData(NpcName npcName, Npcgrp npcgrp) {
        _npcName = npcName;
        _npcgrp = npcgrp;
    }
}