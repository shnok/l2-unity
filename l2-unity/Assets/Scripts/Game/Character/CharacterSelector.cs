using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private List<CharSelectionInfoPackage> _characters;
    public List<CharSelectionInfoPackage> Characters { get { return _characters; } set { _characters = value; } }

    private static CharacterSelector _instance;
    public static CharacterSelector Instance { get { return _instance; } }

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
