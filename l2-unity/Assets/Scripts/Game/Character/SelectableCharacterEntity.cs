using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableCharacterEntity : MonoBehaviour
{
    [SerializeField] private CharSelectionInfoPackage _characterInfo;

    public CharSelectionInfoPackage CharacterInfo { get { return _characterInfo; } set { _characterInfo = value; } }
}
