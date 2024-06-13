using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }
}
