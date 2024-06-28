using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState : byte { 
    LOGIN_SCREEN = 0, 
    LOGIN_CONNECTED = 1, 
    READING_LICENSE = 2, 
    SERVER_LIST = 3, 
    CHAR_SELECT = 4, 
    CHAR_CREATION = 5, 
    IN_GAME = 6 
}